using System;
using System.Data;

namespace DbReader.Construction
{
    /// <summary>
    /// Represents a class that is capable of matching the properties of a given arguments type to the parameters found in a SQL statement.
    /// </summary>
    public interface IParameterMatcher
    {
        /// <summary>
        /// Matches the properties of the <paramref name="argumentsType"/> to the parameters found in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The sql statement containing the parameters.</param>
        /// <param name="argumentsType">The argument object type.</param>
        /// <param name="existingParameters">A list of existing data parameters.</param>
        /// <returns>A list of matching parameters.</returns>
        MatchedParameter[] Match(string sql, Type argumentsType, IDataParameter[] existingParameters);
    }
}
