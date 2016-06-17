namespace DbReader.Construction
{
    using System;
    using System.Data;

    /// <summary>
    /// An <see cref="IOneToManyMethodBuilder{T}"/> decorator that 
    /// caches the dynamically created method used to populate collection
    /// properties of a given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedOneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {      
        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOneToManyMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="oneToManyMethodBuilder">The target <see cref="IOneToManyMethodBuilder{T}"/>.</param>
        public CachedOneToManyMethodBuilder(IOneToManyMethodBuilder<T> oneToManyMethodBuilder)
        {
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;       
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return Cache<Action<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
                () => oneToManyMethodBuilder.CreateMethod(dataRecord, prefix));
        }
    }
}