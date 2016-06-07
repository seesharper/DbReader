namespace DbReader.Tests
{
    using System;
    using DbReader.Interfaces;
    using Mapping;
    using Shouldly;

    public class PropertyMapperTests
    {

        static PropertyMapperTests()
        {
            ValueConverter.RegisterReadDelegate((record, i) => new CustomValueType(record.GetInt32(i)));
        }

        public void Execute_MatchingField_ReturnsPositiveOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void Execute_NonMatchingField_ReturnsNegativeOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { UnknownProperty = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(-1);
        }

        public void Execute_MatchingFieldWithPrefix_ReturnsPositiveOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { SomePrefix_Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, "SomePrefix");

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void Execute_Twice_ReturnsNegativeOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);
            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(-1);
        }

        public void Execute_NonMatchingTypes_ThrowsException(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { CustomValueTypeProperty = "SomeValue" }.ToDataRecord();
            Should.Throw<InvalidOperationException>(() => propertyMapper.Execute(typeof (ClassWithCustomValueType), dataRecord, string.Empty));            
        }

        public void Execute_NonMatchingTypesWithCustomConversion_ReturnsOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { CustomValueTypeProperty = 42 }.ToDataRecord();
            var result = propertyMapper.Execute(typeof (ClassWithCustomValueType), dataRecord, string.Empty);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

    }

    public class ClassWithCustomValueType
    {
        public CustomValueType CustomValueTypeProperty { get; set; }
    }
}