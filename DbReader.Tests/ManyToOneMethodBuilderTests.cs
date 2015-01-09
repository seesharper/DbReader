namespace DbReader.Tests
{
    using Should;

    public class ManyToOneMethodBuilderTests
    {
        [ScopedTheory, InjectData]
        public void ShouldReturnSameInstanceWithinScope(IRelationMethodBuilder<SampleClass> first, IRelationMethodBuilder<SampleClass> second)
        {
            first.ShouldBeSameAs(second);
        }
    }
}