namespace DbClient.Tests
{
    using System;
    using System.Data;
    using System.Linq;
    using Construction;
    using Database;
    using Interfaces;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ArgumentParserTests : ContainerFixture
    {
        public readonly IArgumentParser argumentParser;

        [Fact]
        public void Parse_ValidArguments_ReturnsParameters()
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Parameters[0].Value.ShouldBe(1);
            result.Parameters[1].Value.ShouldBe(2);
        }

        [Fact]
        public void Parse_Null_ShouldCreateDbNull()
        {
            var result = argumentParser.Parse(":firstParameter",
                new { FirstParameter = (string)null },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            result.Parameters[0].Value.ShouldBe(DBNull.Value);
        }

        [Fact]
        public void Parse_MissingArgument_ThrowsException()
        {
            var exception = Should.Throw<InvalidOperationException>(
                () =>
                    argumentParser.Parse(":firstParameter, :secondParameter",
                        new { FirstParameter = 1, InvalidParameter = 2 },
                        () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>()));

            exception.Message.ShouldStartWith("Unable to resolve an argument value for parameter");
        }

        [Fact]
        public void Parse_MissingArgument_DoesNotThrowIfExistingParametersContainsArgument()
        {
            var existingParameterMock = new Mock<IDataParameter>();
            existingParameterMock.SetupAllProperties();
            existingParameterMock.Object.ParameterName = "secondParameter";

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, InvalidParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, new[] { existingParameterMock.Object });

            result.Parameters.Length.ShouldBe(1);
        }

        [Fact]
        public void Parse_ArgumentAlreadyExists_ThrowsException()
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

        [Fact]
        public void Parse_WithDifferentCasing_FavorsParameterNames()
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Parameters[0].ParameterName.ShouldBe("firstParameter");
            result.Parameters[1].ParameterName.ShouldBe("secondParameter");
        }

        [Fact]
        public void ShouldReturnEmptyArrayWhenArgumentObjectIsNull()
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                null,
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            result.Parameters.ShouldBeEmpty();
        }

        [Fact]
        public void Parse_WithDataParameter_ReturnsParameter()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Parameters[1].ShouldBeSameAs(parameterMock.Object);
        }

        [Fact]
        public void Parse_DataParameterWithNoName_SetParameterNameToPropertyName()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            parameterMock.Object.ParameterName.ShouldBe("SecondParameter");
        }

        [Fact]
        public void Parse_DataParameterWithName_DoesNotSetParameterNameToPropertyName()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();
            parameterMock.Object.ParameterName = "SomeValue";

            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            parameterMock.Object.ParameterName.ShouldBe("SomeValue");
        }

        [Fact]
        public void Parse_DuplicateParameterName_ReturnsOnlyOneParameter()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :firstParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Parameters.Length.ShouldBe(1);
        }

        [Fact]
        public void Parse_DuplicateParameterNameWithDifferentCasing_ReturnsOnlyOneParameter()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :FirstParameter",
                new { FirstParameter = 1, SecondParameter = parameterMock.Object },
                () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());

            result.Parameters.Length.ShouldBe(1);
        }

        [Fact]
        public void Parse_ParameterWithParantheses_ReturnsParameter()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();

            var result = argumentParser.Parse(":firstParameter, :secondParameter(1)",
               new { FirstParameter = 1, SecondParameter = parameterMock.Object },
               () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>());
            result.Parameters.Length.ShouldBe(2);
        }

        [Fact]
        public void Parse_ArgumentWithUnknownDataType_ThrowsMeaningfulException()
        {
            var parameterMock = new Mock<IDataParameter>().SetupAllProperties();
            Should.Throw<InvalidOperationException>(() => argumentParser.Parse(":firstParameter",
               new { FirstParameter = new UnknownType() },
               () => new Mock<IDataParameter>().SetupAllProperties().Object, Array.Empty<IDataParameter>()));
        }

        public class UnknownType
        {
        }
    }
}