namespace DbReader
{
    using System;
    using System.Data;

    using DbReader.Interfaces;

    public class InstanceReaderMethodBuilder<T> : IInstanceReaderMethodBuilder<T>
    {
        private readonly IReaderMethodBuilder<T> propertyReaderMethodBuilder;
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IManyToOneMethodBuilder<T> manyToOneMethodBuilder;
        
        public InstanceReaderMethodBuilder(IReaderMethodBuilder<T> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector, IManyToOneMethodBuilder<T> manyToOneMethodBuilder)
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

            if (manyToOneMethod != null)
            {
                return record =>
                    {
                        var instance = propertyReaderMethod(record, ordinals);
                        if (manyToOneMethod != null)
                        {
                            manyToOneMethod(dataRecord, instance);
                        }
                        return instance;
                    };
            }

            return record => propertyReaderMethod(record, ordinals);
        }
    }
}