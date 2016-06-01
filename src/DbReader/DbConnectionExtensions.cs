namespace DbReader
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;

    using DbReader.Interfaces;
    using DbReader.LightInject;

    public static class DbConnectionExtensions
    {
        private static readonly IServiceContainer ServiceContainer = new ServiceContainer();

        static DbConnectionExtensions()
        {
            ServiceContainer.RegisterFrom<CompositionRoot>();            
        }

        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string sql, object arguments = null)
        {
            using (ServiceContainer.BeginScope())
            {
                var result = new HashSet<T>();
                var commandFactory = ServiceContainer.GetInstance<ICommandFactory>();
                var command = commandFactory.CreateCommand(dbConnection, sql, arguments);
                var instanceReader = ServiceContainer.GetInstance<IInstanceReader<T>>();
                var dataReader = command.ExecuteReader();
                
                while (dataReader.Read())
                {
                    result.Add(instanceReader.Read(dataReader, string.Empty));                    
                }

                return result;
            }                                    
        }

        public static async Task<IEnumerable<T>> ReadAsync<T>(
            this IDbConnection dbConnection,
            string sql,
            object arguments = null)
        {
            using (ServiceContainer.BeginScope())
            {
                var result = new HashSet<T>();
                var commandFactory = ServiceContainer.GetInstance<ICommandFactory>();
                var command = (DbCommand) commandFactory.CreateCommand(dbConnection, sql, arguments);
                var dataReader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                var instanceReader = ServiceContainer.GetInstance<IInstanceReader<T>>();
                while (dataReader.Read())
                {
                    result.Add(instanceReader.Read(dataReader, string.Empty));
                }

                return result;
            }
        }
    }


    
}