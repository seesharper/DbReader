namespace DbReader.Tests
{
    using System;
    using System.Data;
    using DbReader.Interfaces;
    using Should;
    
    public class MethodSelectorTests
    {
        private readonly IMethodSelector methodSelector;

        public MethodSelectorTests(IMethodSelector methodSelector)
        {
            this.methodSelector = methodSelector;
        }
       
        public void ShouldReadBoolean()
        {
            var dataRecord = new { SomeColumn = true }.ToDataRecord();
            var result = InvokeMethod<bool>(dataRecord);
            result.ShouldBeTrue();
        }

        public void ShouldReadNullableBoolean()
        {
            var dataRecord = new { SomeColumn = true }.ToDataRecord();
            var result = InvokeMethod<bool?>(dataRecord);
            result.Value.ShouldBeTrue();
        }

        public void ShouldReadByte()
        {
            var dataRecord = new { SomeColumn = (byte)42 }.ToDataRecord();
            var result = InvokeMethod<byte>(dataRecord);
            result.ShouldEqual((byte)42);
        }

        public void ShouldNullableReadByte()
        {
            var dataRecord = new { SomeColumn = (byte?)42 }.ToDataRecord();
            var result = InvokeMethod<byte?>(dataRecord);
            result.ShouldEqual((byte?)42);
        }

        public void ShouldReadChar()
        {
            var dataRecord = new { SomeColumn = 'A' }.ToDataRecord();
            var result = InvokeMethod<char>(dataRecord);
            result.ShouldEqual('A');
        }

        public void ShouldReadNullableChar()
        {
            var dataRecord = new { SomeColumn = (char?)'A' }.ToDataRecord();
            var result = InvokeMethod<char?>(dataRecord);
            result.ShouldEqual('A');
        }

        public void ShouldReadCharArray_ReadsValue()
        {
            var dataRecord = new { SomeColumn = new[] { 'A' } }.ToDataRecord();
            var result = InvokeMethod<char[]>(dataRecord);
            result[0].ShouldEqual('A');
        }

        public void Invoke_DateTime_ReadsValue()
        {
            var dataRecord = new { SomeColumn = new DateTime(2014, 2, 5) }.ToDataRecord();
            var result = InvokeMethod<DateTime>(dataRecord);
            result.ShouldEqual(new DateTime(2014, 2, 5));
        }

        public void Invoke_NullableDateTime_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (DateTime?)new DateTime(2014, 2, 5) }.ToDataRecord();
            var result = InvokeMethod<DateTime?>(dataRecord);
            result.ShouldEqual(new DateTime(2014, 2, 5));
        }

        public void Invoke_Decimal_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (decimal)42 }.ToDataRecord();
            var result = InvokeMethod<decimal>(dataRecord);
            result.ShouldEqual(42);
        }

        public void Invoke_NullableDecimal_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (decimal?)42 }.ToDataRecord();
            var result = InvokeMethod<decimal?>(dataRecord);
            result.ShouldEqual(42);
        }

        public void Invoke_Double_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (double)42 }.ToDataRecord();
            var result = InvokeMethod<double>(dataRecord);
            result.ShouldEqual(42);
        }

        public void Invoke_NullableDouble_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (double?)42 }.ToDataRecord();
            var result = InvokeMethod<double?>(dataRecord);
            result.ShouldEqual(42);
        }

        public void Invoke_Float_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (float)42 }.ToDataRecord();
            var result = InvokeMethod<float>(dataRecord);
            result.ShouldEqual(42);
        }

        public void Invoke_NullableFloat_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (float?)42 }.ToDataRecord();
            var result = InvokeMethod<float?>(dataRecord);
            result.ShouldEqual(42);
        }

        private T InvokeMethod<T>(IDataRecord dataRecord)
        {
            var methodSelector = new MethodSelector();
            var method = methodSelector.Execute(typeof(T));
            if (method.IsStatic)
            {
                return (T)method.Invoke(null, new object[] { dataRecord, 0 });
            }

            return (T)method.Invoke(dataRecord, new object[] { 0 });
        }
    }
}