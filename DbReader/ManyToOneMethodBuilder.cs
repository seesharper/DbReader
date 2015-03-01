namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using DbReader.Interfaces;

    public class ManyToOneMethodBuilder<T> : IManyToOneMethodBuilder<T>
    {
        private readonly IMethodSkeletonFactory methodSkeletonFactory;

        private readonly IPropertySelector manyToOnePropertySelector;

        private readonly Func<Type, object> instanceReaderFactory;

        private readonly IPrefixResolver prefixResolver;

        public ManyToOneMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IPropertySelector manyToOnePropertySelector, Func<Type, object> instanceReaderFactory, IPrefixResolver prefixResolver)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
            this.manyToOnePropertySelector = manyToOnePropertySelector;
            this.instanceReaderFactory = instanceReaderFactory;
            this.prefixResolver = prefixResolver;
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var properties = manyToOnePropertySelector.Execute(typeof(T));
            if (properties.Length == 0)
            {
                return null;
            }
            var instanceReaders = new List<object>(properties.Length);
            var methodSkeleton = methodSkeletonFactory.GetMethodSkeleton(typeof(void), new[] { typeof(T), typeof(IDataRecord), typeof(object[]) });
            var generator = methodSkeleton.GetGenerator();

            bool shouldCreateMethod = false;

            foreach (var property in properties)
            {
                string propertyPrefix = prefixResolver.GetPrefix(property, dataRecord, prefix);
                if (propertyPrefix != null)
                {
                    shouldCreateMethod = true;
                    Type instanceReaderType = typeof(IInstanceReader<>).MakeGenericType(property.PropertyType);
                    object instanceReader = instanceReaderFactory(instanceReaderType);
                    
                    instanceReaders.Add(instanceReader);
                    
                    generator.Emit(OpCodes.Ldarg_0);
                    

                    // Push the object reader.
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.EmitFastInt(instanceReaders.Count - 1);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Castclass, instanceReaderType);

                    MethodInfo readMethod = instanceReaderType.GetMethod("Read");

                    // Push the datarecord
                    generator.Emit(OpCodes.Ldarg_1);

                    //Push the prefix
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);

                    //Call the read method.
                    generator.Emit(OpCodes.Callvirt, readMethod);
                    generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
                }

                generator.Emit(OpCodes.Ret);
            }
            if (shouldCreateMethod)
            {
                var method = (Action<T, IDataRecord, object[]>)methodSkeleton.CreateDelegate(typeof(Action<T, IDataRecord, object[]>));

                return (record, instance) => method(instance, record, instanceReaders.ToArray());    
            }

            return null;
        }

    }
} 