namespace DbReader.Tests
{
    using System.Data;
    using Moq;
    using Shouldly;

    public class ArgumentProcessorTests
    {
        static ArgumentProcessorTests()
        {
            ArgumentProcessor.RegisterProcessDelegate<CustomValueType>((parameter, argument) => parameter.Value = argument.Value);
        }

        public void ShouldProcessArgument()
        {
            Mock<IDataParameter> dataParameterMock = new Mock<IDataParameter>();
            dataParameterMock.SetupAllProperties();

            ArgumentProcessor.Process(typeof(CustomValueType), dataParameterMock.Object, new CustomValueType(42));

            dataParameterMock.Object.Value.ShouldBe(42);
        }
    }
}