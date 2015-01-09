namespace DbReader.Tests
{
    using Should;

    public class PropertyReaderTests
    {
        [ScopedTheory, InjectData]
        public void Read_ReturnsInstance(IReader<SampleClass> propertyReader)
        {
            var dataRecord = new { Int32Property = 42 }.ToDataRecord();
            var instance = propertyReader.Read(dataRecord, new[] { 0 });
            instance.ShouldNotBeNull();
        }
    }
}