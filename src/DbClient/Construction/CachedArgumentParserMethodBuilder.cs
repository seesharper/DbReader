using System;

namespace DbClient.Construction
{
    using System.Data;
    using DbClient.Database;

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

        /// <inheritdoc/>
        public Func<string, object, Func<IDataParameter>, QueryInfo> CreateMethod(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {
            var key = (sql, argumentsType);
            var method = Cache<(string, Type), Func<string, object, Func<IDataParameter>, QueryInfo>>.Get(key);
            if (method == null)
            {
                method = argumentParserMethodBuilder.CreateMethod(sql, argumentsType, existingParameters);
                Cache<(string, Type), Func<string, object, Func<IDataParameter>, QueryInfo>>.Put(key, method);
            }
            return method;
        }
    }



}
