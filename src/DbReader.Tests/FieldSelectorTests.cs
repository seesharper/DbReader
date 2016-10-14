namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbReader.Interfaces;
    using Selectors;
    using Shouldly;


    public class FieldSelectorTests
    {
       
        public void ShouldReturnSameInstanceWithinScope(Func<IFieldSelector> factory)
        {
            var first = factory();
            var second = factory();
            first.ShouldBeSameAs(second);
        }

       
       
        public void Execute_ValidColumnns_ReturnsDictionary(IFieldSelector fieldSelector)
        {
            IDataRecord dataRecord = new { SomeColumn = 42 }.ToDataRecord();
            IReadOnlyDictionary<string, ColumnInfo> result = fieldSelector.Execute(dataRecord);
            result["SomeColumn"].Ordinal.ShouldBe(0);
        }
        
        public void Execute_DuplicateFieldNames_ThrowsArgumentOutOfRangeException(IFieldSelector fieldSelector)
        {
            IDataRecord dataRecord = new { SomeColumn = 42, somecolumn = 42 }.ToDataRecord();
            Should.Throw<ArgumentOutOfRangeException>(() => fieldSelector.Execute(dataRecord));
        }
    }
}