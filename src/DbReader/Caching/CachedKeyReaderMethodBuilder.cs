namespace DbReader.Caching
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    using DbReader.Interfaces;

    public class CachedKeyReaderMethodBuilder : IKeyReaderMethodBuilder
    {
        private readonly IKeyReaderMethodBuilder keyReaderMethodBuilder;
        private readonly ICacheKeyFactory cacheKeyFactory;
        private readonly ConcurrentDictionary<string, Func<IDataRecord, IStructuralEquatable>> cache =
            new ConcurrentDictionary<string, Func<IDataRecord, IStructuralEquatable>>();
        
        public CachedKeyReaderMethodBuilder(IKeyReaderMethodBuilder keyReaderMethodBuilder, ICacheKeyFactory cacheKeyFactory)
        {
            this.keyReaderMethodBuilder = keyReaderMethodBuilder;
            this.cacheKeyFactory = cacheKeyFactory;
        }

        public Func<IDataRecord, IStructuralEquatable> CreateMethod(Type type, IDataRecord dataRecord, string prefix)
        {
            string key = cacheKeyFactory.CreateKey(type, dataRecord, prefix);
            return cache.GetOrAdd(key, s => keyReaderMethodBuilder.CreateMethod(type, dataRecord, prefix));
        }
    }
}