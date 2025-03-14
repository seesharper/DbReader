namespace DbClient.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;
    using Interfaces;
    using Readers;
    using Selectors;

    /// <summary>
    /// A class that dynamically creates a method used to
    /// populate "many-to-one" properties of a given type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> for which to create the dynamic method.</typeparam>
    public class ManyToOneMethodBuilder<T> : IManyToOneMethodBuilder<T>
    {
        private readonly IMethodSkeletonFactory methodSkeletonFactory;
        private readonly IPropertySelector manyToOnePropertySelector;
        private readonly IPrefixResolver prefixResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManyToOneMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for providing an <see cref="IMethodSkeleton"/> instance.</param>
        /// <param name="manyToOnePropertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting properties that represents a "many-to-one" relationship.</param>
        /// <param name="prefixResolver">The <see cref="IPrefixResolver"/> that is responsible for resolving the prefix for each "many-to-one" property.</param>
        public ManyToOneMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IPropertySelector manyToOnePropertySelector, IPrefixResolver prefixResolver)
        {
            this.methodSkeletonFactory = methodSkeletonFactory;
            this.manyToOnePropertySelector = manyToOnePropertySelector;
            this.prefixResolver = prefixResolver;
        }

        /// <summary>
        ///  Creates a dynamic method that populates mapped "many-to-one" properties.
        /// </summary>
        /// <param name="dataRecord">The source <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The property prefix used to identify the fields in the <see cref="IDataRecord"/>.</param>
        /// <returns>A delegate representing a dynamic method that populates mapped "many-to-one" properties.</returns>
        public Action<T, IDataRecord, IInstanceReaderFactory> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var properties = manyToOnePropertySelector.Execute(typeof(T));
            if (properties.Length == 0)
            {
                return null;
            }
            var instanceReaders = new List<object>(properties.Length);
            var methodSkeleton = methodSkeletonFactory.GetMethodSkeleton("ManyToOneDynamicMethod", typeof(void), new[] { typeof(T), typeof(IDataRecord), typeof(IInstanceReaderFactory) });
            var generator = methodSkeleton.GetGenerator();

            bool shouldCreateMethod = false;

            foreach (var property in properties)
            {
                string propertyPrefix = prefixResolver.GetPrefix(property, dataRecord, prefix);
                if (propertyPrefix != null)
                {
                    shouldCreateMethod = true;
                    Type instanceReaderType = typeof(IInstanceReader<>).MakeGenericType(property.PropertyType);

                    generator.Emit(OpCodes.Ldarg_0);


                    // Push the instance readers
                    generator.Emit(OpCodes.Ldarg_2);
                    var closedGenericGetInstanceReaderMethod = typeof(IInstanceReaderFactory).GetMethod(nameof(IInstanceReaderFactory.GetInstanceReader)).MakeGenericMethod(property.PropertyType);
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);
                    generator.Emit(OpCodes.Callvirt, closedGenericGetInstanceReaderMethod);

                    MethodInfo readMethod = instanceReaderType.GetMethod("Read");

                    // Push the datarecord
                    generator.Emit(OpCodes.Ldarg_1);

                    //Push the prefix
                    generator.Emit(OpCodes.Ldstr, propertyPrefix);

                    //Call the read method.
                    generator.Emit(OpCodes.Callvirt, readMethod);
                    generator.Emit(OpCodes.Callvirt, property.GetSetMethod());
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