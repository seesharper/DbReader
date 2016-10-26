namespace DbReader.Database
{
    using System.Data;
    using Interfaces;

    /// <summary>
    /// A class that is capable of creating a <see cref="IDbCommand"/>
    /// </summary>
    public class DbCommandFactory : IDbCommandFactory
    {
        private readonly IArgumentParser argumentParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandFactory"/> class.
        /// </summary>
        /// <param name="argumentMapper">The <see cref="IArgumentMapper"/> that is responsible for mapping the argument values 
        /// to the parameters present in a given SQL statement.</param>
        /// <param name="argumentParser"></param>
        public DbCommandFactory(IArgumentParser argumentParser)
        {
            this.argumentParser = argumentParser;
        }

        /// <summary>
        /// Creates an <see cref="IDbCommand"/> that represents executing the given <paramref name="sql"/>
        /// using the given <paramref name="arguments"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> used to execute the <see cref="IDbCommand"/>.</param>
        /// <param name="sql">The SQL statement to be executed.</param>
        /// <param name="arguments">An object that represents the argument values used when executing the query.</param>
        /// <returns></returns>
        public IDbCommand CreateCommand(IDbConnection dbConnection, string sql, object arguments)
        {
            var command = dbConnection.CreateCommand();            
            command.CommandText = sql;
            var parameters = argumentParser.Parse(sql, arguments, () => command.CreateParameter());
            
            foreach (var parameter in parameters)
            {                                
                command.Parameters.Add(parameter);
            }
            return command;
        }
    }
}