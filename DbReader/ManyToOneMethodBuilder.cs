namespace DbReader
{
    using System;
    using System.Data;

    public class ManyToOneMethodBuilder<T> : IRelationMethodBuilder<T>
    {
        private readonly IPropertySelector manyToOnePropertySelector;

        private readonly Func<Type, object> instanceReaderFactory;

        private readonly IPrefixResolver prefixResolver;

        public ManyToOneMethodBuilder(IPropertySelector manyToOnePropertySelector, Func<Type, object> instanceReaderFactory, IPrefixResolver prefixResolver)
        {
            this.manyToOnePropertySelector = manyToOnePropertySelector;
            this.instanceReaderFactory = instanceReaderFactory;
            this.prefixResolver = prefixResolver;
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return null;
        }
    }
}