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

        /// <summary>
        /// Parses the given <paramref name="sql"/> and maps each 
        /// parameter to the corresponding property of the <paramref name="arguments"/> object.
        /// </summary>
        /// <param name="sql">The sql statement containing the parameters to be parsed.</param>
        /// <param name="arguments">An object that represent the argument values for each parameter.</param>
        /// <param name="parameterFactory">A factory delegate used to create an <see cref="IDataParameter"/> instance.</param>
        /// <returns></returns>
        public IDataParameter[] Parse(string sql, object arguments, Func<IDataParameter> parameterFactory)
        {
            if (arguments == null)
            {
                return new IDataParameter[] {};
            }
            var argumentParseMethod = argumentParserMethodBuilder.CreateMethod(sql, arguments.GetType());
            return argumentParseMethod(arguments, parameterFactory);
        }       
    }
}