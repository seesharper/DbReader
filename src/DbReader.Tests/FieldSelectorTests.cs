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
        //[ScopedTheory, InjectData]
        //public void ShouldReturnSameInstanceWithinScope(IFieldSelector first, IFieldSelector second)
        //{
        //    first.ShouldBeSameAs(second);
        //}

        
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