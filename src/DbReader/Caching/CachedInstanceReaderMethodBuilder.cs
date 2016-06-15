namespace DbReader.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Construction;
    using DbReader.Interfaces;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder;

                       
        public CachedInstanceReaderMethodBuilder(IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;        
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {            
            return SimpleCache<Func<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
                () => instanceReaderMethodBuilder.CreateMethod(dataRecord, prefix));            
        }
    }
}