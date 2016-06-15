namespace DbReader.Database
{
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a <see cref="IDbCommand"/>    
    /// </summary>
    public interface IDbCommandFactory
    {
        /// <summary>
        /// Creates an <see cref="IDbCommand"/> that represents executing the given <paramref name="sql"/>
        /// using the given <paramref name="arguments"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> used to execute the <see cref="IDbCommand"/>.</param>
        /// <param name="sql">The SQL statement to be executed.</param>
        /// <param name="arguments">An object that represents the argument values used when executing the query.</param>
        /// <returns></returns>
        IDbCommand CreateCommand(IDbConnection dbConnection, string sql, object arguments);
    }
}