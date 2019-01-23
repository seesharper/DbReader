namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbReader.Interfaces;
    using Selectors;
    using Shouldly;
    using Xunit;

    public class FieldSelectorTests : ContainerFixture
    {

        public readonly IFieldSelector fieldSelector;

        [Fact]
        public void ShouldReturnSameInstanceWithinScope()
        {
            var first = GetInstance<IFieldSelector>();
            var second = GetInstance<IFieldSelector>();
            first.ShouldBeSameAs(second);
        }

         [Fact]
        public void Execute_ValidColumnns_ReturnsDictionary()
        {
            IDataRecord dataRecord = new { SomeColumn = 42 }.ToDataRecord();
            IReadOnlyDictionary<string, ColumnInfo> result = fieldSelector.Execute(dataRecord);
            result["SomeColumn"].Ordinal.ShouldBe(0);
        }

         [Fact]
        public void Execute_DuplicateFieldNames_ThrowsArgumentOutOfRangeException()
        {
            IDataRecord dataRecord = new { SomeColumn = 42, somecolumn = 42 }.ToDataRecord();
            Should.Throw<ArgumentOutOfRangeException>(() => fieldSelector.Execute(dataRecord));
        }
    }
}