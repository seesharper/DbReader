using System.Data;

namespace DbReader.Database
{
    public class QueryInfo
    {


        public QueryInfo(string query, IDataParameter[] parameters)
        {
            Query = query;
            Parameters = parameters;
        }

        public string Query { get; }
        public IDataParameter[] Parameters { get; }
    }
}