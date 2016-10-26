namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Construction;
    using Dapper;
    using Extensions;
    using Selectors;
    using Shouldly;
    using DbReader.LightInject;
    using DbReader;
    public class IntegrationTests
    {
        private string dbFile = @"..\..\db\northwind.db";

        private string connectionString = @"Data Source = ..\..\db\northwind.db";

        static IntegrationTests()
        {
            DbReaderOptions.WhenPassing<CustomValueType>()
                .Use((parameter, argument) => parameter.Value = argument.Value);
        }


        public IntegrationTests()
        {

            if (!File.Exists(dbFile))
            {
                Console.WriteLine("Hold your horses..creating database...");
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
                var customers = connection.Read<Customer>(SQL.Customers);
                customers.Count().ShouldBe(93);
            }
        }

        public void ShouldReadCustomersAndOrders()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<CustomerWithOrders>(SQL.CustomersAndOrders);
                customers.Count().ShouldBe(89);
                customers.SelectMany(c => c.Orders).Count().ShouldBe(830);
            }
        }


        public void ShouldReadCustomerById()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    new {CustomerId = "ALFKI"});
                customers.Count().ShouldBe(1);
            }
        }

        public void ShouldReadEmployeesWithOrdersAndTerritories()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Employee>(SQL.EmployeesWithOrdersAndTerritories, new {EmployeeId = 7});
                customers.Count().ShouldBe(1);
            }
        }

        public void ShouldThrowExceptionWhenArgumentNotFound()
        {
            using (var connection = CreateConnection())
            {
                Should.Throw<InvalidOperationException>(
                    () =>
                        connection.Read<CustomerWithOrders>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                            new {InvalidArgument = "ALFKI"}))
                    .Message.ShouldStartWith("Unable to resolve an argument value for parameter");
            }
        }

        public void ShouldBeAbleToConvertArgument()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Employee>(SQL.EmployeesWithOrdersAndTerritories,
                    new {EmployeeId = new CustomValueType(7)});
                customers.Count().ShouldBe(1);
            }

        }


        public void ShouldOutPerformDapperForListWithoutParameter(
            IReaderMethodBuilder<CustomerWithOrders> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector)
        {



            using (var connection = CreateConnection())
            {
                var dapperResult = Measure.Run(() => connection.Query<Customer>(SQL.Customers), 100,
                    "Dapper (All Customers)");
                var dbReaderResult = Measure.Run(() => connection.Read<Customer>(SQL.Customers), 100,
                    "DbReader (All Customers)");
                
                Console.WriteLine(dbReaderResult);
                Console.WriteLine(dapperResult);
            }
        }

        public void DbReaderVsDapper()
        {
            GetAllCustomersUsingDapper();
            GetAllCustomersUsingDbReader();
            //GetAllCustomersUsingPropertyReader();
        }



        public void GetAllCustomersUsingDbReader()
        {
            using (var connection = CreateConnection())
            {               
                var dbReaderResult = Measure.Run(() => connection.Read<Customer>(SQL.Customers), 10,
                    "DbReader (All Customers)");
                Console.WriteLine(dbReaderResult);
            }
        }

        public void GetAllCustomersUsingDapper()
        {
            using (var connection = CreateConnection())
            {
                var dbReaderResult = Measure.Run(() => connection.Query<Customer>(SQL.Customers), 10,
                    "Dapper (All Customers)");
                Console.WriteLine(dbReaderResult);
            }
        }

        public void GetAllCustomersUsingPlainDataReader()
        {
            using (var connection = CreateConnection())
            {


                var dbReaderResult = Measure.Run(() =>
                {
                    var command = connection.CreateCommand();
                    command.CommandText = SQL.Customers;
                    var reader = command.ExecuteReader();
                    List<Customer> result = new List<Customer>();
                    while (reader.Read())
                    { 
                        Customer c = new Customer();
                        c.CustomerId = reader.GetString(0);
                        result.TryAdd(c);
                    }
                }, 10,
                    "DbReader (All Customers)");
                Console.WriteLine(dbReaderResult);
            }
        }

        public void GetAllCustomersUsingPropertyReader()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<CompositionRoot>();

            var propertyreaderMethodBuilder = container.GetInstance<IReaderMethodBuilder<Customer>>("PropertyReaderMethodBuilder");
            var method = propertyreaderMethodBuilder.CreateMethod();
            int[] ordinals = new[] {0,1,2,3};
            using (var connection = CreateConnection())
            {
                

                var dbReaderResult = Measure.Run(() =>
                {
                    List<Customer> result = new List<Customer>();
                    //var command = connection.CreateCommand(SQL.Customers, null);
                    var command = connection.CreateCommand();
                    command.CommandText = SQL.Customers;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = method(reader, ordinals);
                            result.Add(customer);
                        }
                        
                    }

                }, 10,
                    "DbReader-PropertyReader (All Customers)");
                Console.WriteLine(dbReaderResult);
            }

        }


    }
}