using System.Data;

namespace DbReader.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using Construction;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CachedArgumentParserMethodBuilderTests
    {
        [Fact]
        public void CreateMethod_SameTypeAndSql_InvokedOnlyOnce()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);

            var arg = new { A = 1 };

            methodBuilder.CreateMethod(FakeSql.Create("10"), arg.GetType(), Array.Empty<IDataParameter>());
            methodBuilder.CreateMethod(FakeSql.Create("10"), arg.GetType(), Array.Empty<IDataParameter>());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<IDataParameter[]>()), Times.Once);
        }

        [Fact]
        public void CreateMethod_SameTypeAndDifferentSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);

            methodBuilder.CreateMethod(FakeSql.Create("1"), new { A = 1 }.GetType(), Array.Empty<IDataParameter>());
            methodBuilder.CreateMethod(FakeSql.Create("2"), new { A = 1 }.GetType(), Array.Empty<IDataParameter>());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<IDataParameter[]>()), Times.Exactly(2));
        }

        [Fact]
        public void CreateMethod_DifferentTypeAndSameSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);


            methodBuilder.CreateMethod(FakeSql.Create(), new { A = 1 }.GetType(), Array.Empty<IDataParameter>());
            methodBuilder.CreateMethod(FakeSql.Create(), new { B = 1 }.GetType(), Array.Empty<IDataParameter>());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<IDataParameter[]>()), Times.Exactly(2));
        }

        [Fact]
        public void CreateMethod_DifferentTypeAndDifferentSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);

            methodBuilder.CreateMethod(FakeSql.Create("1"), new { A = 1 }.GetType(), Array.Empty<IDataParameter>());
            methodBuilder.CreateMethod(FakeSql.Create("2"), new { B = 1 }.GetType(), Array.Empty<IDataParameter>());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<IDataParameter[]>()), Times.Exactly(2));
        }

        private static Mock<IArgumentParserMethodBuilder> CreateMethodBuilderMock()
        {
            var methodBuilderMock = new Mock<IArgumentParserMethodBuilder>();
            methodBuilderMock.Setup(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>(), It.IsAny<IDataParameter[]>()))
                .Returns((s, o, func) => null);
            return methodBuilderMock;
        }
    }
}