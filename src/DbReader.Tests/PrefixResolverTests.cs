namespace DbReader.Tests
{
    using Construction;
    using DbReader.Interfaces;
    using Shouldly;
    using Xunit;

    public class PrefixResolverTests : ContainerFixture
    {

        public readonly IPrefixResolver prefixResolver;

        [Fact]
        public void GetPrefix_NoPrefix_Null()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");
            var dataRecord = new { OrderId = 42L }.ToDataRecord();
            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);
            prefix.ShouldBeNull();
        }

        [Fact]
        public void GetPrefix_FullPrefix_ReturnsPrefix()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");
            var dataRecord = new { Orders_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBe("Orders");
        }

        [Fact]
        public void GetPrefix_UpperCasePrefix_ReturnsPrefix()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");
            var dataRecord = new { O_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBe("O");
        }

        [Fact]
        public void GetPrefix_ExistingPrefix_AppendsToExistingPrefix()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");
            var dataRecord = new { CustomerWithOrders_Orders_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "CustomerWithOrders");

            prefix.ShouldBe("CustomerWithOrders_Orders");
        }

        [Fact]
        public void GetPrefix_UnknownPrefix_ReturnsNull()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");
            var dataRecord = new { UnknownPrefix_OrderId = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        [Fact]
        public void GetPrefix_UnknownField_ReturnsNull()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");

            var dataRecord = new { UnknownField = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, string.Empty);

            prefix.ShouldBeNull();
        }

        [Fact]
        public void GetPrefix_UnknownFieldWithPrefix_ReturnsNull()
        {
            var property = typeof(CustomerWithOrders).GetProperty("Orders");

            var dataRecord = new { Orders_UnknownField = 42L }.ToDataRecord();

            var prefix = prefixResolver.GetPrefix(property, dataRecord, "Orders");

            prefix.ShouldBeNull();
        }
    }
}