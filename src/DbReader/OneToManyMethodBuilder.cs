namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using DbReader.Interfaces;

    public class OneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private readonly IPropertySelector oneToManyPropertySelector;
        private readonly Func<Type, object> instanceReaderFactory;
        private readonly IPrefixResolver prefixResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory"></param>
        /// <param name="oneToManyPropertySelector"></param>
        /// <param name="instanceReaderFactory"></param>
        /// <param name="prefixResolver"></param>
        public OneToManyMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IPropertySelector oneToManyPropertySelector, Func<Type, object> instanceReaderFactory, IPrefixResolver prefixResolver)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
            this.oneToManyPropertySelector = oneToManyPropertySelector;
            this.instanceReaderFactory = instanceReaderFactory;
            this.prefixResolver = prefixResolver;
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            PropertyInfo[] properties = oneToManyPropertySelector.Execute(typeof(T));
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
                    var elementType = property.PropertyType.GetProjectionType();
                    MethodInfo tryAddMethod = CollectionExtensions.GetTryAddMethod(elementType);
                    Type instanceReaderType = typeof(IInstanceReader<>).MakeGenericType(elementType);
                    MethodInfo readMethod = instanceReaderType.GetMethod("Read");

                    instanceReaders.Add(instanceReaderFactory(instanceReaderType));
                    int instanceReaderIndex = instanceReaders.Count - 1;

                    MethodInfo getMethod = property.GetGetMethod();

                    // Push the instance onto the stack. 
                    generator.Emit(OpCodes.Ldarg_0);     
                                                       
                    // Call the property getter and push the result onto the stack.
                    generator.Emit(OpCodes.Callvirt, getMethod);

                    // Push the object reader.
                    generator.Emit(OpCodes.Ldarg_2);
                    generator.EmitFastInt(instanceReaderIndex);
                    generator.Emit(OpCodes.Ldelem_Ref);
                    generator.Emit(OpCodes.Castclass, instanceReaderType);

                    // Push the datarecord
                    generator.Emit(OpCodes.Ldarg_1);

                    // Push the prefix
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);
                    
                    // Call the read method.
                    generator.Emit(OpCodes.Callvirt, readMethod);

                    generator.Emit(OpCodes.Call, tryAddMethod);                                                            
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