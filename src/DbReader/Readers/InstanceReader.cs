namespace DbReader.Readers
{
    using System.Data;
    using Interfaces;

    /// <summary>
    /// A class that is capable of transforming an <see cref="IDataRecord"/> into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be read from the <see cref="IDataRecord"/>.</typeparam>
    public class InstanceReader<T> : IInstanceReader<T>
    {
        private readonly IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceReader{T}"/> class.
        /// </summary>
        /// <param name="instanceReaderMethodBuilder">The <see cref="IInstanceReaderMethodBuilder{T}"/> that is responsible 
        /// for building a method that is capable of reading an instance of <typeparamref name="T"/> from an <see cref="IDataRecord"/>.</param>
        public InstanceReader(IInstanceReaderMethodBuilder<T> instanceReaderMethodBuilder)
        {
            this.instanceReaderMethodBuilder = instanceReaderMethodBuilder;
        }

        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> from which to read an instance of <typeparamref name="T"/>.</param>
        /// <param name="currentPrefix">The current prefix.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public T Read(IDataRecord dataRecord, string currentPrefix)
        {                        
            var method = instanceReaderMethodBuilder.CreateMethod(dataRecord, currentPrefix);
            return method(dataRecord);
        }
    }
}