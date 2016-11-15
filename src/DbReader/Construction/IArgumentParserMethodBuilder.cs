namespace DbReader.Construction
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that based on a given sql and the type of the arguments object,
    /// can create a method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.
    /// </summary>
    public interface IArgumentParserMethodBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql">The sql statement for which to create the method.</param>
        /// <param name="argumentsType">The arguments type for which to create the method.</param>
        /// <returns>A method that maps an argument object instance into a list of <see cref="IDataParameter"/> instances.</returns>
        Func<object, Func<IDataParameter> ,  IDataParameter[]> CreateMethod(string sql, Type argumentsType);
    }   
}