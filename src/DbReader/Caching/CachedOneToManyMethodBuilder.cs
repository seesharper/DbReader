namespace DbReader.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using Construction;
    using DbReader.Interfaces;

    public class CachedOneToManyMethodBuilder<T> : IOneToManyMethodBuilder<T>
    {      
        private readonly IOneToManyMethodBuilder<T> oneToManyMethodBuilder;        
        

        public CachedOneToManyMethodBuilder(IOneToManyMethodBuilder<T> oneToManyMethodBuilder)
        {
            this.oneToManyMethodBuilder = oneToManyMethodBuilder;
       
        }

        public Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix)
        {
            return SimpleCache<Action<IDataRecord, T>>.GetOrAdd(typeof (T), prefix,
                () => oneToManyMethodBuilder.CreateMethod(dataRecord, prefix));
        }
    }
}