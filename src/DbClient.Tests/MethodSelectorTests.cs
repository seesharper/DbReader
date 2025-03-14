namespace DbClient.Tests
{
    using System;
    using System.Data;
    using System.Text;
    using DbClient.Interfaces;
    using DbClient.LightInject;
    using Selectors;
    using Shouldly;
    using Xunit;

    public class MethodSelectorTests
    {
        private readonly IMethodSelector methodSelector;

        public MethodSelectorTests()
        {
            var container = new ServiceContainer();
            this.methodSelector = container.GetInstance<IMethodSelector>();
        }

        [Fact]
        public void ShouldReadBoolean()
        {
            var dataRecord = new { SomeColumn = true }.ToDataRecord();
            var result = InvokeMethod<bool>(dataRecord);
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldReadNullableBoolean()
        {
            var dataRecord = new { SomeColumn = true }.ToDataRecord();
            var result = InvokeMethod<bool?>(dataRecord);
            result.Value.ShouldBeTrue();
        }

        [Fact]
        public void ShouldReadByte()
        {
            var dataRecord = new { SomeColumn = (byte)42 }.ToDataRecord();
            var result = InvokeMethod<byte>(dataRecord);
            result.ShouldBe((byte)42);
        }

        [Fact]
        public void ShouldNullableReadByte()
        {
            var dataRecord = new { SomeColumn = (byte?)42 }.ToDataRecord();
            var result = InvokeMethod<byte?>(dataRecord);
            result.ShouldBe((byte?)42);
        }

        [Fact]
        public void ShouldReadChar()
        {
            var dataRecord = new { SomeColumn = 'A' }.ToDataRecord();
            var result = InvokeMethod<char>(dataRecord);
            result.ShouldBe('A');
        }

        [Fact]
        public void ShouldReadNullableChar()
        {
            var dataRecord = new { SomeColumn = (char?)'A' }.ToDataRecord();
            var result = InvokeMethod<char?>(dataRecord);
            result.ShouldBe('A');
        }

        [Fact]
        public void ShouldReadCharArray_ReadsValue()
        {
            var dataRecord = new { SomeColumn = new[] { 'A' } }.ToDataRecord();
            var result = InvokeMethod<char[]>(dataRecord);
            result[0].ShouldBe('A');
        }

        [Fact]
        public void Invoke_DateTime_ReadsValue()
        {
            var dataRecord = new { SomeColumn = new DateTime(2014, 2, 5) }.ToDataRecord();
            var result = InvokeMethod<DateTime>(dataRecord);
            result.ShouldBe(new DateTime(2014, 2, 5));
        }

        [Fact]
        public void Invoke_NullableDateTime_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (DateTime?)new DateTime(2014, 2, 5) }.ToDataRecord();
            var result = InvokeMethod<DateTime?>(dataRecord);
            result.ShouldBe(new DateTime(2014, 2, 5));
        }

        [Fact]
        public void Invoke_Decimal_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (decimal)42 }.ToDataRecord();
            var result = InvokeMethod<decimal>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_NullableDecimal_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (decimal?)42 }.ToDataRecord();
            var result = InvokeMethod<decimal?>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_Double_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (double)42 }.ToDataRecord();
            var result = InvokeMethod<double>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_NullableDouble_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (double?)42 }.ToDataRecord();
            var result = InvokeMethod<double?>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_Float_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (float)42 }.ToDataRecord();
            var result = InvokeMethod<float>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_NullableFloat_ReadsValue()
        {
            var dataRecord = new { SomeColumn = (float?)42 }.ToDataRecord();
            var result = InvokeMethod<float?>(dataRecord);
            result.ShouldBe(42);
        }

        [Fact]
        public void Invoke_UnknownDatatype_ThrowsException()
        {
            var dataRecord = new { SomeColumn = (StringBuilder)null }.ToDataRecord();
            Should.Throw<ArgumentOutOfRangeException>(() => InvokeMethod<StringBuilder>(dataRecord));
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