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
    using DbReader.Extensions;
    using LightInject;
    using static DbReaderOptions;

    /// <summary>
    /// Extends the <see cref="IDbConnection"/> interface.
    /// </summary>
    public static class DbConnectionExtensions
    {
        private static readonly IServiceContainer Container = new ServiceContainer();
        private static readonly IArgumentParser ArgumentParser;

        private static readonly IParameterParser ListParameterParser;

        static DbConnectionExtensions()
        {
            Container.RegisterFrom<CompositionRoot>();
            DataReaderExtensions.SetContainer(Container);
            ArgumentParser = Container.GetInstance<IArgumentParser>();
            ListParameterParser = Container.GetInstance<IParameterParser>("ListParameterParser");
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
            return await dbConnection.ReadAsync<T>(CancellationToken.None, query, arguments).ConfigureAwait(false);
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
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of the query.</returns>
        public static async Task<IEnumerable<T>> ReadAsync<T>(
           this IDbConnection dbConnection,
           CancellationToken cancellationToken,
           string query,
           object arguments = null)
        {
            using (var dataReader = await dbConnection.ExecuteReaderAsync(cancellationToken, query, arguments).ConfigureAwait(false))
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
            return await dbConnection.ExecuteReaderAsync(CancellationToken.None, query, arguments).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns an <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns><see cref="IDataReader"/></returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection dbConnection, CancellationToken cancellationToken, string query, object arguments = null)
        {
            var command = CreateCommand(dbConnection, query, arguments);
            return await ((DbCommand)command).ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new <see cref="IDbCommand"/> with the given <paramref name="query"/> and <paramref name="arguments"/>.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when creating the command.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns><see cref="IDbCommand"/></returns>
        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = dbConnection.CreateCommand();
            IDataParameter[] parameters;
            var listParameters = ListParameterParser.GetParameters(query);
            if (listParameters.Length > 0)
            {
                var result = PreProcessQuery(query, arguments, () => command.CreateParameter());
                query = result.query;
                parameters = result.parameters;
            }
            else
            {
                parameters = ArgumentParser.Parse(query, arguments, () => command.CreateParameter(), command.Parameters.Cast<IDataParameter>().ToArray());
            }

            SqlStatement.Current = query;
            command.CommandText = query;
            CommandInitializer?.Invoke(command);

            foreach (var dataParameter in parameters)
            {
                command.Parameters.Add(dataParameter);
            }
            return command;
        }

        private static (string query, IDataParameter[] parameters) PreProcessQuery(string query, object arguments, Func<IDataParameter> parameterFactory)
        {
            var result = new List<IDataParameter>();
            var expandedQuery = query;
            var readableProperties = Container.GetInstance<DbReader.Selectors.IPropertySelector>("ReadablePropertySelector").Execute(arguments.GetType());
            var propertyMap = readableProperties.ToDictionary(p => p.Name, p => p, StringComparer.InvariantCultureIgnoreCase);
            var enumerableProperties = readableProperties.Where(p => p.PropertyType.IsEnumerable()).Where(p => !p.PropertyType.IsSimpleType()).ToArray();
            var parameterParser = new RegExParameterParser(@"IN\s*\(((?:@|:)\w+)\)");
            var listParameters = parameterParser.GetParameters(query);

            foreach (var listParameter in listParameters)
            {
                var propertyName = listParameter.Substring(1);
                if (propertyMap.TryGetValue(propertyName, out var property))
                {
                    List<string> expandedParameterList = new List<string>();
                    var propertyValues = ((IEnumerable<object>)property.GetValue(arguments)).ToArray();
                    for (int i = 0; i < propertyValues.Count(); i++)
                    {
                        var parameterName = $"{listParameter}{i}";
                        expandedParameterList.Add(parameterName);
                        var dataParameter = parameterFactory();
                        dataParameter.ParameterName = parameterName;
                        dataParameter.Value = propertyValues[i];
                        result.Add(dataParameter);
                    }
                    var expandedParameterFragment = expandedParameterList.Aggregate((current, next) => $"{current}, {next}");
                    expandedQuery = expandedQuery.Replace(listParameter, expandedParameterFragment);
                }
            }

            return (expandedQuery, result.ToArray());
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns>The number of rows affected.</returns>
        public static int Execute(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = CreateCommand(dbConnection, query, arguments);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection dbConnection, string query, object arguments = null)
        {
            return await dbConnection.ExecuteAsync(CancellationToken.None, query, arguments).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the given <paramref name="query"/> asynchronously and returns the number of rows affected.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <returns>The number of rows affected.</returns>
        public static async Task<int> ExecuteAsync(this IDbConnection dbConnection, CancellationToken cancellationToken, string query, object arguments = null)
        {
            var command = (DbCommand)CreateCommand(dbConnection, query, arguments);
            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the query asynchronously , and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The type of value to be returned.</returns>
        public static async Task<T> ExecuteScalarAsync<T>(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = (DbCommand)CreateCommand(dbConnection, query, arguments);
            var value = await command.ExecuteScalarAsync();
            return ConvertFromDbValue<T>(value);

        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection"/> to be used when executing the query.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="arguments">The argument object that represents the arguments passed to the query.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The type of value to be returned.</returns>
        public static T ExecuteScalar<T>(this IDbConnection dbConnection, string query, object arguments = null)
        {
            var command = CreateCommand(dbConnection, query, arguments);
            var value = command.ExecuteScalar();
            return ConvertFromDbValue<T>(value);
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