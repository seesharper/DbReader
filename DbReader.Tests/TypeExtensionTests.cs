namespace DbReader.Tests
{
    using System;

    using Should;

    using Xunit;

    public class TypeExtensionTests
    {
        [Fact]
        public void GetUnderlyingType_Enum_ReturnsUnderlyingType()
        {
            typeof(StringComparison).GetUnderlyingType().ShouldEqual(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_Nullable_ReturnsUnderlyingType()
        {
            typeof(int?).GetUnderlyingType().ShouldEqual(typeof(int));
        }

        public void GetUnderlyingType_NonNullable_ReturnsType()
        {
            typeof(int).GetUnderlyingType().ShouldEqual(typeof(int));
        }

        [Fact]
        public void IsNullable_NullableType_ReturnsTrue()
        {
            typeof(bool?).IsNullable().ShouldBeTrue();
        }

        [Fact]
        public void IsNullable_NonNullableType_ReturnsFalse()
        {
            typeof(bool).IsNullable().ShouldBeFalse();
        }

        [Fact]
        public void IsSimpleType_PrimitiveType_ReturnsTrue()
        {
            typeof(int).IsSimpleType().ShouldBeTrue();
        }

        [Fact]
        public void IsSimpleType_Enum_ReturnsTrue()
        {
            typeof(StringComparison).IsSimpleType().ShouldBeTrue();
        }

        [Fact]
        public void IsSimpleType_NonPrimitive_ReturnsTrue()
        {
            typeof(string).IsSimpleType().ShouldBeTrue();
        }

        [Fact]
        public void IsSimpleType_CustomType_ReturnsTrue()
        {
            ValueConverter.Register((record, i) => new CustomValueType(42));
            typeof(CustomValueType).IsSimpleType().ShouldBeTrue();
        }

        [Fact]
        public void IsSimpleType_ComplexType_ReturnsFalse()
        {
            typeof(TypeExtensionTests).IsSimpleType().ShouldBeFalse();
        }

        [Fact]
        public void IsSimpleType_Nullable_ReturnsTrue()
        {
            typeof(int?).IsSimpleType().ShouldBeTrue();
        }

       
    }
}