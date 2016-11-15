namespace DbReader.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Database;
    using Extensions;
    using Selectors;
    using Shouldly;

    public class PropertyReflectionTests
    {
        public void ShouldConsiderPublicInstancePropertyWithSetterAsWriteable()
        {
            typeof (ClassWithProperty<int>).GetProperty("Property")
                .IsWriteable().ShouldBeTrue();
        }

        public void ShouldNotConsiderPublicStaticPropertyWithSetterAsWriteable()
        {
            typeof (ClassWithStaticProperty).GetProperty("StaticProperty")
                .IsWriteable().ShouldBeFalse();
        }

        public void ShouldConsiderPublicInstancePropertyWithGetterAsReadable()
        {
            typeof(ClassWithProperty<int>).GetProperty("Property")
                .IsReadable().ShouldBeTrue();
        }

        public void ShouldNotConsiderPublicStaticPropertyWithGetterAsWriteable()
        {
            typeof(ClassWithStaticProperty).GetProperty("StaticProperty")
                .IsReadable().ShouldBeFalse();
        }


        public void OrderByDeclaration_InOrder_ReturnsPropertiesInOrder()
        {
            var type = new {A = 1, B = 2}.GetType();
            var properties = new []{type.GetProperty("A"), type.GetProperty("B")};

            var orderedProperties = properties.OrderByDeclaration();

            orderedProperties.SequenceEqual(properties).ShouldBeTrue();
        }

        public void OrderByDeclaration_OutOfOrder_ReturnsPropertiesInOrder()
        {
            var type = new { A = 1, B = 2 }.GetType();
            var properties = new[] { type.GetProperty("A"), type.GetProperty("B") };

            var orderedProperties = properties.Reverse().OrderByDeclaration();

            orderedProperties.SequenceEqual(properties).ShouldBeTrue();
        }

    }
}