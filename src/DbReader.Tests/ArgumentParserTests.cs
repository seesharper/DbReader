namespace DbReader.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Database;
    using Interfaces;
    using Moq;
    using Shouldly;

    public class ArgumentParserTests
    {
        public void ShouldMapParameters(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new {FirstParameter = 1, SecondParameter = 2},
                () => new Mock<IDataParameter>().SetupAllProperties().Object);
            
            result[0].Value.ShouldBe(1);
            result[1].Value.ShouldBe(2);
        }

        public void ShouldThrowExceptionWhenParameterNameIsNotPresentInArgumentObject(IArgumentParser argumentParser)
        {
            var exception = Should.Throw<InvalidOperationException>(
                () =>
                    argumentParser.Parse(":firstParameter, :secondParameter",
                        new {FirstParameter = 1, InvalidParameter = 2},
                        () => new Mock<IDataParameter>().SetupAllProperties().Object));

            exception.Message.ShouldStartWith("Unable to resolve an argument value for parameter");
        }


        public void ShouldMapMissingParameters(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(string.Empty,
              new { FirstParameter = 1, SecondParameter = 2 },
              () => new Mock<IDataParameter>().SetupAllProperties().Object);
            result.First().Value.ShouldBe(1);
            result.Last().Value.ShouldBe(2);
        }

        public void ShouldFavorParameterNames(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object);

            result[0].ParameterName.ShouldBe("firstParameter");
            result[1].ParameterName.ShouldBe("secondParameter");
        }

    }
}