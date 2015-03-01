namespace DbReader.Tests
{
    using DbReader.Interfaces;

    using Should;

    using Xunit.Extensions;

    public class PrefixResolverTests
    {
        [ScopedTheory, InjectData]
        public void GetPrefix_NoPrefix_ReturnsEmptyPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");
            var dataRecord = new { CustomerId = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeEmpty();
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_FullPrefix_ReturnsPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");
            var dataRecord = new { Customer_CustomerId = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldEqual("Customer");
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_UpperCasePrefix_ReturnsPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");
            var dataRecord = new { C_CustomerId = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldEqual("C");
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_ExistingPrefix_AppendsToExistingPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");
            var dataRecord = new { Order_Customer_CustomerId = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "Order");

            prefix.ShouldEqual("Order_Customer");
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_UnknownPrefix_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");
            var dataRecord = new { UnknownPrefix_CustomerId = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_UnknownField_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");

            var dataRecord = new { UnknownField = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        [ScopedTheory, InjectData]
        public void GetPrefix_UnknownFieldWithPrefix_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Order).GetProperty("Customer");

            var dataRecord = new { Customer_UnknownField = "ALFKI" }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "Customer");

            prefix.ShouldBeNull();
        }
    }
}