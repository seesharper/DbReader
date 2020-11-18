namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Readers;

    /// <summary>
    /// An <see cref="IOneToManyMethodBuilder{T}"/> decorator that
    /// caches the dynamically created method used to populate collection
    /// properties of a given type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> for which to create the dynamic method.</typeparam>
    public class CachedOneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {
        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOneToManyMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="oneToManyMethodBuilder">The target <see cref="IOneToManyMethodBuilder{T}"/>.</param>
        public CachedOneToManyMethodBuilder(IOneToManyMethodBuilder<T> oneToManyMethodBuilder)
        {
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;
        }

        /// <summary>
        /// Creates a dynamic method that populates mapped collection properties.
        /// </summary>
        /// <param name="dataRecord">The source <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The property prefix used to identify the fields in the <see cref="IDataRecord"/>.</param>
        /// <returns>A delegate representing a dynamic method that populates mapped collection properties.</returns>
        public Action<IDataRecord, T, IGenericInstanceReaderFactory> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return StaticCache<Action<IDataRecord, T, IGenericInstanceReaderFactory>>.GetOrAdd(typeof(T), prefix,
                () => oneToManyMethodBuilder.CreateMethod(dataRecord, prefix));
        }
    }
}