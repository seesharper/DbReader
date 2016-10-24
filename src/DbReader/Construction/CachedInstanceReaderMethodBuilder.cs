namespace DbReader.Construction
{
    using System;
    using System.Data;
    using Interfaces;
    using LightInject;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder;
        private static ImmutableHashTree<FastCacheKey, Func<IDataRecord, T>> tree = ImmutableHashTree<FastCacheKey, Func<IDataRecord, T>>.Empty;


        public CachedInstanceReaderMethodBuilder(Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;        
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            var key = new FastCacheKey(typeof (T), prefix, SqlStatement.Current);
            var method = tree.Search(key);
            if (method == null)
            {
                method = instanceReaderMethodBuilder.Value.CreateMethod(dataRecord, prefix);
                tree = tree.Add(key, method);
            }
            return method;
            //return Cache<Func<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
            //    () => instanceReaderMethodBuilder.CreateMethod(dataRecord, prefix));            
        }
    }
}