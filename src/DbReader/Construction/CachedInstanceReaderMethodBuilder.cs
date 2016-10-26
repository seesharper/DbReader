namespace DbReader.Construction
{
    using System;
    using System.Data;
    using Interfaces;
    using LightInject;

    public class CachedInstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder;        

        public CachedInstanceReaderMethodBuilder(Lazy<IInstanceReaderMethodBuilder<T>> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return StaticCache<Func<IDataRecord, T>>.GetOrAdd(typeof(T), prefix,
                () => instanceReaderMethodBuilder.Value.CreateMethod(dataRecord, prefix));
        }
    }
}