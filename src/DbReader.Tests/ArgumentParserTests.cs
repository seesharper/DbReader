namespace DbReader.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Construction;
    using Database;
    using Interfaces;
    using Moq;
    using Shouldly;

    public class ArgumentParserTests
    {
        public void Parse_ValidArguments_ReturnsParameters(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new {FirstParameter = 1, SecondParameter = 2},
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            
            result[0].Value.ShouldBe(1);
            result[1].Value.ShouldBe(2);
        }
        
        public void Parse_MissingArgument_ThrowsException(IArgumentParser argumentParser)
        {
            var exception = Should.Throw<InvalidOperationException>(
                () =>
                    argumentParser.Parse(":firstParameter, :secondParameter",
                        new {FirstParameter = 1, InvalidParameter = 2},
                        () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>()));

            exception.Message.ShouldStartWith("Unable to resolve an argument value for parameter");
        }

        public void Parse_MissingArgument_DoesNotThrowIfExistingParametersContainsArgument(IArgumentParser argumentParser)
        {
            var existingParameterMock = new Mock<IDataParameter>();
            existingParameterMock.SetupAllProperties();
            existingParameterMock.Object.ParameterName = "secondParameter";

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new {FirstParameter = 1, InvalidParameter = 2},
                () => new Mock<IDataParameter>().SetupAllProperties().Object, new[]{existingParameterMock.Object});

            result.Length.ShouldBe(1);
        }

        public void Parse_ArgumentAlreadyExists_ThrowsException(IArgumentParser argumentParser)
        {
            var existingParameterMock = new Mock<IDataParameter>();
            existingParameterMock.SetupAllProperties();
            existingParameterMock.Object.ParameterName = "secondParameter";

            Should.Throw<InvalidOperationException>(
                () => argumentParser.Parse(FakeSql.Create(":firstParameter, :secondParameter"),
                    new { FirstParameter = 1, SecondParameter = 2 },
                    () => new Mock<IDataParameter>().SetupAllProperties().Object, new[] { existingParameterMock.Object }
                ));
        }


        public void Parse_MissingParameters_ReturnsParametersFromArguments(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(FakeSql.Create(),
              new { FirstParameter = 1, SecondParameter = 2 },
              () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            result.First().Value.ShouldBe(1);
            result.Last().Value.ShouldBe(2);
        }
        
        public void Parse_WithDifferentCasing_FavorsParameterNames(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result[0].ParameterName.ShouldBe("firstParameter");
            result[1].ParameterName.ShouldBe("secondParameter");
        }
            

        public void ShouldReturnEmptyArrayWhenArgumentObjectIsNull(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                null,
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            result.ShouldBeEmpty();            
        }

        public void Parse_WithDataParameter_ReturnsParameter(IArgumentParser argumentParser)
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new {FirstParameter = 1, SecondParameter = parameterMock.Object},
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result[1].ShouldBeSameAs(parameterMock.Object);
        }

        public void Parse_DataParameterWithNoName_SetParameterNameToPropertyName(IArgumentParser argumentParser)
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            parameterMock.Object.ParameterName.ShouldBe("SecondParameter");
        }

        public void Parse_DataParameterWithName_DoesNotSetParameterNameToPropertyName(IArgumentParser argumentParser)
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();
            parameterMock.Object.ParameterName = "SomeValue";

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            parameterMock.Object.ParameterName.ShouldBe("SomeValue");
        }

        public void Parse_DuplicateParameterName_ReturnsOnlyOneParameter(IArgumentParser argumentParser)
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :firstParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Length.ShouldBe(1);
        }

        public void Parse_DuplicateParameterNameWithDifferentCasing_ReturnsOnlyOneParameter(IArgumentParser argumentParser)
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :FirstParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Length.ShouldBe(1);
        }
    }
}