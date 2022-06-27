#if !NET462
namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DbReader;
    using Extensions;
    using Shouldly;
    using Xunit;

    public class IntegrationTests
    {
        private static string dbFile;

        private static string connectionString;

        static IntegrationTests()
        {
            dbFile = Path.Combine(Path.GetDirectoryName(new Uri(typeof(IntegrationTests).Assembly.CodeBase).LocalPath), "northwind.db");
            connectionString = $"Data Source = {dbFile}";

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
            var pathToScript = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "db", "northwind.sql");
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


        [Fact]
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
        public void ShouldReadCustomersAsRecordType()
        {
            using (var connection = CreateConnection())
            {
                var test = SQL.Customers;
                var customers = connection.Read<CustomerRecord>("SELECT CustomerID, CompanyName FROM Customers");
                customers.Count().ShouldBe(93);
            }
        }

        [Fact]
        public void ShouldReadCustomersAndOrdersAsRecordType()
        {

            // DbReaderOptions.KeySelector<CustomerRecordWithOrders>(c => c.CustomerId);
            // DbReaderOptions.KeySelector<OrderRecord>(o => o.OrderId);

            using (var connection = CreateConnection())
            {
                var test = SQL.Customers;
                var customers = connection.Read<CustomerRecordWithOrders>("SELECT c.CustomerID, c.CompanyName, o.OrderId as Orders_OrderId, o.OrderDate as Orders_OrderDate FROM Customers c INNER JOIN Orders o ON o.CustomerId = c.CustomerId");
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
        public void ShouldReadCustomerByIdUsingArgumentsBuilder()
        {
            var args = new ArgumentsBuilder().Add("CustomerId", "ALFKI").Build();

            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    args);
                customers.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void ShouldReadCustomerByIdUsingArgumentsBuilderBasedUponExistingObject()
        {
            var args = new ArgumentsBuilder()
                .From(new { Country = "UK" })
                .Add("City", "London")
                .Build();

            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE Country = @Country AND City = @City",
                    args);
                customers.Count().ShouldBe(6);
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
        public void ShouldThrowExceptionWhenArgumentNotFound()
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
        public void ShouldInvokeCommandInitializerBeforeParsingQuery()
        {
            DbReaderOptions.CommandInitializer = command =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@CustomerId";
                parameter.Value = "ALFKI";
                command.Parameters.Add(parameter);
            };

            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId and CompanyName = @CompanyName", new { CompanyName = "Alfreds Futterkiste" });

                customers.Count().ShouldBe(1);
            }

            DbReaderOptions.CommandInitializer = null;
        }


        // [Fact]
        // public void ShouldReadEmployeeHierarchy()
        // {
        //     using (var connection = CreateConnection())
        //     {
        //         var employees = connection.Read<Employee>(SQL.EmployeesHierarchy).ToArray();
        //         Dictionary<long?, Employee> map = new Dictionary<long?, Employee>();
        //         foreach (var employee in employees)
        //         {
        //             if (employee.ReportsTo != null)
        //             {
        //                 map[employee.ReportsTo].Employees.Add(employee);
        //             }
        //             map.Add(employee.EmployeeId, employee);
        //         }

        //         var initialEmployee = map.First().Value;
        //         initialEmployee.Employees.Count().ShouldBe(5);
        //     }
        // }

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
        public async Task ShouldConvertScalarValueUsingValueConverter()
        {
            DbReaderOptions.WhenReading<CustomScalarValue>().Use((datarecord, i) => new CustomScalarValue(datarecord.GetInt64(i)));
            using (var connection = CreateConnection())
            {
                var scalar = await connection.ExecuteScalarAsync<CustomScalarValue>("SELECT COUNT(*) FROM Customers");
                scalar.Value.ShouldBe(93);
            }
        }

        [Fact]
        public async Task ShouldHandleNullScalarValueUsingValueConverter()
        {
            DbReaderOptions.WhenReading<CustomScalarValue?>().Use((datarecord, i) => new CustomScalarValue(datarecord.GetInt64(i)));
            using (var connection = CreateConnection())
            {
                var scalar = await connection.ExecuteScalarAsync<CustomScalarValue?>("SELECT NULL FROM Customers WHERE CustomerID = 'ALFKI'");
                scalar.ShouldBe(null);
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

        [Fact]
        public void ShouldNotSetParameterValueWhenPassingCustomType()
        {
            DbReaderOptions.WhenPassing<CustomerId>().Use((p, c) =>
                {
                    p.Value.ShouldBeNull();
                    p.Value = (string)c;
                });
            using (var connection = CreateConnection())
            {
                var customers = connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId",
                    new { CustomerId = new CustomerId("ALFKI") });
                customers.Count().ShouldBe(1);
            }
        }

        [Fact]
        public void ShouldReadSimpleType()
        {
            using (var connection = CreateConnection())
            {
                var customerIds = connection.Read<string>("SELECT CustomerID FROM Customers");
                customerIds.Count().ShouldBe(93);
            }
        }

        [Fact]
        public void ShouldReadSimpleTypeUsingValueConverter()
        {
            DbReaderOptions.WhenReading<CustomValueType>().Use((record, ordinal) => new CustomValueType(record.GetInt32(ordinal)));

            using (var connection = CreateConnection())
            {
                var orderIds = connection.Read<CustomValueType>("SELECT OrderId FROM Orders");
                orderIds.Count().ShouldBe(830);
            }
        }

        private string LoadSql(string name)
        {
            var provider = new SqlProvider();
            return provider.Load(name);
        }

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

        [Fact]
        public void ShouldGetOrdersWithCustomersTwice()
        {
            var sql = "SELECT o.OrderID as OrderWithCustomerId, c.CustomerID AS Customer_CustomerId FROM ORDERS AS o INNER JOIN Customers c on o.CustomerID = c.CustomerID";
            using (var connection = CreateConnection())
            {
                var firstOrders = connection.Read<OrderWithCustomer>(sql);
                var secondOrders = connection.Read<OrderWithCustomer>(sql);
            }
        }

        [Fact]
        public async Task ShouldNotCacheCollection()
        {
            using (var connection = CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    var customers = await connection.ReadAsync<CustomerWithOrders>("SELECT c.CustomerID as CustomerWithOrdersId, o.OrderId as O_OrderId, o.OrderDate as O_OrderDate FROM Customers c INNER JOIN Orders o ON c.CustomerId =o.CustomerId AND c.CustomerId = @CustomerId", new { CustomerId = "ALFKI" });
                    var firstCustomer = customers.First();
                    var orderDate = new DateTime(2020, 11, 18);
                    var firstCustomerId = firstCustomer.CustomerWithOrdersId;
                    int rowsAffected = await connection.ExecuteAsync("UPDATE Orders set OrderDate = @OrderDate WHERE CustomerID =@CustomerId", new { OrderDate = orderDate, CustomerId = firstCustomerId });
                    customers = await connection.ReadAsync<CustomerWithOrders>("SELECT c.CustomerID as CustomerWithOrdersId, o.OrderId as O_OrderId, o.OrderDate as O_OrderDate FROM Customers c INNER JOIN Orders o ON c.CustomerId =o.CustomerId AND c.CustomerId = @CustomerId", new { CustomerId = "ALFKI" });
                    customers.Single().Orders.ShouldAllBe(o => o.OrderDate == orderDate);
                    transaction.Rollback();
                }
            }
        }




        public class CustomerId
        {
            private string Value { get; set; }

            public CustomerId(string value)
            {
                Value = value;
            }

            public static implicit operator string(CustomerId value)
            {
                return value.Value;
            }

            public static implicit operator CustomerId(string value)
            {
                return new CustomerId(value);
            }
        }

    }

    public struct CustomScalarValue
    {
        public CustomScalarValue(long value)
        {
            Value = value;
        }

        public long Value { get; }
    }

    public record CustomerRecord(string CustomerId, string CompanyName)
    {
    }

    public record CustomerRecordWithOrders([property: Key] string CustomerId, string CompanyName, ICollection<OrderRecord> Orders)
    {
    }

    public record OrderRecord([property: Key] long OrderId, DateTime? OrderDate);
}
#endif