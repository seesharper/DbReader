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
        private readonly IParameterMatcher parameterMatcher;
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private readonly IParameterValidator parameterValidator;
        private static readonly MethodInfo ParameterFactoryInvokeMethod;
        private static readonly MethodInfo DataParameterSetParameterNameMethod;
        private static readonly MethodInfo DataParameterSetValueMethod;
        private static readonly MethodInfo ProcessDelegateInvokeMethod;
        private static readonly MethodInfo SetNameMethod;
        private static readonly MethodInfo ToDbNullIfNullMethod;

        private static readonly ConstructorInfo ListConstructor;

        private static readonly MethodInfo AddMethod;

        private static readonly MethodInfo AddDataParameterMethod;

        private static readonly MethodInfo AddAndConvertMethod;

        private static readonly MethodInfo AddEnumerableMethod;

        private static readonly MethodInfo AddAndConvertEnumerable;

        private static readonly MethodInfo CreateQueryInfoMethod;




        static ArgumentParserMethodBuilder()
        {
            ParameterFactoryInvokeMethod = typeof(Func<IDataParameter>).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "Invoke");
            DataParameterSetParameterNameMethod =
                typeof(IDataParameter).GetTypeInfo().GetProperty("ParameterName").SetMethod;
            DataParameterSetValueMethod =
                typeof(IDataParameter).GetTypeInfo().GetProperty("Value").SetMethod;
            ProcessDelegateInvokeMethod = typeof(Action<IDataParameter, object>).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "Invoke");
            SetNameMethod = typeof(DataParameterHelper).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "SetName");
            ToDbNullIfNullMethod = typeof(DbNullConverter).GetMethod("ToDbNullIfNull", BindingFlags.Static | BindingFlags.Public);

            ListConstructor = typeof(List<IDataParameter>).GetConstructor(Type.EmptyTypes);

            AddDataParameterMethod = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.AddDataParameter));

            AddMethod = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.Add));

            AddAndConvertMethod = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.AddAndConvert));

            AddEnumerableMethod = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.AddEnumerable));

            CreateQueryInfoMethod = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.CreateQueryInfo));

            AddAndConvertEnumerable = typeof(ParameterHelper).GetMethod(nameof(ParameterHelper.AddAndConvertEnumerable));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentParserMethodBuilder"/> class.
        /// </summary>

        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for providing a <see cref="IMethodSkeleton"/>.</param>
        public ArgumentParserMethodBuilder(IParameterMatcher parameterMatcher, IMethodSkeletonFactory methodSkeletonFactory)
        {
            this.parameterMatcher = parameterMatcher;
            this.methodSkeletonFactory = methodSkeletonFactory;
        }




        public Func<string, object, Func<IDataParameter>, QueryInfo> CreateMethod2(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {
            // var dataParameters = parameterParser.GetParameters(sql);

            // var propertyMap = readablePropertySelector.Execute(argumentsType).ToDictionary(p => p.Name, p => p, StringComparer.InvariantCultureIgnoreCase);

            // var matchedParameters = dataParameters.Select(dp => new { DataParameter = dp, Property = propertyMap[dp.Name] });

            var matchedParameters = parameterMatcher.Match(sql, argumentsType, existingParameters);

            var dynamicMethod = methodSkeletonFactory.GetMethodSkeleton("ParseArguments", typeof(QueryInfo),
               new[] { typeof(string), typeof(object), typeof(Func<IDataParameter>) }, argumentsType);

            var generator = dynamicMethod.GetGenerator();

            generator.DeclareLocal(typeof(List<IDataParameter>));
            generator.DeclareLocal(argumentsType);

            // Create the parameter list and store it in a local variable.
            generator.Emit(OpCodes.Newobj, ListConstructor);
            generator.Emit(OpCodes.Stloc_0);

            // Load the arguments object, cast it and store it in the local variable.
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Castclass, argumentsType);
            generator.Emit(OpCodes.Stloc_1);

            foreach (var matchedParameter in matchedParameters)
            {
                if (matchedParameter.Property.IsDataParameter())
                {
                    // Load the local data parameter list onto the stack.
                    generator.Emit(OpCodes.Ldloc_0);

                    // Load the arguments object onto the stack
                    generator.Emit(OpCodes.Ldloc_1);

                    // Load the IDataParameter instance from the property.
                    generator.Emit(OpCodes.Callvirt, matchedParameter.Property.GetMethod);

                    // Load the property name on the stack (If the parameter name is empty)
                    generator.Emit(OpCodes.Ldstr, matchedParameter.Property.Name);


                    // Call the AddDataParameter method.
                    generator.Emit(OpCodes.Call, AddDataParameterMethod);
                }
                else
                {
                    if (ArgumentProcessor.CanProcess(matchedParameter.Property.PropertyType))
                    {
                        // Load the local data parameter list onto the stack.
                        generator.Emit(OpCodes.Ldloc_0);

                        // Load the parameter name onto the stack.
                        generator.Emit(OpCodes.Ldstr, matchedParameter.DataParameter.Name);

                        // Load the arguments object onto the stack
                        generator.Emit(OpCodes.Ldloc_1);

                        // Load the property value onto the stack.
                        generator.Emit(OpCodes.Callvirt, matchedParameter.Property.GetMethod);

                        // Load the parameter factory onto the stack.
                        generator.Emit(OpCodes.Ldarg_2);

                        // Call the AddAndConvert method.
                        generator.Emit(OpCodes.Call, AddAndConvertMethod.MakeGenericMethod(matchedParameter.Property.PropertyType));
                    }
                    else
                    {
                        if (matchedParameter.DataParameter.IsListParameter)
                        {
                            // Load the local data parameter list onto the stack.
                            generator.Emit(OpCodes.Ldloc_0);

                            // Load the parameter full name onto the stack.
                            generator.Emit(OpCodes.Ldstr, matchedParameter.DataParameter.Name);

                            // Load the parameter full name onto the stack.
                            generator.Emit(OpCodes.Ldstr, matchedParameter.DataParameter.FullName);

                            // Load the arguments object onto the stack
                            generator.Emit(OpCodes.Ldloc_1);

                            // Load the property value onto the stack.
                            generator.Emit(OpCodes.Callvirt, matchedParameter.Property.GetMethod);

                            // Load the parameter factory onto the stack.
                            generator.Emit(OpCodes.Ldarg_2);

                            // Load the address of the sql parameter onto the stack (ref)
                            // This is the first parameter passed into the dynamic method.
                            generator.Emit(OpCodes.Ldarga_S, (byte)0);

                            var projectionType = matchedParameter.Property.PropertyType.GetProjectionType();
                            if (ArgumentProcessor.CanProcess(projectionType))
                            {
                                generator.Emit(OpCodes.Call, AddAndConvertEnumerable.MakeGenericMethod(projectionType));
                            }
                            else
                            {
                                generator.Emit(OpCodes.Call, AddEnumerableMethod.MakeGenericMethod(projectionType));
                            }
                        }
                        else
                        {
                            // Load the local data parameter list onto the stack.
                            generator.Emit(OpCodes.Ldloc_0);

                            // Load the parameter name onto the stack.
                            generator.Emit(OpCodes.Ldstr, matchedParameter.DataParameter.Name);

                            // Load the arguments object onto the stack
                            generator.Emit(OpCodes.Ldloc_1);

                            // Load the property value onto the stack.
                            generator.Emit(OpCodes.Callvirt, matchedParameter.Property.GetMethod);

                            // Load the parameter factory onto the stack.
                            generator.Emit(OpCodes.Ldarg_2);

                            // Call the AddAndConvert method.
                            generator.Emit(OpCodes.Call, AddMethod.MakeGenericMethod(matchedParameter.Property.PropertyType));
                        }
                    }
                }
            }

            // Load the sql paramater onto the stack.
            generator.Emit(OpCodes.Ldarg_0);

            // Load the data parameter list variable onto the stack.
            generator.Emit(OpCodes.Ldloc_0);

            // Call the CreateQueryInfo method.
            generator.Emit(OpCodes.Call, CreateQueryInfoMethod);

            // Return from the dynamic method.
            generator.Emit(OpCodes.Ret);

            var method =
                (Func<string, object, Func<IDataParameter>, QueryInfo>)
                    dynamicMethod.CreateDelegate(typeof(Func<string, object, Func<IDataParameter>, QueryInfo>));
            return method;
        }


        /// <summary>
        /// Creates a method at runtime that maps an argument object instance into a list of data parameters.
        /// </summary>
        /// <param name="sql">The sql statement for which to create the method.</param>
        /// <param name="argumentsType">The arguments type for which to create the method.</param>
        /// <param name="existingParameters">A list of already existing parameters.</param>
        /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        public Func<object, Func<IDataParameter>, IDataParameter[]> CreateMethod(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {

            var processDelegates = new List<Action<IDataParameter, object>>();
            var dataParameters = parameterParser.GetParameters(sql);
            var parameterNames = dataParameters.Select(p => p.Name).ToArray();
            var properties = readablePropertySelector.Execute(argumentsType).OrderByDeclaration().ToArray();
            if (parameterNames.Length > 0)
            {
                properties = properties.Where(p => parameterNames.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
                parameterValidator.ValidateParameters(dataParameters, properties, existingParameters);
            }
            else
            {
                parameterNames = properties.Select(p => p.Name).ToArray();
            }


            var parameters = parameterNames.ToDictionary(n => n, StringComparer.OrdinalIgnoreCase);

            var dm = methodSkeletonFactory.GetMethodSkeleton("ParseArguments", typeof(IDataParameter[]),
                new[] { typeof(object), typeof(Func<IDataParameter>), typeof(Action<IDataParameter, object>[]) }, argumentsType);


            var generator = dm.GetGenerator();

            LocalBuilder arguments = generator.DeclareLocal(argumentsType);
            LocalBuilder dataParameter = generator.DeclareLocal(typeof(IDataParameter));

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
                        else
                        {
                            generator.Emit(OpCodes.Call, ToDbNullIfNullMethod);
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
                    dm.CreateDelegate(typeof(Func<object, Func<IDataParameter>, Action<IDataParameter, object>[], IDataParameter[]>));

            return (args, parameterFactory) => method(args, parameterFactory, processDelegates.ToArray());
        }
    }

    public static class DbNullConverter
    {
        public static object ToDbNullIfNull(object value)
        {
            return value ?? DBNull.Value;
        }
    }


    internal static class ParameterHelper
    {
        public static void Add<T>(List<IDataParameter> dataParameters, string name, T value, Func<IDataParameter> parameterFactory)
        {
            IDataParameter dataParameter = CreateParameter(name, value, parameterFactory);
            dataParameters.Add(dataParameter);
        }

        public static void AddAndConvert<T>(List<IDataParameter> dataParameters, string name, T value, Func<IDataParameter> parameterFactory)
        {
            var dataParameter = CreateParameter<T>(name, value, parameterFactory);
            ArgumentProcessor.Process(typeof(T), dataParameter, value);
            dataParameters.Add(dataParameter);
        }

        public static void AddEnumerable<T>(List<IDataParameter> dataParameters, string name, string fullName, IEnumerable<T> values, Func<IDataParameter> parameterFactory, ref string sql)
        {
            int i = 0;
            var expandedParameterList = new List<string>();

            foreach (var value in values)
            {
                var listParameterName = $"{name}{i}";
                var dataParameter = CreateParameter(listParameterName, value, parameterFactory);
                dataParameters.Add(dataParameter);
                var expandedParameterName = $"{fullName}{i}";
                expandedParameterList.Add(expandedParameterName);
                i++;
            }
            var expandedParameterFragment = expandedParameterList.Aggregate((current, next) => $"{current}, {next}");
            sql = sql.Replace(fullName, expandedParameterFragment);
        }

        public static void AddAndConvertEnumerable<T>(List<IDataParameter> dataParameters, string name, string fullName, IEnumerable<T> values, Func<IDataParameter> parameterFactory, ref string sql)
        {
            int i = 0;
            var expandedParameterList = new List<string>();

            foreach (var value in values)
            {
                var listParameterName = $"{name}{i}";
                var dataParameter = CreateParameter(listParameterName, value, parameterFactory);
                ArgumentProcessor.Process(typeof(T), dataParameter, value);
                dataParameters.Add(dataParameter);
                var expandedParameterName = $"{fullName}{i}";
                expandedParameterList.Add(expandedParameterName);
                i++;
            }
            var expandedParameterFragment = expandedParameterList.Aggregate((current, next) => $"{current}, {next}");
            sql = sql.Replace(fullName, expandedParameterFragment);
        }


        public static void AddDataParameter(List<IDataParameter> dataParameters, IDataParameter dataParameter, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(dataParameter.ParameterName))
            {
                dataParameter.ParameterName = propertyName;
            }
            dataParameters.Add(dataParameter);
        }



        private static IDataParameter CreateParameter<T>(string name, T value, Func<IDataParameter> parameterFactory)
        {
            var dataParameter = parameterFactory();
            dataParameter.ParameterName = name;
            dataParameter.Value = DbNullConverter.ToDbNullIfNull(value);
            return dataParameter;
        }

        public static QueryInfo CreateQueryInfo(string sql, List<IDataParameter> dataParameters)
        {
            return new QueryInfo(sql, dataParameters.ToArray());
        }
    }
}