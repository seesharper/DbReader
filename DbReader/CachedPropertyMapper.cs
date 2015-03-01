namespace DbReader
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
        private readonly IPropertyMapper propertyMapper;

        private readonly ConcurrentDictionary<Tuple<Type, string>, MappingInfo[]> cache
            = new ConcurrentDictionary<Tuple<Type, string>, MappingInfo[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertyMapper"/> class.
        /// </summary>
        /// <param name="propertyMapper">
        /// The <see cref="IPropertyMapper"/> being decorated.
        /// </param>
        public CachedPropertyMapper(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
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
            return cache.GetOrAdd(Tuple.Create(type, prefix), t => propertyMapper.Execute(t.Item1, dataRecord, t.Item2));
        }
    }
}