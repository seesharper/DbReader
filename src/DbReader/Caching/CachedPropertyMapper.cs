namespace DbReader.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    using DbReader.Interfaces;

    /// <summary>
    /// A decorator that caches a list of <see cref="MappingInfo"/> instances per <see cref="Type"/>.
    /// </summary>
    public class CachedPropertyMapper : IPropertyMapper
    {
        private readonly Func<IPropertyMapper> getPropertyMapper;

        private readonly ICacheKeyFactory cacheKeyFactory;

        private readonly ConcurrentDictionary<string, MappingInfo[]> cache
            = new ConcurrentDictionary<string, MappingInfo[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertyMapper"/> class.
        /// </summary>
        /// <param name="propertyMapper">
        /// The <see cref="IPropertyMapper"/> being decorated.
        /// </param>
        /// <param name="cacheKeyFactory"></param>
        public CachedPropertyMapper(Func<IPropertyMapper> getPropertyMapper, ICacheKeyFactory cacheKeyFactory)
        {
            this.getPropertyMapper = getPropertyMapper;
            this.cacheKeyFactory = cacheKeyFactory;
        }

        /// <summary>
        /// Maps the fields from the <paramref name="dataRecord"/> to the properties of the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the properties to be mapped.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the fields to be mapped.</param>
        /// <param name="prefix">The prefix to use when mapping columns to properties.</param>
        /// <returns>A list of <see cref="MappingInfo"/> instances that represents the mapping between a field and a property.</returns>
        public MappingInfo[] Execute(Type type, IDataRecord dataRecord, string prefix)
        {
            var key = cacheKeyFactory.CreateKey(type, dataRecord, prefix);
            return cache.GetOrAdd(key, k => getPropertyMapper().Execute(type, dataRecord, prefix));

        }
    }
}