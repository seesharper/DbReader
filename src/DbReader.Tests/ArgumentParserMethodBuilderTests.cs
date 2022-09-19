using System;
using System.Data;
using System.Data.Common;
using DbReader.Construction;
using Moq;
using Shouldly;
using Xunit;

namespace DbReader.Tests
{
    public class ArgumentParserMethodBuilderTests : ContainerFixture
    {
        private IArgumentParserMethodBuilder argumentParserMethodBuilder;

        [Fact]
        public void ShouldThrowMeaningfulExceptionWhenParameterValueIsNotAccepted()
        {
            var args = new { CustomerId = "ALFKI" };
            var method = argumentParserMethodBuilder.CreateMethod("@CustomerId", args.GetType(), Array.Empty<IDataParameter>());
            var exception = Should.Throw<ArgumentOutOfRangeException>(() => method("@CustomerId", args, () => new ThrowingDataParameter()));
            exception.Message.ShouldContain("The parameter (CustomerId) did not accept the value `ALFKI` (String). If the value is a custom type, consider adding support for the type using DbReaderOptions.WhenPassing()");
        }


        [Fact]
        public void ShouldHandleEnumWithoutConverterFunction()
        {
            var args = new { Status = EnumWithoutConverterFunction.Value2 };
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
            result.Parameters[0].Value.ShouldBe(2);
        }

        [Fact]
        public void ShouldHandleEnumWithConverterFunction()
        {
            DbReaderOptions.WhenPassing<EnumWithConverterFunction>().Use((parameter, value) => parameter.Value = (int)EnumWithConverterFunction.Value3);
            var args = new { Status = EnumWithConverterFunction.Value2 };
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
            result.Parameters[0].Value.ShouldBe(3);
        }

        [Fact]
        public void ShouldHandleNullableEnumWithoutConverterFunctionPassingNull()
        {
            var args = new { Status = new EnumWithoutConverterFunction?() };
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
            result.Parameters[0].Value.ShouldBe(DBNull.Value);
        }

        [Fact]
        public void ShouldHandleNullableEnumWithoutConverterFunction()
        {
            DbReaderOptions.WhenPassing<int>().Use((parameter, value) =>
            {

            });

            var args = new { Status = new EnumWithoutConverterFunction?(EnumWithoutConverterFunction.Value2) };
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
            result.Parameters[0].Value.ShouldBe(EnumWithoutConverterFunction.Value2);
        }


        [Fact]
        public void ShouldHandleNullableEnum()
        {
            // Nullable<int> nullableInt = 10;

            // var r = nullableInt.GetType();

            // TestMethod(nullableInt);

            var args = new { Status = new Nullable<DbType>(DbType.Currency) };
            //DbReaderOptions.WhenPassing<DbType>().Use((p, v) => p.Value = 1);
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
        }

        [Fact]
        public void ShouldHandleNullableInt()
        {
            var args = new { Status = new Nullable<int>(42) };
            //DbReaderOptions.WhenPassing<DbType?>().Use((p, v) => p.Value = 1);
            var method = argumentParserMethodBuilder.CreateMethod("@Status", args.GetType(), Array.Empty<IDataParameter>());
            var result = method("@Status", args, () => new TestDataParameter());
        }

        public class TestDataParameter : IDataParameter
        {
            private object _value;

            public DbType DbType { get; set; }
            public ParameterDirection Direction { get; set; }

            public bool IsNullable => true;

            public string ParameterName { get; set; }
            public string SourceColumn { get; set; }
            public DataRowVersion SourceVersion { get; set; }
            public object Value
            {
                get => _value; set
                {
                    _value = value;
                }
            }
        }


        public class ThrowingDataParameter : IDataParameter
        {
            public DbType DbType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public ParameterDirection Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public bool IsNullable => throw new NotImplementedException();

            public string ParameterName { get; set; }
            public string SourceColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public DataRowVersion SourceVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public object Value { get => throw new NotImplementedException(); set => throw new ArgumentException(); }
        }
    }

    public enum EnumWithoutConverterFunction
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 3
    }

    public enum EnumWithConverterFunction
    {
        Value1 = 1,
        Value2 = 2,
        Value3 = 3
    }
}