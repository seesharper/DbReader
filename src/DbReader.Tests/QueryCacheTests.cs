using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Shouldly;
using System.Linq;

namespace DbReader.Tests
{
    public class QueryCacheTests
    {
        [Fact]
        public void ShouldNotCacheAcrossReadsUsingOneToManyRelation()
        {
            var rows = new[]
            {
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 1 ,Orders_ShippingAddress = "Address 1" },
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 2 ,Orders_ShippingAddress = "Address 2" }
            };
            var dataReader = rows.ToDataReader();
            SqlStatement.Current = "TEST-STATEMENT";
            var customers = dataReader.Read<CustomerWithOrders>();

            rows = new[]
            {
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 1 ,Orders_ShippingAddress = "Address 3" },
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 2 ,Orders_ShippingAddress = "Address 3" }
            };

            dataReader = rows.ToDataReader();
            SqlStatement.Current = "TEST-STATEMENT";
            customers = dataReader.Read<CustomerWithOrders>();

            customers.Single().Orders.ShouldAllBe(o => o.ShippingAddress == "Address 3");
        }

        [Fact]
        public void ShouldNotCacheAcrossReadsUsingManyToOneRelation()
        {
            var rows = new[]
            {
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Alfreds Futterkiste" , OrderWithCustomerId = 1 ,ShippingAddress = "Address 1" },
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Alfreds Futterkiste" , OrderWithCustomerId = 2 ,ShippingAddress = "Address 2" }
            };
            var dataReader = rows.ToDataReader();
            SqlStatement.Current = "TEST-STATEMENT";
            dataReader.Read<OrderWithCustomer>();

            rows = new[]
             {
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Fred Futterkiste" , OrderWithCustomerId = 1 ,ShippingAddress = "Address 1" },
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Fred Futterkiste" , OrderWithCustomerId = 2 ,ShippingAddress = "Address 2" }
            };

            dataReader = rows.ToDataReader();
            SqlStatement.Current = "TEST-STATEMENT";
            var orders = dataReader.Read<OrderWithCustomer>();

            orders.First().Customer.Name.ShouldBe("Fred Futterkiste");
        }



        public class CustomerWithOrders
        {
            [Key]
            public string CustomerWithOrdersId { get; set; }

            public string Name { get; set; }
            public ICollection<Order> Orders { get; set; }
        }


        public class Order
        {
            [Key]
            public int OrderId { get; set; }

            public string ShippingAddress { get; set; }
        }

        public class Customer
        {
            [Key]
            public string CustomerId { get; set; }

            public string Name { get; set; }

        }

        public class OrderWithCustomer
        {
            [Key]
            public int OrderWithCustomerId { get; set; }

            public Customer Customer { get; set; }
        }
    }
}