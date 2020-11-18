namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Readers;
    using Interfaces;
    using Selectors;

    /// <summary>
    /// A class that creates an instance of <typeparamref name="T"/>
    /// based on a given <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IReaderMethodBuilder<T> propertyReaderMethodBuilder;
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IManyToOneMethodBuilder<T> manyToOneMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="propertyReaderMethodBuilder">The <see cref="IReaderMethodBuilder{T}"/> that is responsible for creating a dynamic method
        /// that populates an instance of <typeparamref name="T"/> from an <see cref="IDataRecord"/>.</param>
        /// <param name="ordinalSelector">The <see cref="IOrdinalSelector"/> that is responsible for providing the ordinals used to map a type from a <see cref="IDataRecord"/>.</param>
        /// <param name="manyToOneMethodBuilder">The <see cref="IManyToOneMethodBuilder{T}"/> that is responsible for creating a dynamic method
        /// that populates "many-to-one" properties of a given type.</param>
        public InstanceReaderMethodBuilder(IReaderMethodBuilder<T> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector, IManyToOneMethodBuilder<T> manyToOneMethodBuilder)
        {
            this.propertyReaderMethodBuilder = propertyReaderMethodBuilder;
            this.ordinalSelector = ordinalSelector;
            this.manyToOneMethodBuilder = manyToOneMethodBuilder;
        }

        /// <summary>
        /// Creates a method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the data for the instance.</param>
        /// <param name="prefix">The current prefix.</param>
        /// <returns>A method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.</returns>
        public Func<IDataRecord, IInstanceReaderFactory, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            int[] ordinals = ordinalSelector.Execute(typeof(T), dataRecord, prefix);
            Func<IDataRecord, int[], T> propertyReaderMethod = propertyReaderMethodBuilder.CreateMethod();

            Action<T, IDataRecord, IInstanceReaderFactory> manyToOneMethod = manyToOneMethodBuilder.CreateMethod(dataRecord, prefix);

            if (manyToOneMethod != null)
            {
                return (record, instanceReaderFactory) =>
                    {
                        var instance = propertyReaderMethod(record, ordinals);
                        manyToOneMethod(instance, record, instanceReaderFactory);
                        return instance;
                    };
            }

            return (record, instanceReaderFactory) => propertyReaderMethod(record, ordinals);
        }
    }
}