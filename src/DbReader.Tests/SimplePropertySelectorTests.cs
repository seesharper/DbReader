namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DbReader.LightInject;
    using Shouldly;
    using Xunit;
    using IPropertySelector = Selectors.IPropertySelector;


    public class SimplePropertySelectorTests : ContainerFixture
    {
        public readonly IPropertySelector simplePropertySelector;

        public void ShouldReturnSameInstance()
        {
            var firstInstance = ServiceFactory.GetInstance<IPropertySelector>("SimplePropertySelector");
            var secondInstance = ServiceFactory.GetInstance<IPropertySelector>("SimplePropertySelector");
            firstInstance.ShouldBeSameAs(secondInstance);
        }

        [Fact]
        public void Execute_PublicWriteableProperty_ReturnsProperty()
        {
            simplePropertySelector.Execute(typeof(ClassWithPublicProperty)).ShouldNotBeEmpty();
        }

        [Fact]
        public void Execute_NonPublicWriteableProperty_ReturnsEmptyList()
        {
            simplePropertySelector.Execute(typeof(ClassWithNonPublicProperty)).ShouldBeEmpty();
        }

        [Fact]
        public void Execute_ReadOnlyProperty_ReturnsEmptyList()
        {
            simplePropertySelector.Execute(typeof(ClassWithPublicReadOnlyProperty)).ShouldBeEmpty();
        }

        [Fact]
        public void Execute_StaticProperty_ReturnsEmptyList()
        {
            simplePropertySelector.Execute(typeof(ClassWithStaticProperty)).ShouldBeEmpty();
        }

        [Fact]
        public void Execute_ComplexProperty_ReturnsEmptyList()
        {
            simplePropertySelector.Execute(typeof(ClassWithComplexProperty)).ShouldBeEmpty();
        }
    }

    public class ClassWithPublicProperty
    {
        public int PublicProperty { get; set; }
    }

    public class ClassWithNonPublicProperty
    {
        internal int InternalProperty { get; set; }
    }

    public class ClassWithPublicReadOnlyProperty
    {
        public int ReadOnlyProperty { get; private set; }
    }

    public class ClassWithStaticProperty
    {
        public static int StaticProperty { get; set; }
    }

    public class ClassWithComplexProperty
    {
        public StringBuilder ComplexProperty { get; set; }
    }

    public class ClassWithEnumerableProperty
    {
        public IEnumerable<ClassWithPublicProperty> EnumerableProperty { get; set; }
    }

    public class ClassWithStringProperty : ClassWithId
    {
        public string Property { get; set; }
    }

    public class ClassWithId
    {
        public int Id { get; set; }
    }

    public class AnotherClassWithId
    {
        public int Id { get; set; }
    }


    public class ClassWithProperty<T> : ClassWithId
    {
        public T Property { get; set; }
    }

    public class ClassWithTwoProperties<T1, T2> : ClassWithId
    {
        public T1 FirstProperty { get; set; }

        public T2 SecondProperty { get; set; }
    }

    public class ClassWithDateTimeProperty : ClassWithId
    {
        public DateTime Property { get; set; }
    }

    public class ClassWithByteArrayProperty
    {
        public byte[] ByteArrayProperty { get; set; }
    }


    public class GuruMeditationException : Exception
    {
    }
}