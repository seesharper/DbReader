namespace DbReader
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using DbReader.Interfaces;
    using DbReader.LightInject;

    public static class DbConnectionExtensions
    {
        private static IServiceContainer serviceContainer = new ServiceContainer();

        static DbConnectionExtensions()
        {
            serviceContainer.RegisterFrom<CompositionRoot>();
        }

        public static IEnumerable<T> Read<T>(this IDbConnection dbConnection, string sql, object arguments = null)
        {
            using (serviceContainer.BeginScope())
            {
                var result = new Collection<T>();
                var commandFactory = serviceContainer.GetInstance<ICommandFactory>();
                var command = commandFactory.CreateCommand(dbConnection, sql, arguments);
                var instanceReader = serviceContainer.GetInstance<IInstanceReader<T>>();
                var dataReader = command.ExecuteReader();
                
                while (dataReader.Read())
                {
                    result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                }

                return result;
            }                                    
        }

        public static async Task<IEnumerable<T>> ReadAsync<T>(
            this IDbConnection dbConnection,
            string sql,
            object arguments)
        {
            var result = new Collection<T>();
            var commandFactory = serviceContainer.GetInstance<ICommandFactory>();
            var command = (DbCommand)commandFactory.CreateCommand(dbConnection, sql, arguments);           
            var reader = command.ExecuteReaderAsync().ConfigureAwait(false);
            var instanceReader = serviceContainer.GetInstance<IInstanceReader<T>>();
            return null;
        }
    }


    
}