namespace DbReader.Tests
{
    using DbReader.Interfaces;

    using Should;

    public class PropertyMapperTests
    {
        [ScopedTheory, InjectData]
        public void ShouldReturnSameInstanceWithinScope(IPropertyMapper first, IPropertyMapper second)
        {
            first.ShouldBeSameAs(second);
        }

        [ScopedTheory, InjectData]
        public void Execute_MatchingField_ReturnsPositiveOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldEqual(0);
        }

        [ScopedTheory, InjectData]
        public void Execute_NonMatchingField_ReturnsNegativeOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { UnknownProperty = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldEqual(-1);
        }

        [ScopedTheory, InjectData]
        public void Execute_MatchingFieldWithPrefix_ReturnsPositiveOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { SomePrefix_Int32Property = 42 }.ToDataRecord();

            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, "SomePrefix");

            result[0].ColumnInfo.Ordinal.ShouldEqual(0);
        }

        [ScopedTheory, InjectData]
        public void Execute_Twice_ReturnsNegativeOrdinal(IPropertyMapper propertyMapper)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();

            propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);
            var result = propertyMapper.Execute(typeof(SampleClass), dataRecord, string.Empty);

            result[0].ColumnInfo.Ordinal.ShouldEqual(-1);
        }



    }
}