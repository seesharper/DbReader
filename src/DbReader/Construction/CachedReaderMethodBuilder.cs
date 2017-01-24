namespace DbReader.Construction
{
    using System;
    using System.Data;

    public class CachedReaderMethodBuilder<T> : IReaderMethodBuilder<T>
    {
        
        private Lazy<Func<IDataRecord, int[], T>> cache;

        public CachedReaderMethodBuilder(IReaderMethodBuilder<T> readerMethodBuilder)
        {
            cache = new Lazy<Func<IDataRecord, int[], T>>(readerMethodBuilder.CreateMethod);        
        }

        public Func<IDataRecord, int[], T> CreateMethod()
        {
            return cache.Value;
        }
    }
}