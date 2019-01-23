namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Selectors;
    using Shouldly;
    using Xunit;

    public class OneToManyPropertySelectorTests : ContainerFixture
    {
        public IPropertySelector oneToManyPropertySelector;


        [Fact]
        public void ShouldAllowIEnumerable()
        {
            var properties = oneToManyPropertySelector.Execute(typeof (ClassWithProperty<IEnumerable<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        [Fact]
        public void ShouldAllowICollection()
        {
            var properties = oneToManyPropertySelector.Execute(typeof(ClassWithProperty<ICollection<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        [Fact]
        public void ShouldAllowCollection()
        {
            var properties = oneToManyPropertySelector.Execute(typeof(ClassWithProperty<Collection<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        [Fact]
        public void ShotNotAllowReadOnlyCollection()
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<SampleClass[]>)));
            exception.Message.ShouldStartWith("The navigation property (one-to-many)");
        }

        [Fact]
        public void ShouldNotAllowNonGenericCollection()
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<SampleClassCollection>)));
            exception.Message.ShouldStartWith("The navigation property (one-to-many)");
        }

        [Fact]
        public void ShouldNotAllowSimpleTypesAsProjectionType()
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<IEnumerable<int>>)));
            exception.Message.ShouldContain("Simple types such as string and int");
        }
    }
}