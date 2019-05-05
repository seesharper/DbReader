#if !NET462
namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Construction;
    //using Dapper;
    using Extensions;
    using Selectors;
    using Shouldly;
    using DbReader.LightInject;
    using DbReader;
    using Readers;
    using Xunit;
    using System.Text;
    using System.Data.SQLite;

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
            var pathToScript = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "db", "your_sqlite_text.txt");
            var script = File.ReadAllText(pathToScript, Encoding.UTF8);
            return script;

        }

        private IDbConnection CreateConnection()
        {
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        [Fact]
        public void ShouldReadCustomers()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>(SQL.Customers);
                customers.Count().ShouldBe(93);
            }
        }

        [Fact]
        public void ShouldReturnEmptyListWithoutNavigationProperties()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE 1 = 2");
                customers.Count().ShouldBe(0);
            }
        }

        [Fact]
        public void ShouldReturnEmptyListWithNavigationProperties()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<CustomerWithOrders>("SELECT CustomerId as CustomerWithOrdersId FROM Customers WHERE 1 = 2");
                customers.Count().ShouldBe(0);
            }
        }


        public async Task ShouldHandleRunningInParallel()
        {
            var task1 = ShouldReadCustomersAndOrdersAsync();
            var task2 = ShouldReadCustomersAndOrdersAsync();
            var task3 = ShouldReadCustomersAndOrdersAsync();
            var task4 = ShouldReadCustomersAndOrdersAsync();

            await Task.WhenAll(task1, task2, task3, task4);
        }


        [Fact]
        public async Task ShouldReadCustomersAsync()
        {
            using (var connection = CreateConnection())
            {
                var customers = await connection.ReadAsync<Customer>(SQL.Customers);
                customers.Count().ShouldBe(93);
            }
        }

        [Fact]
        public async Task ShouldReadCustomersAsyncWithCustomKeySelector()
        {
            DbReaderOptions.KeySelector<CustomerWithCustomKeySelector>(c => c.CustomerId);
            using (var connection = CreateConnection())
            {
                var customers = await connection.ReadAsync<Customer>(SQL.Customers);
                customers.Count().ShouldBe(93);
            }
        }


        [Fact]
        public void ShouldReadCustomersAndOrders()
        {
            using (var connection = CreateConnection())
            {
                var test = SQL.CustomersAndOrders;
                var customers = connection.Read<CustomerWithOrders>(SQL.CustomersAndOrders);
                customers.Count().ShouldBe(89);
                customers.SelectMany(c => c.Orders).Count().ShouldBe(830);
            }
        }

        [Fact]
        public async Task ShouldReadCustomersAndOrdersAsync()
        {
            using (var connection = CreateConnection())
            {
                await Task.Delay(10);
                var customers = await connection.ReadAsync<CustomerWithOrders>(SQL.CustomersAndOrders);
                customers.Count().ShouldBe(89);
                customers.SelectMany(c => c.Orders).Count().ShouldBe(830);
            }
        }

        [Fact]
        public void ShouldReadCustomerById()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    new { CustomerId = "ALFKI" });
                customers.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void ShouldReadCustomerByIdUsingDataParameter()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    new { CustomerId = new SQLiteParameter("@CustomerId", "ALFKI") });
                customers.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void ShouldReadEmployeesWithOrdersAndTerritories()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Employee>(SQL.EmployeesWithOrdersAndTerritories, new { EmployeeId = 7 });
                customers.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void
        ShouldThrowExceptionWhenArgumentNotFound()
        {
            using (var connection = CreateConnection())
            {
                Should.Throw<InvalidOperationException>(
                    () =>
                        connection.Read<CustomerWithOrders>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                            new { InvalidArgument = "ALFKI" }))
                    .Message.ShouldStartWith("Unable to resolve an argument value for parameter");
            }
        }

        [Fact]
        public void ShouldBeAbleToConvertArgument()
        {
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Employee>(SQL.EmployeesWithOrdersAndTerritories,
                    new { EmployeeId = new CustomValueType(7) });
                customers.Count().ShouldBe(1);
            }

        }

        [Fact]
        public void ShouldInvokeCommandInitializer()
        {
            bool invoked = false;
            DbReaderOptions.CommandInitializer = command => { invoked = true; };

            using (var connection = CreateConnection())
            {
                connection.CreateCommand(SQL.Customers);
            }
            invoked.ShouldBeTrue();

            DbReaderOptions.CommandInitializer = null;
        }

        [Fact]
        public void ShouldReadEmployeeHierarchy()
        {
            using (var connection = CreateConnection())
            {
                var employees = connection.Read<Employee>(SQL.EmployeesHierarchy).ToArray();
                Dictionary<long?, Employee> map = new Dictionary<long?, Employee>();
                foreach (var employee in employees)
                {
                    if (employee.ReportsTo != null)
                    {
                        map[employee.ReportsTo].Employees.Add(employee);
                    }
                    map.Add(employee.EmployeeId, employee);
                }

                var initialEmployee = map.First().Value;
                initialEmployee.Employees.Count().ShouldBe(5);
            }
        }

        [Fact]
        public void ShouldReturnRowsAffected()
        {
            using (var connection = CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var rowsAffected = connection.Execute("UPDATE Regions SET RegionDescription = @description", new { Description = "SomeDescription" });
                    transaction.Rollback();
                    rowsAffected.ShouldBe(4);
                }
            }
        }

        [Fact]
        public async Task ShouldReturnRowsAffectedAsync()
        {
            using (var connection = CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var rowsAffected = await connection.ExecuteAsync("UPDATE Regions SET RegionDescription = @description", new { Description = "SomeDescription" });
                    transaction.Rollback();
                    rowsAffected.ShouldBe(4);
                }
            }
        }

        [Fact]
        public void ShouldGetScalarValue()
        {
            using (var connection = CreateConnection())
            {
                var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Customers");
                count.ShouldBe(93);
            }
        }

        [Fact]
        public void ShouldConvertScalarValue()
        {
            using (var connection = CreateConnection())
            {
                var count = connection.ExecuteScalar<long>("SELECT '42' FROM Customers WHERE CustomerID = 'ALFKI'");
                count.ShouldBe(42);
            }
        }


        [Fact]
        public async Task ShouldGetScalarValueAsync()
        {
            using (var connection = CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Customers");
                count.ShouldBe(93);
            }
        }

        [Fact]
        public async Task ShouldReturnDefaultValueForNonExistingScalarValue()
        {
            using (var connection = CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<string>("SELECT CustomerId FROM Customers WHERE CustomerID = 'InvalidCustomerID'");
                id.ShouldBeNull();
            }
        }

        [Fact]
        public async Task ShouldReturnDefaultValueWhenValueIsDbNull()
        {
            using (var connection = CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<string>("SELECT Region FROM Customers WHERE CustomerID = 'ALFKI'");
                id.ShouldBeNull();
            }
        }

        [Fact]
        public async Task ShouldExecuteReaderAsyncWithoutCancellationToken()
        {
            using (var connection = CreateConnection())
            {
                var reader = await connection.ExecuteReaderAsync("SELECT City FROM Customers WHERE CustomerID = 'ALFKI'");
                reader.Read();
                var city = reader.GetString(0);
                city.ShouldBe("Berlin");
            }
        }


        [Fact]
        public void ShouldHandleList()
        {
            using (var connection = CreateConnection())
            {
                var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Customers WHERE CustomerID IN (@Ids)", new { Ids = new[] { "ALFKI", "BLAUS" } });
                count.ShouldBe(2);
            }
        }

        [Fact]
        public void ShouldHandleListWithCustomType()
        {
            using (var connection = CreateConnection())
            {
                var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Suppliers WHERE SupplierId IN (@Ids)", new { Ids = new[] { new CustomValueType(1), new CustomValueType(2) } });
                count.ShouldBe(2);
            }
        }

        [Fact]
        public void ShouldThrowMeaningfulExpectionWhenArgumentIsNotEnumerable()
        {
            using (var connection = CreateConnection())
            {
                var exception = Should.Throw<InvalidOperationException>(() => connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Suppliers WHERE SupplierId IN (@Ids)", new { Ids = 10 }));
                exception.Message.ShouldBe("The parameter @Ids is defined a list parameter, but the property Ids is not IEnumerable<T>");
            }
        }


        private string LoadSql(string name)
        {
            var provider = new SqlProvider();
            return provider.Load(name);
        }

        //public void ShouldOutPerformDapperForListWithoutParameter(
        //    IReaderMethodBuilder<CustomerWithOrders> propertyReaderMethodBuilder, IOrdinalSelector ordinalSelector)
        //{



        //    using (var connection = CreateConnection())
        //    {
        //        var dapperResult = Measure.Run(() => connection.Query<Customer>(SQL.Customers), 100,
        //            "Dapper (All Customers)");
        //        var dbReaderResult = Measure.Run(() => connection.Read<Customer>(SQL.Customers), 100,
        //            "DbReader (All Customers)");

        //        Console.WriteLine(dbReaderResult);
        //        Console.WriteLine(dapperResult);
        //    }
        //}

        //public void DbReaderVsDapper()
        //{
        //    GetAllCustomersUsingDapper();
        //    GetAllCustomersUsingDbReader();
        //    //GetAllCustomersUsingPropertyReader();
        //}



        [Fact]
        public void GetAllCustomersUsingDbReader()
        {
            using (var connection = CreateConnection())
            {
                var dbReaderResult = Measure.Run(() => connection.Read<Customer>(SQL.Customers), 10,
                    "DbReader (All Customers)");
                Console.WriteLine(dbReaderResult);
            }
        }

        //public void GetAllCustomersUsingDapper()
        //{
        //    using (var connection = CreateConnection())
        //    {
        //        var dbReaderResult = Measure.Run(() => connection.Query<Customer>(SQL.Customers), 10,
        //            "Dapper (All Customers)");
        //        Console.WriteLine(dbReaderResult);
        //    }
        //}

        public void GetAllCustomersUsingPlainDataReader()
        {
            using (var connection = CreateConnection())
            {


                var dbReaderResult = Measure.Run(() =>
                {
                    var command = connection.CreateCommand();
                    command.CommandText = LoadSql("Customers");
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

        //public void GetAllCustomersUsingPropertyReader()
        //{
        //    var container = new ServiceContainer();
        //    container.RegisterFrom<CompositionRoot>();

        //    var propertyreaderMethodBuilder = container.GetInstance<IReaderMethodBuilder<Customer>>("PropertyReaderMethodBuilder");
        //    var method = propertyreaderMethodBuilder.CreateMethod();
        //    int[] ordinals = new[] {0,1,2,3};
        //    using (var connection = CreateConnection())
        //    {


        //        var dbReaderResult = Measure.Run(() =>
        //        {
        //            List<Customer> result = new List<Customer>();
        //            //var command = connection.CreateCommand(SQL.Customers, null);
        //            var command = connection.CreateCommand();
        //            command.CommandText = SQL.Customers;
        //            using (var reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var customer = method(reader, ordinals);
        //                    result.Add(customer);
        //                }

        //            }

        //        }, 10,
        //            "DbReader-PropertyReader (All Customers)");
        //        Console.WriteLine(dbReaderResult);
        //    }

        //}

    }
}
#endif