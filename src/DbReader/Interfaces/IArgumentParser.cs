namespace DbReader.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Construction;
    using Selectors;

    public interface IArgumentParser
    {
        IReadOnlyDictionary<string, object> Parse(object value);
    }

    public class ArgumentParser : IArgumentParser
    {
        private readonly IPropertySelector readablePropertySelector;
        private readonly IMethodSkeletonFactory methodSkeletonFactory;

        public ArgumentParser(IPropertySelector readablePropertySelector, IMethodSkeletonFactory methodSkeletonFactory)
        {
            this.readablePropertySelector = readablePropertySelector;
            this.methodSkeletonFactory = methodSkeletonFactory;
        }

        public IReadOnlyDictionary<string, object> Parse(object value)
        {
            if (value == null)
            {
                return new Dictionary<string, object>();
            }

            Dictionary<string, object> map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            CreateParseMethod(value.GetType())(value, map);

            var properties = readablePropertySelector.Execute(value.GetType());
            return properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(value) ?? DBNull.Value, StringComparer.OrdinalIgnoreCase);
        }

        private Action<object, Dictionary<string, object>> CreateParseMethod(Type argumentsType)
        {
            var properties = readablePropertySelector.Execute(argumentsType);
            var methodSkeleton = methodSkeletonFactory.GetMethodSkeleton("ParseArguments", typeof (void),
                new[] {typeof (object), typeof (Dictionary<string, object>)});
            DynamicMethod dm = new DynamicMethod("ParseArguments", typeof (void),
                new[] {typeof (object), typeof (Dictionary<string, object>)}, argumentsType);

            var generator = dm.GetILGenerator();
            LocalBuilder instance = generator.DeclareLocal(argumentsType);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass,argumentsType);
            generator.Emit(OpCodes.Stloc, instance);
            foreach (var property in properties)
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Ldloc, instance);                
                generator.Emit(OpCodes.Callvirt, property.GetMethod);
                MethodInfo addmethod = typeof (Dictionary<string, object>).GetTypeInfo().GetDeclaredMethod("Add");
                generator.Emit(OpCodes.Callvirt, addmethod);                                
            }

            generator.Emit(OpCodes.Ret);

            return
                (Action<object, Dictionary<string, object>>)
                    dm.CreateDelegate(typeof (Action<object, Dictionary<string, object>>));
        } 
    }
}