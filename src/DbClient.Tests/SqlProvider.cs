using System.IO;
using System.Text;

namespace DbClient.Tests
{
    public interface ISqlProvider
    {
        string Customers { get; }

        string CustomersAndOrders { get; }

        string EmployeesHierarchy { get; }

        string EmployeesWithOrdersAndTerritories { get; }

    }

    public class SqlProvider : ISqlProvider
    {

        public string Customers { get => Load("Customers"); }

        public string CustomersAndOrders { get => Load("CustomersAndOrders"); }

        public string EmployeesHierarchy { get => Load("EmployeesHierarchy"); }

        public string EmployeesWithOrdersAndTerritories { get => Load("EmployeesWithOrdersAndTerritories"); }

        public string Load(string name)
        {
            return LoadSql(name);
        }

        private static string LoadSql(string name)
        {
            var assembly = typeof(SqlProvider).Assembly;
            var test = assembly.GetManifestResourceNames();
            var resourceStream = assembly.GetManifestResourceStream($"DbClient.Tests.Queries.{name}.sql");
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }


}