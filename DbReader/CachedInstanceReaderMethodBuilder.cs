namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    using DbReader.Interfaces;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder;

        private readonly ICacheKeyFactory cacheKeyFactory;

        private readonly ConcurrentDictionary<string, Func<IDataRecord, T>> cache =
            new ConcurrentDictionary<string, Func<IDataRecord, T>>();

        public CachedInstanceReaderMethodBuilder(IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder, ICacheKeyFactory cacheKeyFactory)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;
            this.cacheKeyFactory = cacheKeyFactory;
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var key = cacheKeyFactory.CreateKey(typeof(T), dataRecord, prefix);
            return cache.GetOrAdd(key, k => instanceReaderMethodBuilder.CreateMethod(dataRecord, prefix));
        }
    }
}