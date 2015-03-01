namespace DbReader.Tests
{
    using DbReader.Interfaces;

    using Should;

    public class ManyToOneMethodBuilderTests
    {
        [ScopedTheory, InjectData]
        public void ShouldReturnSameInstanceWithinScope(IManyToOneMethodBuilder<SampleClass> first, IManyToOneMethodBuilder<SampleClass> second)
        {
            first.ShouldBeSameAs(second);
        }

        [ScopedTheory, InjectData]
        public void Test(IManyToOneMethodBuilder<Order> manyToOneMethodBuilder)
        {
            var dataRecord = new { CustomerId = "ALFKI" }.ToDataRecord();
            var method = manyToOneMethodBuilder.CreateMethod(dataRecord, string.Empty);
            method(dataRecord, new Order());
        }
    }
}