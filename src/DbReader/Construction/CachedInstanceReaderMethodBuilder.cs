namespace DbReader.Construction
{
    using System;
    using System.Data;
    using Interfaces;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly Func<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilderFactory;

                       
        public CachedInstanceReaderMethodBuilder(Func<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilderFactory)
        {
            this.instanceReaderMethodBuilderFactory = instanceReaderMethodBuilderFactory;        
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {            
            return Cache<Func<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
                () => instanceReaderMethodBuilderFactory().CreateMethod(dataRecord, prefix));            
        }
    }
}