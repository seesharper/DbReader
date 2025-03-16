using Shouldly;
using Xunit;

namespace DbClient.Tests
{
    public class KeyConventionTests
    {
        [Fact]
        public void ShouldUseClassIdProperty()
        {
            var result = DbClientOptions.KeyConvention(typeof(Customer).GetProperty(nameof(Customer.CustomerId)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseIdProperty()
        {
            var result = DbClientOptions.KeyConvention(typeof(Employee).GetProperty(nameof(Employee.Id)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseCustomIdIntProperty()
        {
            DbClientOptions.KeySelector<AnotherCustomer>(ac => ac.CustomId);
            var result = DbClientOptions.KeyConvention(typeof(AnotherCustomer).GetProperty(nameof(AnotherCustomer.CustomId)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseCustomIdStringProperty()
        {
            DbClientOptions.KeySelector<AnotherEmployee>(ac => ac.CustomId);
            var result = DbClientOptions.KeyConvention(typeof(AnotherEmployee).GetProperty(nameof(AnotherEmployee.CustomId)));
            result.ShouldBeTrue();
        }

        public class AnotherCustomer
        {
            public string CustomId { get; set; }
        }

        public class Customer
        {
            public int CustomerId { get; set; }
        }

        public class Employee
        {
            public int Id { get; set; }
        }

        public class AnotherEmployee
        {
            public string CustomId { get; set; }
        }
    }



}