namespace DbReader.Database
{
    using System;
    using System.Data;
    using Construction;

    /// <summary>
    /// A class that parses an SQL statement and
    /// maps each parameter to the properties of an arguments object.
    /// </summary>
    public class ArgumentParser : IArgumentParser
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentParser"/> class.
        /// </summary>
        /// <param name="argumentParserMethodBuilder">The <see cref="IArgumentParserMethodBuilder"/>
        /// that is responsible for creating a method at runtime that maps an argument object instance into a list of data parameters.</param>
        public ArgumentParser(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }

        /// <inheritdoc/>
        public QueryInfo Parse(string sql, object arguments, Func<IDataParameter> parameterFactory, IDataParameter[] existingParameters)
        {
            if (arguments == null)
            {
                return new QueryInfo(sql, new IDataParameter[] { });
            }
            var argumentParseMethod = argumentParserMethodBuilder.CreateMethod(sql, arguments.GetType(), existingParameters);
            return argumentParseMethod(sql, arguments, parameterFactory);
        }
    }
}