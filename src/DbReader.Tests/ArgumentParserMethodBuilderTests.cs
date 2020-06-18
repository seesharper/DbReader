using System;
using System.Data;
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
}