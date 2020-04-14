namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using Extensions;
    using Shouldly;
    using Xunit;

    public class TypeExtensionTests
    {
        [Fact]
        public void GetUnderlyingType_Enum_ReturnsUnderlyingType()
        {
            typeof(StringComparison).GetUnderlyingType().ShouldBe(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_Nullable_ReturnsUnderlyingType()
        {
            typeof(int?).GetUnderlyingType().ShouldBe(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_NonNullable_ReturnsType()
        {
            typeof(int).GetUnderlyingType().ShouldBe(typeof(int));
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
            ValueConverter.RegisterReadDelegate((record, i) => new CustomValueType(42));
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

        [Fact]
        public void IsEnumerable_IEnumerable_ReturnsTrue()
        {
            typeof(IEnumerable<int>).IsEnumerable().ShouldBeTrue();
        }

        [Fact]
        public void IsEnumerable_CollectionTypeImplementingIEnumerable_ReturnsTrue()
        {
            typeof(Collection<int>).IsEnumerable().ShouldBeTrue();
        }
    }
}