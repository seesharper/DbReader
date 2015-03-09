namespace DbReader.Tests
{
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Should;

    public class IntegrationTests
    {
        private string dbFile = @"..\..\db\northwind.db";

        private string connectionString = @"Data Source = ..\..\db\northwind.db";

        public IntegrationTests()
        {

            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
                using (var connection = new SQLiteConnection("Data Source = " + dbFile))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = ReadScript();
                    command.ExecuteNonQuery();
                }
            }
        }

        private string ReadScript()
        {
            using (StreamReader reader = new StreamReader(@"..\..\db\northwind.sql"))
            {
                return reader.ReadToEnd();
            }
        }

        private IDbConnection CreateConnection()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }


        public void ShouldReadCustomers()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers");
                customers.Count().ShouldEqual(93);
            }
        }

        public void ShouldReadCustomersAndOrders()
        {            
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>(SQL.CustomersAndOrders);
                customers.Count().ShouldEqual(89);
                customers.SelectMany(c => c.Orders).Count().ShouldEqual(830);
            }
        }


        public void ShouldReadCustomerById()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId", new { CustomerId = "ALFKI" });
                customers.Count().ShouldEqual(1);
            }
        }



    }


    //public class Customer
    //{
    //    public string CustomerId { get; set; }
    //}
}