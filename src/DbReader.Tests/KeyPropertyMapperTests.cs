namespace DbReader.Tests
{
    using System;

    using DbReader.Interfaces;

    using Should;
    using Should.Core.Assertions;

    public class KeyPropertyMapperTests
    {
        public void Execute_ClassWithIdProperty_ReturnsMapping(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty);
            result.Length.ShouldEqual(1);
            result[0].ColumnInfo.Ordinal.ShouldEqual(0);
        }

        public void Execute_ClassWithTypeNamePrefixedIdProperty_ReturnsMapping(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { ClassWithTypeNamePrefixedIdPropertyId = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithTypeNamePrefixedIdProperty), dataRecord, string.Empty);
            result.Length.ShouldEqual(1);
            result[0].ColumnInfo.Ordinal.ShouldEqual(0);
        }

        public void Execute_UnMappedKeyProperty_ThrowsException(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { InvalidField = 42 }.ToDataRecord();
            Assert.Throws<InvalidOperationException>(
                () => keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty));
        }

        public void Execute_MissingKeyProperty_ThrowsException(IPropertyMapper keyPropertyMapper)
        {
            var dataRecord = new { InvalidField = 42 }.ToDataRecord();
            Assert.Throws<InvalidOperationException>(
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