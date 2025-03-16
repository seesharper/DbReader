namespace DbClient.Tests
{
    using System;
    using DbClient.Interfaces;
    using Mapping;
    using Shouldly;
    using Xunit;

    public class KeyPropertyMapperTests : ContainerFixture
    {
        public readonly IKeyPropertyMapper keyPropertyMapper;

        [Fact]
        public void Execute_ClassWithIdProperty_ReturnsMapping()
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty);
            result.Length.ShouldBe(1);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void Execute_ClassWithTypeNamePrefixedIdProperty_ReturnsMapping()
        {
            var dataRecord = new { ClassWithTypeNamePrefixedIdPropertyId = 42 }.ToDataRecord();
            var result = keyPropertyMapper.Execute(typeof(ClassWithTypeNamePrefixedIdProperty), dataRecord, string.Empty);
            result.Length.ShouldBe(1);
            result[0].ColumnInfo.Ordinal.ShouldBe(0);
        }

        [Fact]
        public void Execute_UnMappedKeyProperty_ThrowsException()
        {
            var dataRecord = new { InvalidField = 42 }.ToDataRecord();
            var exception = Should.Throw<InvalidOperationException>(
                () => keyPropertyMapper.Execute(typeof(ClassWithIdProperty), dataRecord, string.Empty));
            exception.Message.ShouldContain("ClassWithIdProperty.Id");
        }

        [Fact]
        public void Execute_MissingKeyProperty_ThrowsException()
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