namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Database;

    /// <summary>
    /// Represents a class that based on a given sql and the type of the arguments object,
    /// can create a method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.
    /// </summary>
    public interface IArgumentParserMethodBuilder
    {
        /// <summary>
        /// Creates a method at runtime that maps an argument object instance into a list of data parameters.
        /// </summary>
        /// <param name="sql">The sql statement for which to create the method.</param>
        /// <param name="argumentsType">The arguments type for which to create the method.</param>
        /// <param name="existingParameters">A list of already existing parameters.</param>
        /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        Func<string, object, Func<IDataParameter>, QueryInfo> CreateMethod(string sql, Type argumentsType, IDataParameter[] existingParameters);
    }
}