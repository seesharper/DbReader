namespace DbReader.Tests
{
    public struct CustomValueType
    {        
        public CustomValueType(int value)
            : this()
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}