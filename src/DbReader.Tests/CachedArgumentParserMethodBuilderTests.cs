namespace DbReader.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using Construction;
    using Moq;
    using Shouldly;

    public class CachedArgumentParserMethodBuilderTests
    {
        public void CreateMethod_SameTypeAndSql_InvokedOnlyOnce()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);
                        
            methodBuilder.CreateMethod(FakeSql.Create(), new { A = 1 }.GetType());
            methodBuilder.CreateMethod(FakeSql.Create(), new { A = 1 }.GetType());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>()), Times.Once);
        }
       
        public void CreateMethod_SameTypeAndDifferentSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);

            methodBuilder.CreateMethod(FakeSql.Create("1"), new { A = 1 }.GetType());
            methodBuilder.CreateMethod(FakeSql.Create("2"), new { A = 1 }.GetType());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>()), Times.Exactly(2));
        }

        public void CreateMethod_DifferentTypeAndSameSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);
           

            methodBuilder.CreateMethod(FakeSql.Create(), new { A = 1 }.GetType());
            methodBuilder.CreateMethod(FakeSql.Create(), new { B = 1 }.GetType());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>()), Times.Exactly(2));
        }

        public void CreateMethod_DifferentTypeAndDifferentSql_InvokedTwice()
        {
            var methodBuilderMock = CreateMethodBuilderMock();
            var methodBuilder = new CachedArgumentParserMethodBuilder(methodBuilderMock.Object);            

            methodBuilder.CreateMethod(FakeSql.Create("1"), new { A = 1 }.GetType());
            methodBuilder.CreateMethod(FakeSql.Create("2"), new { B = 1 }.GetType());

            methodBuilderMock.Verify(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>()), Times.Exactly(2));
        }

        private static Mock<IArgumentParserMethodBuilder> CreateMethodBuilderMock()
        {
            var methodBuilderMock = new Mock<IArgumentParserMethodBuilder>();
            methodBuilderMock.Setup(m => m.CreateMethod(It.IsAny<string>(), It.IsAny<Type>()))
                .Returns((o, func) => null);
            return methodBuilderMock;
        }       
    }
}