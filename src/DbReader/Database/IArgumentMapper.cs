namespace DbReader.Database
{
    using System.Collections.Generic;
    using Construction;

    /// <summary>
    /// Represents a class that is capable of mapping the argument values to 
    /// the parameters present in a given SQL statement.
    /// </summary>
    public interface IArgumentMapper
    {
        /// <summary>
        /// Maps the argument values represented by the <paramref name="arguments"/>
        /// to the parameters defined in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The query containing the parameters.</param>
        /// <param name="arguments">An object where each readable property represents a named argument value.</param>
        /// <returns>A </returns>
        IReadOnlyDictionary<string, ArgumentValue> Map(string sql, object arguments);
    }
}