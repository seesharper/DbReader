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
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 1 ,Orders_ShippingAddress = "John Dear Road 10" },
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 2 ,Orders_ShippingAddress = "John Dear Road 20" }
            };
            var dataReader = rows.ToDataReader();

            var customers = dataReader.Read<CustomerWithOrders>();

            rows = new[]
            {
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 1 ,Orders_ShippingAddress = "John Dear Road 30" },
                new { CustomerWithOrdersId = "ALFKI", Name = "Alfreds Futterkiste" , Orders_OrderId = 2 ,Orders_ShippingAddress = "John Dear Road 30" }
            };

            dataReader = rows.ToDataReader();

            customers = dataReader.Read<CustomerWithOrders>();

            customers.Single().Orders.ShouldAllBe(o => o.ShippingAddress == "John Dear Road 30");
        }

        [Fact]
        public void ShouldNotCacheAcrossReadsUsingManyToOneRelation()
        {
            var rows = new[]
            {
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Alfreds Futterkiste" , OrderWithCustomerId = 1 ,ShippingAddress = "John Dear Road 10" },
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Alfreds Futterkiste" , OrderWithCustomerId = 2 ,ShippingAddress = "John Dear Road 20" }
            };
            var dataReader = rows.ToDataReader();

            dataReader.Read<OrderWithCustomer>();

            rows = new[]
             {
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Fred Futterkiste" , OrderWithCustomerId = 1 ,ShippingAddress = "John Dear Road 10" },
                new { Customer_CustomerId = "ALFKI", Customer_Name = "Fred Futterkiste" , OrderWithCustomerId = 2 ,ShippingAddress = "John Dear Road 20" }
            };

            dataReader = rows.ToDataReader();

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