using System.Data;

namespace DbClient.Database
{

    /// <summary>
    /// Represents information about the query to be executed and its data parameters.
    /// </summary>
    public class QueryInfo
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryInfo"/> class.
        /// </summary>
        /// <param name="query">The query/SQL to be executed.</param>
        /// <param name="parameters">The data parameters to be used when executing the query.</param>
        public QueryInfo(string query, IDataParameter[] parameters)
        {
            Query = query;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the query/SQL to be executed.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Gets the data parameters to be used when executing the query.
        /// </summary>
        public IDataParameter[] Parameters { get; }
    }
}