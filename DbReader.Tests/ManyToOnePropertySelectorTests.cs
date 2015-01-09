namespace DbReader.Tests
{
    using Should;

    using Xunit.Extensions;

    public class ManyToOnePropertySelectorTests
    {
        [Theory, InjectData]
        public void Execute_NonEnumerableProperty_ReturnsProperty(IPropertySelector manyToOnePropertySelector)
        {
            manyToOnePropertySelector.Execute(typeof(ClassWithComplexProperty)).ShouldNotBeEmpty();
        }
        
        [Theory, InjectData]
        public void Execute_SimpleProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
        {
            manyToOnePropertySelector.Execute(typeof(ClassWithPublicProperty)).ShouldBeEmpty();
        }

        [Theory, InjectData]
        public void Execute_EnumerableProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
        {
            manyToOnePropertySelector.Execute(typeof(ClassWithEnumerableProperty)).ShouldBeEmpty();
        }

        
    }
}