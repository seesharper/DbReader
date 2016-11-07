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

        public void ShouldHandleDifferentTypeForSameSql(IArgumentParser argumentParser)
        {
            var result1 = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object);

            result1[0].ParameterName.ShouldBe("firstParameter");
            result1[1].ParameterName.ShouldBe("secondParameter");

            var result2 = argumentParser.Parse(":firstParameter, :secondParameter",
                new { firstParameter = 1, secondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object);

            result2[0].ParameterName.ShouldBe("firstParameter");
            result2[1].ParameterName.ShouldBe("secondParameter");


        }


        public void ShouldReturnEmptyArrayWhenArgumentObjectIsNull(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                null,
                () => new Mock<IDataParameter>().SetupAllProperties().Object);
            result.ShouldBeEmpty();
        }
    }
}