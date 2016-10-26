namespace DbReader.Construction
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;
    using Selectors;

    public interface IArgumentParserMethodBuilder
    {
        Action<object, Dictionary<string, ArgumentValue>> CreateMethod(Type argumentsType);
    }

    public class ArgumentParserMethodBuilder : IArgumentParserMethodBuilder
    {
        private readonly IPropertySelector readablePropertySelector;
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private static readonly ConstructorInfo ArgumentConstructor;
        private static readonly FieldInfo ArgumentValueField;
        private static readonly FieldInfo ArgumentTypeField;
        private static readonly MethodInfo DictionaryAddMethod;

        static ArgumentParserMethodBuilder()
        {
            TypeInfo argumentTypeInfo = typeof (ArgumentValue).GetTypeInfo();
            ArgumentConstructor = argumentTypeInfo.DeclaredConstructors.First();
            ArgumentValueField = argumentTypeInfo.DeclaredFields.Single(f => f.Name == "Value");
            ArgumentTypeField = argumentTypeInfo.DeclaredFields.Single(f => f.Name == "Type");
            DictionaryAddMethod = typeof(Dictionary<string, ArgumentValue>).GetTypeInfo().GetDeclaredMethod("Add");
        }

        public ArgumentParserMethodBuilder(IPropertySelector readablePropertySelector, IMethodSkeletonFactory methodSkeletonFactory)
        {
            this.readablePropertySelector = readablePropertySelector;
            this.methodSkeletonFactory = methodSkeletonFactory;
        }


        public Action<object, Dictionary<string, ArgumentValue>> CreateMethod(Type argumentsType)
        {
            var properties = readablePropertySelector.Execute(argumentsType);
            var dm = methodSkeletonFactory.GetMethodSkeleton("ParseArguments", typeof(void),
                new[] { typeof(object), typeof(Dictionary<string, ArgumentValue>), typeof(Type[]) }, argumentsType);

            //DynamicMethod dm = new DynamicMethod("ParseArguments", typeof(void),
            //    new[] { typeof(object), typeof(Dictionary<string, object>), typeof(Type[]) }, argumentsType,true);

            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var generator = dm.GetGenerator();
            LocalBuilder instance = generator.DeclareLocal(argumentsType);
            LocalBuilder argumentValue = generator.DeclareLocal(typeof (ArgumentValue));


            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, argumentsType);
            generator.Emit(OpCodes.Stloc, instance);
            for (int index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                generator.Emit(OpCodes.Newobj, ArgumentConstructor);
                generator.Emit(OpCodes.Stloc, argumentValue);
                generator.Emit(OpCodes.Ldloc, argumentValue);
                generator.Emit(OpCodes.Ldloc, instance);
                generator.Emit(OpCodes.Callvirt, property.GetMethod);
                if (property.PropertyType.GetTypeInfo().IsValueType)
                {
                    generator.Emit(OpCodes.Box, property.PropertyType);
                }
                generator.Emit(OpCodes.Stfld, ArgumentValueField);
                generator.Emit(OpCodes.Ldloc, argumentValue);
                //Load the property type array
                generator.Emit(OpCodes.Ldarg_2);
                generator.EmitFastInt(index);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.Emit(OpCodes.Stfld, ArgumentTypeField);

                //Load the dictionary
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Ldloc, argumentValue);
                generator.Emit(OpCodes.Callvirt, DictionaryAddMethod);                              
            }

            generator.Emit(OpCodes.Ret);

            

            var method = 
                (Action<object, Dictionary<string, ArgumentValue>, Type[]>)
                    dm.CreateDelegate(typeof(Action<object, Dictionary<string, ArgumentValue>, Type[]>));
            return (arguments, map) => method(arguments, map, propertyTypes);
        }    
    }


    public class ArgumentValue
    {
        public object Value;
        public Type Type;
    }
}