namespace DbReader.Tests
{
    public class SQL
    {
        private static ISqlProvider sqlProvider = new SqlProvider();

        public static string Customers => sqlProvider.Customers;

        public static string CustomersAndOrders => sqlProvider.CustomersAndOrders;

        public static string EmployeesHierarchy => sqlProvider.EmployeesHierarchy;

        public static string EmployeesWithOrdersAndTerritories => sqlProvider.EmployeesWithOrdersAndTerritories;
    }
}