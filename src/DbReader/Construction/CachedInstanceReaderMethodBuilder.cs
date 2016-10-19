namespace DbReader.Construction
{
    using System;
    using System.Data;
    using Interfaces;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder;

                       
        public CachedInstanceReaderMethodBuilder(IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;        
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {            
            return Cache<Func<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
                () => instanceReaderMethodBuilder.CreateMethod(dataRecord, prefix));            
        }
    }
}