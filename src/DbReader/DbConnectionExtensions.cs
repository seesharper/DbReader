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

    public static class DbConnectionExtensions
    {
        private static readonly IServiceContainer Container = new ServiceContainer();
        private static readonly IDbCommandFactory DbCommandFactory;

        static DbConnectionExtensions()
        {
            Container.RegisterFrom<CompositionRoot>();
            DbCommandFactory = Container.GetInstance<IDbCommandFactory>();
        }

        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string sql, object arguments = null)
        {
            var command = CreateCommand(dbConnection, sql, arguments);
            using (var dataReader = command.ExecuteReader())
            {
                return dataReader.Read<T>();
            }                
        }

        public static IDataReader ExecuteReader(this IDbConnection dbConnection, string sql, object arguments)
        {
            var command = CreateCommand(dbConnection, sql, arguments);
            return command.ExecuteReader();
        }

        public static IDbCommand CreateCommand(this IDbConnection dbConnection, string sql, object arguments)
        {
            SqlStatement.Current = sql;
            var command = DbCommandFactory.CreateCommand(dbConnection, sql, arguments);
            CommandInitializer?.Invoke(command);
            return command;
        }

        public static async Task<IEnumerable<T>> ReadAsync<T>(
            this IDbConnection dbConnection,
            string sql,
            object arguments = null)
        {
            var command = CreateCommand(dbConnection, sql, arguments);
            using (var dataReader = await ((DbCommand) command).ExecuteReaderAsync())
            {
                return dataReader.Read<T>();
            }                
        }
    }    
}