namespace DbClient.Database
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
        DataParameterInfo[] GetParameters(string sql);
    }

    /// <summary>
    /// Contains information about a parsed data parameter.
    /// </summary>
    public class DataParameterInfo
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParameterInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the data parameter excluding the prefix.</param>
        /// <param name="fullName">The name of the data parameter including the prefix.</param>
        /// <param name="isListParameter">Determines if the data parameter is a list parameter that needs to be expanded.</param>
        public DataParameterInfo(string name, string fullName, bool isListParameter)
        {
            Name = name;
            FullName = fullName;
            IsListParameter = isListParameter;
        }

        /// <summary>
        /// Get the name of the data parameter excluding the prefix.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The name of the data parameter including the prefix.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets a value that indicates whether this is a list parameter that needs to be expanded.
        /// </summary>
        /// <value></value>
        public bool IsListParameter { get; }
    }
}