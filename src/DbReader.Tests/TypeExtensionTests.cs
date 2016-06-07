namespace DbReader.Tests
{
    using System;
    using Extensions;
    using Shouldly;

    public class TypeExtensionTests
    {
        public void GetUnderlyingType_Enum_ReturnsUnderlyingType()
        {
            typeof(StringComparison).GetUnderlyingType().ShouldBe(typeof(int));
        }

        public void GetUnderlyingType_Nullable_ReturnsUnderlyingType()
        {
            typeof(int?).GetUnderlyingType().ShouldBe(typeof(int));
        }

        public void GetUnderlyingType_NonNullable_ReturnsType()
        {
            typeof(int).GetUnderlyingType().ShouldBe(typeof(int));
        }

        public void IsNullable_NullableType_ReturnsTrue()
        {
            typeof(bool?).IsNullable().ShouldBeTrue();
        }

        public void IsNullable_NonNullableType_ReturnsFalse()
        {
            typeof(bool).IsNullable().ShouldBeFalse();
        }

        public void IsSimpleType_PrimitiveType_ReturnsTrue()
        {
            typeof(int).IsSimpleType().ShouldBeTrue();
        }

        public void IsSimpleType_Enum_ReturnsTrue()
        {
            typeof(StringComparison).IsSimpleType().ShouldBeTrue();
        }

        public void IsSimpleType_NonPrimitive_ReturnsTrue()
        {
            typeof(string).IsSimpleType().ShouldBeTrue();
        }

        public void IsSimpleType_CustomType_ReturnsTrue()
        {
            ValueConverter.RegisterReadDelegate((record, i) => new CustomValueType(42));
            typeof(CustomValueType).IsSimpleType().ShouldBeTrue();
        }

        public void IsSimpleType_ComplexType_ReturnsFalse()
        {
            typeof(TypeExtensionTests).IsSimpleType().ShouldBeFalse();
        }

        public void IsSimpleType_Nullable_ReturnsTrue()
        {
            typeof(int?).IsSimpleType().ShouldBeTrue();
        }
    }
}