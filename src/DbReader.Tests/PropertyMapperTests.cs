namespace DbReader.Tests
{
    using System;
    using Mapping;
    using Shouldly;

    public class PropertyMapperTests
    {

        static PropertyMapperTests()
        {
            ValueConverter.RegisterReadDelegate((record, i) => new CustomValueType(record.GetInt32(i)));
        }

        public void ShouldMapMatchingField(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void ShouldNotMapUnknownField(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { UnknownProperty = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(-1);
        }

        public void ShouldMapFieldWithPrefix(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { SomePrefix_Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, "SomePrefix");

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void ShouldMapAlreadyMappedProperty(IPropertyMapper propertyMapper)
        {           
            var dataRecord = new { Id = 42 }.ToDataRecord();

            propertyMapper.Execute(typeof(ClassWithId), dataRecord, string.Empty);
            var result = propertyMapper.Execute(typeof(AnotherClassWithId), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void ShouldThrowExceptionWhenTypesDoesNotMatch(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Property = "SomeValue" }.ToDataRecord();
            Should.Throw<InvalidOperationException>(() => propertyMapper.Execute(typeof (ClassWithProperty<int>), dataRecord, string.Empty));            
        }

        public void ShouldMapPropertiesWithCustomConversion(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Property = 42 }.ToDataRecord();
            var result = propertyMapper.Execute(typeof (ClassWithProperty<CustomValueType>), dataRecord, string.Empty);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void ShouldProvideMeaningfulStringRepresentation(IPropertyMapper propertyMapper)
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