using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader.Construction
{
    using LightInject;

    public static class FastCache<TValue>
    {
        private static ImmutableHashTree<FastCacheKey, TValue> tree = ImmutableHashTree<FastCacheKey, TValue>.Empty;
        
                
    }

    public class FastCacheKey
    {
        private readonly Type type;
        private readonly string prefix;
        private readonly string sql;

        public FastCacheKey(Type type, string prefix, string sql)
        {
            this.type = type;
            this.prefix = prefix;
            this.sql = sql;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = (FastCacheKey) obj;
            return (other.type == type) && (other.prefix == prefix) && (other.sql == sql);
        }
    }
}
