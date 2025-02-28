using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DbReader.DynamicArguments;
using DbReader.Tracking;

namespace DbReader
{
    /// <summary>
    /// Defines and creates new dynamic/anonymous types at runtime.
    /// </summary>
    public class ArgumentsBuilder
    {
        private readonly List<DynamicMemberInfo> dynamicMembers = new List<DynamicMemberInfo>();

        private static ConcurrentDictionary<DynamicMemberInfo[], Type> cache = new ConcurrentDictionary<DynamicMemberInfo[], Type>(new DynamicMemberInfoArrayEqualityComparer());

        /// <summary>
        /// Adds public properties and public fields from the given <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The instance from which to add property and fields.</param>
        /// <typeparam name="TSource">The type source object.</typeparam>
        /// <returns><see cref="ArgumentsBuilder"/></returns>
        public ArgumentsBuilder From<TSource>(TSource source)
        {
            var extractor = new DynamicValueExtractor();
            var extractedDynamicMembers = extractor.GetDynamicMembers(source);
            dynamicMembers.AddRange(extractedDynamicMembers);
            return this;
        }

        /// <summary>
        /// Adds a property with a given <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value for the property.</param>
        /// <typeparam name="T">The property type.</typeparam>
        /// <returns><see cref="ArgumentsBuilder"/></returns>
        public ArgumentsBuilder Add<T>(string name, T value)
        {
            dynamicMembers.Add(new DynamicMemberInfo<T>(name, value));
            return this;
        }

        /// <summary>
        /// Builds and created an instance of the dynamic type.
        /// </summary>
        /// <returns>An <see cref="IDynamicType"/> representing the dynamic type instance.</returns>
        public IDynamicType Build()
        {
            var type = cache.GetOrAdd(dynamicMembers.ToArray(), args => CreateType());
            var activator = new DynamicTypeActivator();
            var instance = activator.Activate(type, dynamicMembers.ToArray());
            return instance;
        }

        private Type CreateType()
        {
            var typeBuilder = GetTypeBuilder();
            ImplementMembers(typeBuilder);
            ImplementDynamicTypeInterface(typeBuilder);
            var type = typeBuilder.CreateTypeInfo();
            return type;
        }

        private TypeBuilder GetTypeBuilder()
        {
            AssemblyName dynamicAssemblyName = new AssemblyName("DynamicTypeAssembly");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("DynamicTypeModule");
            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType("DynamicType", TypeAttributes.Public);
            return dynamicAnonymousType;
        }

        private FieldBuilder[] ImplementMembers(TypeBuilder typeBuilder)
        {
            var fields = new List<FieldBuilder>();
            var members = dynamicMembers.ToArray();
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, members.Select(a => a.Type).ToArray());
            var constructorGenerator = constructorBuilder.GetILGenerator();
            for (int i = 0; i < members.Length; i++)
            {
                DynamicMemberInfo argumentInfo = members[i];
                var fieldBuilder = typeBuilder.DefineField($"_{argumentInfo.Name}", argumentInfo.Type, FieldAttributes.Public);
                var argumentBuilder = constructorBuilder.DefineParameter(
                    i + 1,
                    ParameterAttributes.None,
                    $"{argumentInfo.Name.Substring(0, 1)}{argumentInfo.Name.Substring(1)}");
                constructorGenerator.Emit(OpCodes.Ldarg, 0);
                constructorGenerator.Emit(OpCodes.Ldarg, i + 1);
                constructorGenerator.Emit(OpCodes.Stfld, fieldBuilder);

                var propertyBuilder = typeBuilder.DefineProperty(argumentInfo.Name, PropertyAttributes.None, argumentInfo.Type, Type.EmptyTypes);
                var getMethodBuilder = typeBuilder.DefineMethod($"get_{argumentInfo.Name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, argumentInfo.Type, Type.EmptyTypes);
                var getMethodGenerator = getMethodBuilder.GetILGenerator();
                getMethodGenerator.Emit(OpCodes.Ldarg_0);
                getMethodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                getMethodGenerator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(getMethodBuilder);
                fields.Add(fieldBuilder);
            }

            constructorGenerator.Emit(OpCodes.Ret);

            return fields.ToArray(); ;
        }

        private static void ImplementDynamicTypeInterface(TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(typeof(IDynamicType));

            var openGenericGetMethod = typeof(IDynamicType).GetMethod(nameof(IDynamicType.Get));
            var closedValueAccessorType = typeof(ValueAccessor<>).MakeGenericType(typeBuilder);

            var openGenericGetValueMethod = typeof(ValueAccessor<>).GetMethod("GetValue");
            var closedGenericGetValueMethod = TypeBuilder.GetMethod(closedValueAccessorType, openGenericGetValueMethod);

            var methodAttributes = openGenericGetMethod.Attributes ^ MethodAttributes.Abstract;

            var methodBuilder = typeBuilder.DefineMethod(openGenericGetMethod.Name, methodAttributes, openGenericGetMethod.ReturnType, new[] { typeof(string) });
            var genericArgumentName = openGenericGetMethod.GetGenericArguments()[0].Name;



            var genericTypeParameterBuilder = methodBuilder.DefineGenericParameters(genericArgumentName)[0];


            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, closedGenericGetValueMethod);
            generator.Emit(OpCodes.Ret);

        }
    }
}
