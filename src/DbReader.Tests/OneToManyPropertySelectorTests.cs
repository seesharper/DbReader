namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Selectors;
    using Shouldly;

    public class OneToManyPropertySelectorTests
    {
        public void ShouldAllowIEnumerable(IPropertySelector oneToManyPropertySelector)
        {
            var properties = oneToManyPropertySelector.Execute(typeof (ClassWithProperty<IEnumerable<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        public void ShouldAllowICollection(IPropertySelector oneToManyPropertySelector)
        {
            var properties = oneToManyPropertySelector.Execute(typeof(ClassWithProperty<ICollection<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        public void ShouldAllowCollection(IPropertySelector oneToManyPropertySelector)
        {
            var properties = oneToManyPropertySelector.Execute(typeof(ClassWithProperty<Collection<SampleClass>>));
            properties.ShouldNotBeEmpty();
        }

        public void ShotNotAllowReadOnlyCollection(IPropertySelector oneToManyPropertySelector)
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<SampleClass[]>)));
            exception.Message.ShouldStartWith("The navigation property (one-to-many)");
        }

        public void ShouldNotAllowNonGenericCollection(IPropertySelector oneToManyPropertySelector)
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<SampleClassCollection>)));
            exception.Message.ShouldStartWith("The navigation property (one-to-many)");
        }

        public void ShouldNotAllowSimpleTypesAsProjectionType(IPropertySelector oneToManyPropertySelector)
        {
            var exception = Should.Throw<InvalidOperationException>(() => oneToManyPropertySelector.Execute(typeof(ClassWithProperty<IEnumerable<int>>)));
            exception.Message.ShouldContain("Simple types such as string and int");
        }
    }
}