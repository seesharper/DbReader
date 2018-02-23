namespace DbReader.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Database;
    using Extensions;
    using Selectors;

    /// <summary>
    /// A class that based on a given sql and the type of the arguments object,
    /// can create a method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.
    /// </summary>
    public class ArgumentParserMethodBuilder : IArgumentParserMethodBuilder
    {
        private readonly IPropertySelector readablePropertySelector;
        private readonly IParameterParser parameterParser;
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private static readonly MethodInfo ParameterFactoryInvokeMethod;
        private static readonly MethodInfo DataParameterSetParameterNameMethod;
        private static readonly MethodInfo DataParameterSetValueMethod;
        private static readonly MethodInfo ProcessDelegateInvokeMethod;
        private static readonly MethodInfo SetNameMethod;

        static ArgumentParserMethodBuilder()
        {            
            ParameterFactoryInvokeMethod = typeof (Func<IDataParameter>).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "Invoke");
            DataParameterSetParameterNameMethod =
                typeof (IDataParameter).GetTypeInfo().GetProperty("ParameterName").SetMethod;
            DataParameterSetValueMethod =
                typeof(IDataParameter).GetTypeInfo().GetProperty("Value").SetMethod;
            ProcessDelegateInvokeMethod = typeof(Action<IDataParameter, object>).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "Invoke");
            SetNameMethod = typeof (DataParameterHelper).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "SetName");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentParserMethodBuilder"/> class.
        /// </summary>
        /// <param name="readablePropertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting readable properties from a given type.</param>
        /// <param name="parameterParser">The <see cref="IParameterParser"/> that is responsible for parsing parameters from a SQL statement.</param>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for providing a <see cref="IMethodSkeleton"/>.</param>
        public ArgumentParserMethodBuilder(IPropertySelector readablePropertySelector, IParameterParser parameterParser, IMethodSkeletonFactory methodSkeletonFactory)
        {
            this.readablePropertySelector = readablePropertySelector;
            this.parameterParser = parameterParser;
            this.methodSkeletonFactory = methodSkeletonFactory;
        }


        /// <summary>
        /// Creates a method at runtime that maps an argument object instance into a list of data parameters.
        /// </summary>
        /// <param name="sql">The sql statement for which to create the method.</param>
        /// <param name="argumentsType">The arguments type for which to create the method.</param>
        /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        public Func<object, Func<IDataParameter> ,IDataParameter[]> CreateMethod(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {
            
            var processDelegates = new List<Action<IDataParameter, object>>();

            var parameterNames = parameterParser.GetParameters(sql);            
            var properties = readablePropertySelector.Execute(argumentsType).OrderByDeclaration().ToArray();
            if (parameterNames.Length > 0)
            {
                properties = properties.Where(p => parameterNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
                ValidateParameters(parameterNames, properties, existingParameters);
            }
            else
            {
                parameterNames = properties.Select(p => p.Name).ToArray();
            }


            var parameters = parameterNames.ToDictionary(n => n, StringComparer.OrdinalIgnoreCase);

            var dm = methodSkeletonFactory.GetMethodSkeleton("ParseArguments", typeof(IDataParameter[]),
                new[] { typeof(object), typeof(Func<IDataParameter>) ,typeof(Action<IDataParameter, object>[]) }, argumentsType);
           
            
            var generator = dm.GetGenerator();

            LocalBuilder arguments = generator.DeclareLocal(argumentsType);
            LocalBuilder dataParameter = generator.DeclareLocal(typeof (IDataParameter));

            //Create the result array
            generator.EmitFastInt(properties.Length);
            generator.Emit(OpCodes.Newarr, typeof(IDataParameter));

            //Load the arguments object and store it in a local variable.
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, argumentsType);
            generator.Emit(OpCodes.Stloc, arguments);

            for (int i = 0; i < properties.Length; i++)
            {
                if (typeof(IDataParameter).GetTypeInfo().IsAssignableFrom(properties[i].PropertyType))
                {
                    generator.Emit(OpCodes.Ldloc, arguments);
                    generator.Emit(OpCodes.Callvirt, properties[i].GetMethod);

                    //Store the dataparameter directly from the property
                    generator.Emit(OpCodes.Stloc, dataParameter);
                    generator.Emit(OpCodes.Ldloc, dataParameter);
                    generator.Emit(OpCodes.Ldstr, properties[i].Name);

                    //Set the name of the parameter if the parameter name if null
                    generator.Emit(OpCodes.Call, SetNameMethod);

                    //Duplicate the topmost value on the stack (IDataParameter[])
                    generator.Emit(OpCodes.Dup);
                    generator.EmitFastInt(i);
                    generator.Emit(OpCodes.Ldloc, dataParameter);
                    generator.Emit(OpCodes.Stelem_Ref);
                }
                else
                {
                    //Load, execute the parameter factory and store the dataparameter in a local variable
                    generator.Emit(OpCodes.Ldarg_1);
                    generator.Emit(OpCodes.Callvirt, ParameterFactoryInvokeMethod);
                    generator.Emit(OpCodes.Stloc, dataParameter);

                    //Set the parameter name;
                    generator.Emit(OpCodes.Ldloc, dataParameter);
                    generator.Emit(OpCodes.Ldstr, parameters[properties[i].Name]);
                    generator.Emit(OpCodes.Callvirt, DataParameterSetParameterNameMethod);

                    Action<IDataParameter, object> processDelegate = ArgumentProcessor.GetProcessDelegate(properties[i].PropertyType);
                    if (processDelegate != null)
                    {
                        processDelegates.Add(processDelegate);
                        int processDelegateIndex = processDelegates.Count - 1;

                        //Load the argument process delegate
                        generator.Emit(OpCodes.Ldarg_2);
                        generator.EmitFastInt(processDelegateIndex);
                        generator.Emit(OpCodes.Ldelem_Ref);

                        generator.Emit(OpCodes.Ldloc, dataParameter);
                        generator.Emit(OpCodes.Ldloc, arguments);
                        generator.Emit(OpCodes.Callvirt, properties[i].GetMethod);
                        if (properties[i].PropertyType.GetTypeInfo().IsValueType)
                        {
                            generator.Emit(OpCodes.Box, properties[i].PropertyType);
                        }
                        generator.Emit(OpCodes.Callvirt, ProcessDelegateInvokeMethod);

                        //Duplicate the topmost value on the stack (IDataParameter[])
                        generator.Emit(OpCodes.Dup);
                        generator.EmitFastInt(i);
                        generator.Emit(OpCodes.Ldloc, dataParameter);
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldloc, dataParameter);
                        generator.Emit(OpCodes.Ldloc, arguments);
                        generator.Emit(OpCodes.Callvirt, properties[i].GetMethod);
                        if (properties[i].PropertyType.GetTypeInfo().IsValueType)
                        {
                            generator.Emit(OpCodes.Box, properties[i].PropertyType);
                        }
                        generator.Emit(OpCodes.Callvirt, DataParameterSetValueMethod);

                        //Duplicate the topmost value on the stack (IDataParameter[])
                        generator.Emit(OpCodes.Dup);
                        generator.EmitFastInt(i);
                        generator.Emit(OpCodes.Ldloc, dataParameter);
                        generator.Emit(OpCodes.Stelem_Ref);
                    }
                }                                
            }
                                  
            generator.Emit(OpCodes.Ret);
            
            var method =
                (Func<object, Func<IDataParameter>, Action<IDataParameter, object>[], IDataParameter[]>)
                    dm.CreateDelegate(typeof (Func<object, Func<IDataParameter>, Action<IDataParameter, object>[], IDataParameter[]>));

            return (args, parameterFactory) =>  method(args, parameterFactory, processDelegates.ToArray());            
        }

        private void ValidateParameters(string[] parameterNames, PropertyInfo[] properties, IDataParameter[] existingParameters)
        {
            var propertyNames = new HashSet<string>(properties.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);            
            var existingParameterNames = new HashSet<string>(existingParameters.Select(p => p.ParameterName), StringComparer.OrdinalIgnoreCase);

            var firstDuplicateParameterName = propertyNames.Intersect(existingParameterNames, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
            if (firstDuplicateParameterName != null)
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateParameter.FormatWith(firstDuplicateParameterName));
            }

            foreach (var parameterName in parameterNames)
            {
                if (!propertyNames.Contains(parameterName) && !existingParameterNames.Contains(parameterName))
                {
                    throw new InvalidOperationException(ErrorMessages.MissingArgument.FormatWith(parameterName));
                }                
            }


        }

        
    }
}