using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader.Construction
{
    using System.Data;
    using Interfaces;

    public class CachedArgumentParserMethodBuilder : IArgumentParserMethodBuilder
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;

        public CachedArgumentParserMethodBuilder(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }


        public Func<object, Func<IDataParameter>, IDataParameter[]> CreateMethod(string sql, Type argumentsType)
        {
            var key = new CacheKey() {Sql = sql, ArgumentsType = argumentsType};
            var method = Cache<CacheKey, Func<object, Func<IDataParameter>, IDataParameter[]>>.Get(key);
            if (method == null)
            {
                method = argumentParserMethodBuilder.CreateMethod(sql, argumentsType);
                Cache<CacheKey, Func<object, Func<IDataParameter>, IDataParameter[]>>.Put(key, method);
            }
            return method;
        }


        private class CacheKey
        {
            public string Sql;
            public Type ArgumentsType;

            public override int GetHashCode()
            {
                return Sql.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = (CacheKey) obj;
                return (other.Sql == Sql) && (other.ArgumentsType == ArgumentsType);
            }
        }
    }

    
    
}
