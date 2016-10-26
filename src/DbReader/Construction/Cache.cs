using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader.Construction
{
    using System.Data;
    using System.Threading;
    using LightInject;

    
    public class Cache<TKey, TValue>
    {
        private static ImmutableHashTree<TKey, TValue> tree = ImmutableHashTree<TKey, TValue>.Empty;

        public static TValue Get(TKey key)
        {            
            return tree.Search(key);
        }

        public static void Put(TKey key, TValue value)
        {           
            tree = tree.Add(key, value);
        }        
    }


    public sealed class PropertyReaderDelegates<T> : Cache<string, PropertyReaderDelegate<T>>
    {
        
    }



    public class PropertyReaderDelegate<T>
    {
        public Func<IDataRecord, int[], T> ReadMethod;
        public int[] Ordinals;        
    }

    



    public class FastCacheKey
    {       
        private readonly string prefix;
        private readonly string sql;

        public FastCacheKey(string prefix, string sql)
        {            
            this.prefix = prefix;
            this.sql = sql;
        }

        public override int GetHashCode()
        {
            return sql.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = (FastCacheKey) obj;
            return (other.prefix == prefix) && (other.sql == sql);
        }
    }
}
