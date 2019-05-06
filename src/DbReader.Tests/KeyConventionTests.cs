using Shouldly;
using Xunit;

namespace DbReader.Tests
{
    public class KeyConventionTests
    {
        [Fact]
        public void ShouldUseClassIdProperty()
        {
            var result = DbReaderOptions.KeyConvention(typeof(Customer).GetProperty(nameof(Customer.CustomerId)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseIdProperty()
        {
            var result = DbReaderOptions.KeyConvention(typeof(Employee).GetProperty(nameof(Employee.Id)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseCustomIdIntProperty()
        {
            DbReaderOptions.KeySelector<AnotherCustomer>(ac => ac.CustomId);
            var result = DbReaderOptions.KeyConvention(typeof(AnotherCustomer).GetProperty(nameof(AnotherCustomer.CustomId)));
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldUseCustomIdStringProperty()
        {
            DbReaderOptions.KeySelector<AnotherEmployee>(ac => ac.CustomId);
            var result = DbReaderOptions.KeyConvention(typeof(AnotherEmployee).GetProperty(nameof(AnotherEmployee.CustomId)));
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