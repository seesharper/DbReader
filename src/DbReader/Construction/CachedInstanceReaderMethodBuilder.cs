namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Readers;
    using Interfaces;

    /// <summary>
    /// An <see cref="IInstanceReaderMethodBuilder{T}"/> decorator that is used
    /// to cache the dynamic method created at runtime to read an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInstanceReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="instanceReaderMethodBuilder">The target <see cref="IInstanceReaderMethodBuilder{T}"/>.</param>
        public CachedInstanceReaderMethodBuilder(Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;
        }

        /// <summary>
        /// Creates a method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the data for the instance.</param>
        /// <param name="prefix">The current prefix.</param>
        /// <returns>A method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.</returns>
        public Func<IDataRecord, IGenericInstanceReaderFactory, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return StaticCache<Func<IDataRecord, IGenericInstanceReaderFactory, T>>.GetOrAdd(typeof(T), prefix,
                () => instanceReaderMethodBuilder.Value.CreateMethod(dataRecord, prefix));
        }
    }
}