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
            parameters.ShouldContain("Parameter");
        }

        [Fact]
        public void ShouldParseParametersWithColonSymbol()
        {
            string source = "Id = :Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain("Parameter");
        }

        [Fact]
        public void ShouldParseMultipleParameters()
        {
            string source = "Id = @Parameter, AnotherId = @AnotherParameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(2);
            parameters.ShouldContain("Parameter");
            parameters.ShouldContain("AnotherParameter");
        }

        [Fact]
        public void ShouldParseDuplicateParametersOnlyOnce()
        {
            string source = "Id = @Parameter, AnotherId = @Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(1);
            parameters.ShouldContain("Parameter");
        }
    }
}