namespace DbReader.Tests
{
    using System;
    using Mapping;
    using Shouldly;
    using Xunit;

    public class PropertyMapperTests : ContainerFixture
    {

        public readonly IPropertyMapper propertyMapper;

        static PropertyMapperTests()
        {
            ValueConverter.RegisterReadDelegate((record, i) => new CustomValueType(record.GetInt32(i)));
        }

        [Fact]
        public void ShouldMapMatchingField()
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void ShouldMapMatchingFieldUsingUpperCaseLetters()
        {
            var dataRecord = new { I32P = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void ShouldNotMapUnknownField()
        {
            var dataRecord = new { UnknownProperty = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(-1);
            result[0].ColumnInfo.ToString().ShouldBe("Not Mapped");
        }

        [Fact]
        public void ShouldMapFieldWithPrefix()
        {
            var dataRecord = new { SomePrefix_Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, "SomePrefix");

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void ShouldMapAlreadyMappedProperty()
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();

            propertyMapper.Execute(typeof(ClassWithId), dataRecord, string.Empty);
            var result = propertyMapper.Execute(typeof(AnotherClassWithId), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void ShouldThrowExceptionWhenTypesDoesNotMatch()
        {
            var dataRecord = new { Property = "SomeValue" }.ToDataRecord();
            Should.Throw<InvalidOperationException>(() => propertyMapper.Execute(typeof(ClassWithProperty<int>), dataRecord, string.Empty));
        }

        [Fact]
        public void ShouldMapPropertiesWithCustomConversion()
        {
            var dataRecord = new { Property = 42 }.ToDataRecord();
            var result = propertyMapper.Execute(typeof(ClassWithProperty<CustomValueType>), dataRecord, string.Empty);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void ShouldProvideMeaningfulStringRepresentation()
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ToString().ShouldBe("[DbReader.Tests.SampleClass] Property: Int32 Int32Property, Ordinal: 0");
        }
    }

    public class ClassWithCustomValueType
    {
        public CustomValueType CustomValueTypeProperty { get; set; }
    }
}