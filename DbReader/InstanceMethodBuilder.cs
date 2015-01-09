namespace DbReader
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Threading.Tasks;

    public class InstanceMethodBuilder<T> : IInstanceMethodBuilder<T>
    {
        private readonly IReaderMethodBuilder<T> propertyReaderMethodBuilder;
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IRelationMethodBuilder<T> manyToOneMethodBuilder;

        public InstanceMethodBuilder(IReaderMethodBuilder<T> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector, IRelationMethodBuilder<T> manyToOneMethodBuilder)
        {
            this.propertyReaderMethodBuilder = propertyReaderMethodBuilder;
            this.ordinalSelector = ordinalSelector;
            this.manyToOneMethodBuilder = manyToOneMethodBuilder;
        }

        public Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            int[] ordinals = ordinalSelector.Execute(typeof(T), dataRecord, prefix);
            Func<IDataRecord, int[], T> propertyReaderMethod = propertyReaderMethodBuilder.CreateMethod();

            Action<IDataRecord, T> manyToOneMethod = manyToOneMethodBuilder.CreateMethod(dataRecord, prefix);
           
            if (manyToOneMethod == null)
            {
                return record => propertyReaderMethod(record, ordinals);
            }

            return record =>
                {
                    var instance = propertyReaderMethod(record, ordinals);
                    manyToOneMethod(record, instance);
                    return instance;
                };
        }
    }
}