namespace DbReader.Tests
{
    using Database;
    using Shouldly;
    using Xunit;
    using DbReader.LightInject;

    public class RegExParameterParserTests : ContainerFixture
    {
        public readonly IParameterParser parameterParser;

        [Fact]
        public void ShouldParseParametersWithAtSymbol()
        {
            string source = "Id = @Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain(p => p.Name == "Parameter");
        }

        [Fact]
        public void ShouldParseParametersWithColonSymbol()
        {
            string source = "Id = :Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain(p => p.Name == "Parameter");
        }

        [Fact]
        public void ShouldParseMultipleParameters()
        {
            string source = "Id = @Parameter, AnotherId = @AnotherParameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(2);
            parameters.ShouldContain(p => p.Name == "Parameter");
            parameters.ShouldContain(p => p.Name == "AnotherParameter");
        }

        [Fact]
        public void ShouldParseDuplicateParametersOnlyOnce()
        {
            string source = "Id = @Parameter, AnotherId = @Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(1);
            parameters.ShouldContain(p => p.Name == "Parameter");
        }

        [Theory]
        [InlineData("Id = @Parameter, AnotherId IN (@ListParameter)")]
        [InlineData("Id = @Parameter, AnotherId IN ( @ListParameter )")]
        [InlineData("Id = @Parameter, AnotherId IN(@ListParameter)")]
        [InlineData("Id = @Parameter, AnotherId IN( @ListParameter )")]
        [InlineData("Id = @Parameter, AnotherId in( @ListParameter )")]
        public void ShouldMarkSingleListParameter(string source)
        {
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(2);
            parameters.ShouldContain(p => p.FullName == "@ListParameter" && p.IsListParameter);
        }

        [Theory]
        [InlineData("Id = @Parameter, AnotherId IN (@Param1, @Param3)")]
        public void ShouldNotMarkAsListParameterWhenMultiple(string source)
        {
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(3);
            parameters.ShouldNotContain(p => p.IsListParameter);
        }
    }
}