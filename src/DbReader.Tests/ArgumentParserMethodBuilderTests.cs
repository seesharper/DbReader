using System;
using System.Data;
using DbReader.Construction;
using Moq;
using Xunit;

namespace DbReader.Tests
{
    public class ArgumentParserMethodBuilderTests : ContainerFixture
    {
        private IArgumentParserMethodBuilder argumentParserMethodBuilder;

        [Fact]
        public void TestName()
        {
            var args = new { CustomerId = "ALFKI" };
            var method = argumentParserMethodBuilder.CreateMethod("@CustomerId", args.GetType(), Array.Empty<IDataParameter>());
            method("@CustomerId", args, () => new ThrowingDataParameter());

            //method("@CustomerId",)
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