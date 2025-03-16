namespace DbClient.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;
    using Readers;
    using Selectors;

    /// <summary>
    /// A class that dynamically creates a method used to
    /// populate collection properties of a given type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> for which to create the dynamic method.</typeparam>
    public class OneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private readonly IPropertySelector oneToManyPropertySelector;
        private readonly IPrefixResolver prefixResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for providing an <see cref="IMethodSkeleton"/> instance.</param>
        /// <param name="oneToManyPropertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting properties that represents a "one-to-many" relationship.</param>
        /// <param name="prefixResolver">The <see cref="IPrefixResolver"/> that is responsible for resolving the prefix for each "one-to-many" property.</param>
        public OneToManyMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IPropertySelector oneToManyPropertySelector, IPrefixResolver prefixResolver)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
            this.oneToManyPropertySelector = oneToManyPropertySelector;
            this.prefixResolver = prefixResolver;
        }

        /// <summary>
        /// Creates a dynamic method that populates mapped collection properties.
        /// </summary>
        /// <param name="dataRecord">The source <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The property prefix used to identify the fields in the <see cref="IDataRecord"/>.</param>
        /// <returns>A delegate representing a dynamic method that populates mapped collection properties.</returns>
        public Action<T, IDataRecord, IInstanceReaderFactory> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            PropertyInfo[] properties = oneToManyPropertySelector.Execute(typeof(T));
            if (properties.Length == 0)
            {
                return null;
            }
            var instanceReaders = new List<object>(properties.Length);
            var methodSkeleton = methodSkeletonFactory.GetMethodSkeleton("OneToManyDynamicMethod", typeof(void), new[] { typeof(T), typeof(IDataRecord), typeof(IInstanceReaderFactory) });
            var generator = methodSkeleton.GetGenerator();
            bool shouldCreateMethod = false;
            foreach (var property in properties)
            {
                string propertyPrefix = prefixResolver.GetPrefix(property, dataRecord, prefix);
                if (propertyPrefix != null)
                {

                    shouldCreateMethod = true;
                    var elementType = property.PropertyType.GetProjectionType();
                    MethodInfo tryAddMethod = Extensions.CollectionExtensions.GetTryAddMethod(elementType);
                    Type instanceReaderType = typeof(IInstanceReader<>).MakeGenericType(elementType);
                    MethodInfo readMethod = instanceReaderType.GetMethod("Read");

                    MethodInfo getMethod = property.GetGetMethod();

                    // Push the instance onto the stack.
                    generator.Emit(OpCodes.Ldarg_0);

                    // Call the property getter and push the result onto the stack.
                    generator.Emit(OpCodes.Callvirt, getMethod);

                    // Push the instancereader factory
                    generator.Emit(OpCodes.Ldarg_2);
                    var closedGenericGetInstanceReaderMethod = typeof(IInstanceReaderFactory).GetMethod(nameof(IInstanceReaderFactory.GetInstanceReader)).MakeGenericMethod(elementType);
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);
                    generator.Emit(OpCodes.Callvirt, closedGenericGetInstanceReaderMethod);

                    // Push the datarecord
                    generator.Emit(OpCodes.Ldarg_1);

                    // Push the prefix
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);

                    // Call the read method.
                    generator.Emit(OpCodes.Callvirt, readMethod);

                    generator.Emit(OpCodes.Call, tryAddMethod);
                }

            }

            if (shouldCreateMethod)
            {
                generator.Emit(OpCodes.Ret);
                var method = (Action<T, IDataRecord, IInstanceReaderFactory>)methodSkeleton.CreateDelegate(typeof(Action<T, IDataRecord, IInstanceReaderFactory>));
                return method;
            }

            return null;

        }
    }
}