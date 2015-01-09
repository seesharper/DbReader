namespace DbReader
{
    using System;
    using System.Data;

    public class PropertyReader<T> : IReader<T> where T : new()
    {
        private readonly IReaderMethodBuilder<T> readerMethodBuilder;

        private readonly Func<IDataRecord, int[], T> readMethod;

        public PropertyReader(IReaderMethodBuilder<T> readerMethodBuilder)
        {
            this.readerMethodBuilder = readerMethodBuilder;
            readMethod = readerMethodBuilder.CreateMethod();
        }

        public T Read(IDataRecord dataRecord, int[] ordinals)
        {
            return readMethod(dataRecord, ordinals);
        }
    }
}