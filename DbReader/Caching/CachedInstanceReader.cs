namespace DbReader.Caching
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;

    using DbReader.Interfaces;

    /// <summary>
    /// An <see cref="IInstanceReader{T}"/> decorator that caches instances 
    /// of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be cached.</typeparam>
    public class CachedInstanceReader<T> : IInstanceReader<T>
    {
        private readonly IInstanceReader<T> instanceReader;

        private readonly IKeyReader keyReader;

        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;

        private readonly ConcurrentDictionary<IStructuralEquatable, T> queryCache = new ConcurrentDictionary<IStructuralEquatable, T>();

        private readonly Dictionary<IStructuralEquatable, T> queryCache2 = new Dictionary<IStructuralEquatable, T>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInstanceReader{T}"/> class.
        /// </summary>
        /// <param name="instanceReader">The <see cref="IInstanceReader{T}"/> that is responsible
        /// for reading an instance of <typeparamref name="T"/> from an <see cref="IDataRecord"/>.</param>
        /// <param name="keyReader">An instance of <typeparamref name="T"/>.</param>
        /// <param name="oneToManyMethodBuilder"></param>
        public CachedInstanceReader(IInstanceReader<T> instanceReader, IKeyReader keyReader, IOneToManyMethodBuilder<T> oneToManyMethodBuilder)
        {
            this.instanceReader = instanceReader;
            this.keyReader = keyReader;
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;
        }

        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> from which to read an instance of <typeparamref name="T"/>.</param>
        /// <param name="currentPrefix">The current prefix.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public T Read(IDataRecord dataRecord, string currentPrefix)
        {
            var instance = ReadInstance(dataRecord, currentPrefix);
            var oneToManyMethod = oneToManyMethodBuilder.CreateMethod(dataRecord, currentPrefix);
            if (oneToManyMethod != null)
            {
                oneToManyMethod(dataRecord, instance);
            }
            
            return instance;
        }

        private T ReadInstance(IDataRecord dataRecord, string currentPrefix)
        {
            //return instanceReader.Read(dataRecord, currentPrefix);
            var key = keyReader.Read(typeof(T), dataRecord, currentPrefix);

            T instance;
            if (!queryCache2.TryGetValue(key, out instance))
            {
                instance = instanceReader.Read(dataRecord, currentPrefix);
                queryCache2.Add(key, instance);
            }
            return instance;

            //return queryCache.GetOrAdd(key, k => instanceReader.Read(dataRecord, currentPrefix));
        }
    }
}