namespace DbReader
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    using DbReader.Interfaces;

    public class CachedOneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {
        private readonly ICacheKeyFactory cacheKeyFactory;

        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;        

        private ConcurrentDictionary<string, Action<IDataRecord, T>> cache =
            new ConcurrentDictionary<string, Action<IDataRecord, T>>();


        public CachedOneToManyMethodBuilder(IOneToManyMethodBuilder<T> oneToManyMethodBuilder, ICacheKeyFactory cacheKeyFactory)
        {
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;
            this.cacheKeyFactory = cacheKeyFactory;
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var key = cacheKeyFactory.CreateKey(typeof(T), dataRecord, prefix);
            return cache.GetOrAdd(key, tuple => oneToManyMethodBuilder.CreateMethod(dataRecord, prefix));
        }
    }
}