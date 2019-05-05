using System;

namespace DbReader.Construction
{
    using System.Data;
    using DbReader.Database;

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

        // /// <summary>
        // /// Creates a method at runtime that maps an argument object instance into a list of data parameters.
        // /// </summary>
        // /// <param name="sql">The sql statement for which to create the method.</param>
        // /// <param name="argumentsType">The arguments type for which to create the method.</param>
        // /// <param name="existingParameters">A list of already existing parameters.</param>
        // /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        // public Func<object, Func<IDataParameter>, IDataParameter[]> CreateMethod(string sql, Type argumentsType, IDataParameter[] existingParameters)
        // {
        //     var key = new CacheKey { Sql = sql, ArgumentsType = argumentsType };
        //     var method = Cache<CacheKey, Func<object, Func<IDataParameter>, IDataParameter[]>>.Get(key);
        //     if (method == null)
        //     {
        //         method = argumentParserMethodBuilder.CreateMethod(sql, argumentsType, existingParameters);
        //         Cache<CacheKey, Func<object, Func<IDataParameter>, IDataParameter[]>>.Put(key, method);
        //     }
        //     return method;
        // }

        public Func<string, object, Func<IDataParameter>, QueryInfo> CreateMethod2(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {
            var key = (sql, argumentsType);
            var method = Cache<(string, Type), Func<string, object, Func<IDataParameter>, QueryInfo>>.Get(key);
            if (method == null)
            {
                method = argumentParserMethodBuilder.CreateMethod2(sql, argumentsType, existingParameters);
                Cache<(string, Type), Func<string, object, Func<IDataParameter>, QueryInfo>>.Put(key, method);
            }
            return method;
        }
    }



}
