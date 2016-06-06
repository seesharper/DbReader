namespace DbReader.Tests
{
    using DbReader.Interfaces;
    using Shouldly;

    public class PrefixResolverTests
    {
        public void GetPrefix_NoPrefix_ReturnsEmptyPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");
            var dataRecord = new { OrderId = 42L }.ToDataRecord();
            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);
            prefix.ShouldBeEmpty();
        }

        public void GetPrefix_FullPrefix_ReturnsPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");
            var dataRecord = new { Orders_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBe("Orders");
        }

        public void GetPrefix_UpperCasePrefix_ReturnsPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");
            var dataRecord = new { O_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBe("O");
        }

        public void GetPrefix_ExistingPrefix_AppendsToExistingPrefix(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");
            var dataRecord = new { Customer_Orders_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "Customer");

            prefix.ShouldBe("Customer_Orders");
        }

        public void GetPrefix_UnknownPrefix_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");
            var dataRecord = new { UnknownPrefix_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        public void GetPrefix_UnknownField_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");

            var dataRecord = new { UnknownField = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        
        public void GetPrefix_UnknownFieldWithPrefix_ReturnsNull(IPrefixResolver prefixResolver)
        {
            var property = typeof(Customer).GetProperty("Orders");

            var dataRecord = new { Orders_UnknownField = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "Orders");

            prefix.ShouldBeNull();
        }
    }
}