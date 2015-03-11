namespace DbReader.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbReader.Interfaces;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder;

        private readonly ICacheKeyFactory cacheKeyFactory;

        private readonly Dictionary<string, Func<IDataRecord, T>> cache = new Dictionary<string, Func<IDataRecord, T>>(); 

        //private readonly ConcurrentDictionary<string, Func<IDataRecord, T>> cache =
        //    new ConcurrentDictionary<string, Func<IDataRecord, T>>();

        public CachedInstanceReaderMethodBuilder(IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder, ICacheKeyFactory cacheKeyFactory)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;
            this.cacheKeyFactory = cacheKeyFactory;
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var key = cacheKeyFactory.CreateKey(typeof(T), dataRecord, prefix);
            Func<IDataRecord, T> methodDelegate;
            if (!cache.TryGetValue(key, out methodDelegate))
            {
                methodDelegate = instanceReaderMethodBuilder.CreateMethod(dataRecord, prefix);
                cache.Add(key, methodDelegate);
            }

            return methodDelegate;            
        }
    }
}