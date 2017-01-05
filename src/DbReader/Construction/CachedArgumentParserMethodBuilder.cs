using System;

namespace DbReader.Construction
{
    using System.Data;

    /// <summary>
    /// An <see cref="IArgumentParserMethodBuilder"/> decorator that caches 
    /// the method created at runtime that is used to parse argument from an argument object.
    /// </summary>
    public class CachedArgumentParserMethodBuilder : IArgumentParserMethodBuilder
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedArgumentParserMethodBuilder"/> class.
        /// </summary>
        /// <param name="argumentParserMethodBuilder">The target <see cref="IArgumentParserMethodBuilder"/>.</param>
        public CachedArgumentParserMethodBuilder(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }

        /// <summary>
        /// Creates a method at runtime that maps an argument object instance into a list of data parameters.
        /// </summary>
        /// <param name="sql">The sql statement for which to create the method.</param>
        /// <param name="argumentsType">The arguments type for which to create the method.</param>
        /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        public Func<object, Func<IDataParameter>, IDataParameter[]> CreateMethod(string sql, Type argumentsType)
        {
            var key = new CacheKey {Sql = sql, ArgumentsType = argumentsType};
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
