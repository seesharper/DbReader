namespace DbReader.Tests
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.InteropServices;

    using DbReader.Interfaces;

    using Should;
    using Should.Core.Assertions;

    public class FieldSelectorTests
    {
        [ScopedTheory, InjectData]      
        public void ShouldReturnSameInstanceWithinScope(IFieldSelector first, IFieldSelector second)
        {
            first.ShouldBeSameAs(second);
        }

        [ScopedTheory, InjectData]      
        public void Execute_ValidColumnns_ReturnsDictionary(IFieldSelector fieldSelector)
        {
            IDataRecord dataRecord = new { SomeColumn = 42 }.ToDataRecord();
            IReadOnlyDictionary<string, ColumnInfo> result = fieldSelector.Execute(dataRecord);
            result["SomeColumn"].Ordinal.ShouldEqual(0);
        }

        [ScopedTheory, InjectData]
        public void Execute_DuplicateFieldNames_ThrowsArgumentOutOfRangeException(IFieldSelector fieldSelector)
        {
            IDataRecord dataRecord = new { SomeColumn = 42, somecolumn = 42 }.ToDataRecord();
            Assert.Throws<ArgumentOutOfRangeException>(() => fieldSelector.Execute(dataRecord));            
        }
    }
}