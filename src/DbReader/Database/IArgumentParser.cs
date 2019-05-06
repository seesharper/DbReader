namespace DbReader.Database
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that parses an SQL statement and
    /// maps each parameter to the properties of an arguments object.
    /// </summary>
    public interface IArgumentParser
    {
        /// <summary>
        /// Parses the given <paramref name="sql"/> and maps each
        /// parameter to the corresponding property of the <paramref name="arguments"/> object.
        /// </summary>
        /// <param name="sql">The sql statement containing the parameters to be parsed.</param>
        /// <param name="arguments">An object that represent the argument values for each parameter.</param>
        /// <param name="parameterFactory">A factory delegate used to create an <see cref="IDataParameter"/> instance.</param>
        /// <param name="existingParameters">A list of already existing parameters.</param>
        /// <returns></returns>
        QueryInfo Parse(string sql, object arguments, Func<IDataParameter> parameterFactory, IDataParameter[] existingParameters);
    }
}