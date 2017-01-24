namespace DbReader.Construction
{
    using System;
    using System.Data;

    /// <summary>
    /// An <see cref="IReaderMethodBuilder{T}"/> decorator that caches the 
    /// dynamic method created at runtime used to create and pupulate an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedReaderMethodBuilder<T> : IReaderMethodBuilder<T>
    {
        
        private readonly Lazy<Func<IDataRecord, int[], T>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="readerMethodBuilder">The target <see cref="IReaderMethodBuilder{T}"/>.</param>
        public CachedReaderMethodBuilder(IReaderMethodBuilder<T> readerMethodBuilder)
        {
            cache = new Lazy<Func<IDataRecord, int[], T>>(readerMethodBuilder.CreateMethod);        
        }

        /// <summary>
        /// Creates a new method that initializes and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.
        /// </summary>
        /// <returns>A delegate that creates and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.</returns>
        public Func<IDataRecord, int[], T> CreateMethod()
        {
            return cache.Value;
        }
    }
}