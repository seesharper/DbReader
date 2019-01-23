// namespace DbReader.Tests
// {
//    using DbReader.Interfaces;
//     using DbReader.LightInject;
//     using DbReader.Tests.LightInject.xUnit2;
//     using Shouldly  ;
//     using Xunit;
//     using Xunit.Extensions;

//    public class ManyToOnePropertySelectorTests
//    {
//        [Theory, Scoped, InjectData]
//        public void Execute_NonEnumerableProperty_ReturnsProperty(IPropertySelector manyToOnePropertySelector)
//        {
//            manyToOnePropertySelector.Execute(typeof(ClassWithComplexProperty)).ShouldNotBeEmpty();
//        }

//        [Theory, InjectData]
//        public void Execute_SimpleProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
//        {
//            manyToOnePropertySelector.Execute(typeof(ClassWithPublicProperty)).ShouldBeEmpty();
//        }

//        [Theory, InjectData]
//        public void Execute_EnumerableProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
//        {
//            manyToOnePropertySelector.Execute(typeof(ClassWithEnumerableProperty)).ShouldBeEmpty();
//        }

//        [Theory, InjectData]
//        public void Execute_StringProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
//        {
//            manyToOnePropertySelector.Execute(typeof(ClassWithStringProperty)).ShouldBeEmpty();
//        }

//        [Theory, InjectData]
//        public void Execute_ByteArrayProperty_ReturnsEmptyList(IPropertySelector manyToOnePropertySelector)
//        {
//            manyToOnePropertySelector.Execute(typeof(ClassWithByteArrayProperty)).ShouldBeEmpty();
//        }

//    }
// }