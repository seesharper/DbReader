namespace DbReader
{
    using System.Data;
    using System.Data.Common;

    public interface IDbCommandFactory
    {
        IDbCommand CreateCommand(IDbConnection dbConnection, string sql, object arguments);
    }


    public class DbCommandFactory : IDbCommandFactory
    {
        private readonly IArgumentProvider argumentProvider;

        public DbCommandFactory(IArgumentProvider argumentProvider)
        {
            this.argumentProvider = argumentProvider;
        }

        public IDbCommand CreateCommand(IDbConnection dbConnection, string sql, object arguments)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = sql;
            var parameterValues = argumentProvider.GetArguments(sql, arguments);
            foreach (var parameterValue in parameterValues)
            {
                var parameter = command.CreateParameter();
                parameter.Value = parameterValue.Value;
                parameter.ParameterName = parameterValue.Key;
                command.Parameters.Add(parameter);
            }
            return command;
        }
    }
}