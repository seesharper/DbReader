namespace DbReader.Tests
{
    using System;

    using DbReader.Interfaces;
    using Mapping;
    using Shouldly;


    public class KeyPropertyMapperTests
    {
        public void Execute_ClassWithIdProperty_ReturnsMapping(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty);
            result.Length.ShouldBe(1);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void Execute_ClassWithTypeNamePrefixedIdProperty_ReturnsMapping(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { ClassWithTypeNamePrefixedIdPropertyId = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithTypeNamePrefixedIdProperty), dataRecord, string.Empty);
            result.Length.ShouldBe(1);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        public void Execute_UnMappedKeyProperty_ThrowsException(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { InvalidField = 42 }.ToDataRecord();
            Should.Throw<InvalidOperationException>(
                () => keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty));
        }

        public void Execute_MissingKeyProperty_ThrowsException(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { InvalidField = 42 }.ToDataRecord();
            Should.Throw<InvalidOperationException>(
                () => keyPropertyMapper.Execute(typeof(ClassWithoutKeyProperty), dataRecord, string.Empty));
        }
    }


    public class ClassWithIdProperty
    {
        public int Id { get; set; }
    }

    public class ClassWithTypeNamePrefixedIdProperty
    {
        public int ClassWithTypeNamePrefixedIdPropertyId { get; set; }
    }

    public class ClassWithoutKeyProperty
    {
        public string Name { get; set; }
    }
}