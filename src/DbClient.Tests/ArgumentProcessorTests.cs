namespace DbClient.Tests
{
    using System.Data;
    using Extensions;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ArgumentProcessorTests
    {
        static ArgumentProcessorTests()
        {
            ArgumentProcessor.RegisterProcessDelegate<CustomValueType>((parameter, argument) => parameter.Value = argument.Value);
        }

        [Fact]
        public void ShouldProcessArgument()
        {
            Mock<IDataParameter> dataParameterMock = new Mock<IDataParameter>();
            dataParameterMock.SetupAllProperties();

            ArgumentProcessor.Process(typeof(CustomValueType), dataParameterMock.Object, new CustomValueType(42));

            dataParameterMock.Object.Value.ShouldBe(42);
        }

        [Fact]
        public void ShouldRecognizeCustomArgumentTypeAsSimpleType()
        {
            typeof(CustomValueType).IsSimpleType().ShouldBeTrue();
        }
    }
}