namespace DbReader
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Caching;

    public static class DbConnectionExtensions
    {                
        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string sql, object arguments = null)
        {            
            var command = DbReaderOptions.CommandFactory.CreateCommand(dbConnection, sql, arguments);                
            var dataReader = command.ExecuteReader();
            SqlStatement.Current = sql;
            return dataReader.Read<T>();                                                                
        }

        public static async Task<IEnumerable<T>> ReadAsync<T>(
            this IDbConnection dbConnection,
            string sql,
            object arguments = null)
        {
            var command = DbReaderOptions.CommandFactory.CreateCommand(dbConnection, sql, arguments);
            var dataReader = await ((DbCommand)command).ExecuteReaderAsync();
            SqlStatement.Current = sql;
            return dataReader.Read<T>();
        }
    }


    
}