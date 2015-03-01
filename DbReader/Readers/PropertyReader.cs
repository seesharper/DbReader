//namespace DbReader
//{
//    using System;
//    using System.Data;

//    using DbReader.Readers;

//    public class PropertyReader<T> : IReader<T>
//    {
//        private readonly IReaderMethodBuilder<T> propertyReaderMethodBuilder;

//        private readonly IOrdinalSelector ordinalSelector;
                
//        public PropertyReader(IReaderMethodBuilder<T> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector)
//        {
//            this.propertyReaderMethodBuilder = propertyReaderMethodBuilder;
//            this.ordinalSelector = ordinalSelector;
//        }

//        public T Read(IDataRecord dataRecord, int[] ordinals)
//        {
//            var readMethod = propertyReaderMethodBuilder.CreateMethod();


//        }
//    }
//}