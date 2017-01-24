namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Database;
    using LightInject;
    using static DbReaderOptions;

    /// <summary>
    /// Extends the <see cref="IDbConnection"/> interface.
    /// </summary>
    public static class DbConnectionExtensions
    {
        private static readonly IServiceContainer Container = new ServiceContainer();
        private static readonly IDbCommandFactory DbCommandFactory;

        static DbConnectionExtensions()
        {
            Container.RegisterFrom<CompositionRoot>();
            DataReaderExtensions.SetContainer(Container);
            DbCommandFactory = Container.GetInstance<IDbCommandFactory>();
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns an <see cref="IEnumerable{T}"/> that 
        /// represents the result of the query.
        /// </summary>
        /// <typeparam name="T">The type to be projected from the query.</typeparam>
        /// <param name="dbConnection">The target <see cref="IDbConnection"/>.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">An object that represents the query arguments.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string query, object arguments = null)
        {                        
            using (var dataReader = dbConnection.ExecuteReader(query, arguments))
            {
                return dataReader.Read<T>();
            }                
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns an <see cref="IEnumerable{T}"/> that 
        /// represents the result of the query.
        /// </summary>
        /// <typeparam name="T">The type to be projected from the query.</typeparam>
        /// <param name="dbConnection">The target <see cref="IDbConnection"/>.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">An object that represents the query arguments.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static async Task<IEnumerable<T>> ReadAsync<T>(
           this IDbConnection dbConnection,
           string query,
           object arguments = null)
        {
            using (var dataReader = await dbConnection.ExecuteReaderAsync(query, arguments))
            {
                SqlStatement.Current = query;
                return dataReader.Read<T>();
            }
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReader(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = CreateCommand(dbConnection, query, arguments);            
            return command.ExecuteReader();
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = CreateCommand(dbConnection, query, arguments);
            return await ((DbCommand)command).ExecuteReaderAsync().ConfigureAwait(false);
        }


        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string sql, object arguments = null)
        {
            SqlStatement.Current = sql;
            var command = DbCommandFactory.CreateCommand(dbConnection, sql, arguments);
            CommandInitializer?.Invoke(command);
            return command;
        }

       

      
    }    
}