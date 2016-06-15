namespace DbReader.Database
{
    /// <summary>
    /// Represents a class that is capable of parsing a SQL statement 
    /// and return a list of parameters.
    /// </summary>
    public interface IParameterParser
    {
        /// <summary>
        /// Gets a list that represents the names of the parameters 
        /// used in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The sql statement to be parsed.</param>
        /// <returns>A list of parameters used in the given <paramref name="sql"/>.</returns>
        string[] GetParameters(string sql);
    }
}