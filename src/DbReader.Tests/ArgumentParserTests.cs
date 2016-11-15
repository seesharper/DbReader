﻿namespace DbReader.Tests
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
                () => new Mock<IDataParameter>().SetupAllProperties().Object);
            
            result[0].Value.ShouldBe(1);
            result[1].Value.ShouldBe(2);
        }
        
        public void Parse_MissingArgument_ThrowsException(IArgumentParser argumentParser)
        {
            var exception = Should.Throw<InvalidOperationException>(
                () =>
                    argumentParser.Parse(":firstParameter, :secondParameter",
                        new {FirstParameter = 1, InvalidParameter = 2},
                        () => new Mock<IDataParameter>().SetupAllProperties().Object));

            exception.Message.ShouldStartWith("Unable to resolve an argument value for parameter");
        }
       
        public void Parse_MissingParameters_ReturnsParametersFromArguments(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(FakeSql.Create(),
              new { FirstParameter = 1, SecondParameter = 2 },
              () => new Mock<IDataParameter>().SetupAllProperties().Object);
            result.First().Value.ShouldBe(1);
            result.Last().Value.ShouldBe(2);
        }
        
        public void Parse_WithDifferentCasing_FavorsParameterNames(IArgumentParser argumentParser)
        {
            var result = argumentParser.Parse(":firstParameter, :secondParameter",
                new { FirstParameter = 1, SecondParameter = 2 },
                () => new Mock<IDataParameter>().SetupAllProperties().Object);

            result[0].ParameterName.ShouldBe("firstParameter");
            result[1].ParameterName.ShouldBe("secondParameter");
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