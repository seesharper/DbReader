using System.Linq;

namespace DbReader
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Database;
    using DbReader.Construction;
    using DbReader.Extensions;
    using LightInject;
    using static DbReaderOptions;

    /// <summary>
    /// Extends the <see cref="IDbConnection"/> interface.
    /// </summary>
    public static class DbConnectionExtensions
    {
        private static readonly IServiceContainer Container = new ServiceContainer(options => options.EnableCurrentScope = false);
        private static readonly IArgumentParser ArgumentParser;

        static DbConnectionExtensions()
        {
            Container.RegisterFrom<CompositionRoot>();
            DataReaderExtensions.SetContainer(Container);
            ArgumentParser = Container.GetInstance<IArgumentParser>();
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns an <see cref="IEnumerable{T}"/> that
        /// represents the result of the query.
        /// </summary>
        /// <typeparam name="T">The type to be projected from the query.</typeparam>
        /// <param name="dbConnection">The target <see cref="IDbConnection"/>.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">An object that represents the query arguments.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var dataReader = dbConnection.ExecuteReader(query, arguments, configureCommand))
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
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static async Task<IEnumerable<T>> ReadAsync<T>(
           this IDbConnection dbConnection,
           string query,
           object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            return await dbConnection.ReadAsync<T>(CancellationToken.None, query, arguments, configureCommand).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns an <see cref="IEnumerable{T}"/> that
        /// represents the result of the query.
        /// </summary>
        /// <typeparam name="T">The type to be projected from the query.</typeparam>
        /// <param name="dbConnection">The target <see cref="IDbConnection"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">An object that represents the query arguments.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static async Task<IEnumerable<T>> ReadAsync<T>(
           this IDbConnection dbConnection,
           CancellationToken cancellationToken,
           string query,
           object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var dataReader = await dbConnection.ExecuteReaderAsync(cancellationToken, query, arguments, configureCommand).ConfigureAwait(false))
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
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReader(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            // var command = CreateCommand(dbConnection, query, arguments, configureCommand);
            // return command.ExecuteReader();
            var command = CreateCommand(dbConnection, query, arguments, configureCommand);
            return new CommandWrappingDataReader((DbCommand)command, (DbDataReader)command.ExecuteReader());

        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            return await dbConnection.ExecuteReaderAsync(CancellationToken.None, query, arguments, configureCommand).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection dbConnection, CancellationToken cancellationToken, string query, object arguments = null, Action<IDbCommand> configureCommand = null)
        {
            var command = CreateCommand(dbConnection, query, arguments, configureCommand);
            return new CommandWrappingDataReader((DbCommand)command, await ((DbCommand)command).ExecuteReaderAsync(cancellationToken));
        }

        /// <summary>
        /// Creates a new <see cref="IDbCommand"/> with the given <paramref name="query"/> and <paramref name="arguments"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when creating the command.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns><see cref="IDbCommand"/></returns>
        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = query;
            CommandInitializer?.Invoke(command);
            configureCommand?.Invoke(command);
            var queryInfo = ArgumentParser.Parse(query, arguments, () => command.CreateParameter(), command.Parameters.Cast<IDataParameter>().ToArray());

            SqlStatement.Current = queryInfo.Query;
            command.CommandText = queryInfo.Query;


            foreach (var dataParameter in queryInfo.Parameters)
            {
                command.Parameters.Add(dataParameter);
            }
            return command;
        }



        /// <summary>
        /// Executes the given <paramref name="query"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var command = CreateCommand(dbConnection, query, arguments, configureCommand))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            return await dbConnection.ExecuteAsync(CancellationToken.None, query, arguments, configureCommand).ConfigureAwait(false);
        }


        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection dbConnection, CancellationToken cancellationToken, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var command = (DbCommand)CreateCommand(dbConnection, query, arguments, configureCommand))
            {
                return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes the query asynchronously , and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The type of value to be returned.</returns>
        public static async Task<T> ExecuteScalarAsync<T>(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var command = (DbCommand)CreateCommand(dbConnection, query, arguments, configureCommand))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return ReadScalarValue<T>(reader);
                }
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <param name="configureCommand">A function used to configure the underlying <see cref="IDbCommand"/>.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The type of value to be returned.</returns>
        public static T ExecuteScalar<T>(this IDbConnection dbConnection, string query, object arguments = null, Action<IDbCommand> configureCommand = default)
        {
            using (var command = CreateCommand(dbConnection, query, arguments, configureCommand))
            {
                using (var reader = command.ExecuteReader())
                {
                    return ReadScalarValue<T>(reader);
                }
            }
        }

        private static T ReadScalarValue<T>(IDataReader reader)
        {
            if (!reader.Read() || reader.IsDBNull(0))
            {
                return default;
            }

            var underlyingType = typeof(T).GetUnderlyingType();

            if (ValueConverter.CanConvert(underlyingType))
            {
                return (T)ValueConverter.GetConvertMethod(underlyingType).Invoke(null, new object[] { reader, 0 });
            }

            return ConvertFromDbValue<T>(reader.GetValue(0));
        }

        private static T ConvertFromDbValue<T>(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}