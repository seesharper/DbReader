namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using DbReader.LightInject;

    using Should;
  
    using IPropertySelector = DbReader.Interfaces.IPropertySelector;

    public class SimplePropertySelectorTests
    {
        public void ShouldReturnSameInstance(Func<string, IPropertySelector> factory)
        {
            var firstInstance = factory("SimplePropertySelector");
            var secondInstance = factory("SimplePropertySelector");
            firstInstance.ShouldBeSameAs(secondInstance);
        }

        public void Execute_PublicWriteableProperty_ReturnsProperty(IPropertySelector simplePropertySelector)
        {
            simplePropertySelector.Execute(typeof(ClassWithPublicProperty)).ShouldNotBeEmpty();
        }

        public void Execute_NonPublicWriteableProperty_ReturnsEmptyList(IPropertySelector simplePropertySelector)
        {
            simplePropertySelector.Execute(typeof(ClassWithNonPublicProperty)).ShouldBeEmpty();
        }

        public void Execute_ReadOnlyProperty_ReturnsEmptyList(IPropertySelector simplePropertySelector)
        {
            simplePropertySelector.Execute(typeof(ClassWithPublicReadOnlyProperty)).ShouldBeEmpty();
        }

        public void Execute_StaticProperty_ReturnsEmptyList(IPropertySelector simplePropertySelector)
        {
            simplePropertySelector.Execute(typeof(ClassWithStaticProperty)).ShouldBeEmpty();
        }

        public void Execute_ComplexProperty_ReturnsEmptyList(IPropertySelector simplePropertySelector)
        {
            simplePropertySelector.Execute(typeof(ClassWithComplexProperty)).ShouldBeEmpty();
        }

        internal static void Configure(IServiceContainer container)
        {
            container.Register<Func<string, IPropertySelector>>((factory) => s => factory.GetInstance<IPropertySelector>(s));
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



    public class ClassWithProperty<T> : ClassWithId
    {
        public T Property { get; set; }
    }

    public class ClassWithDateTimeProperty : ClassWithId
    {
        public DateTime Property { get; set; }
    }

    public class ClassWithByteArrayProperty
    {
        public byte[] ByteArrayProperty { get; set; }
    }

}