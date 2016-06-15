namespace DbReader.Tests
{
    using Database;
    using Shouldly;

    public class RegExParameterParserTests
    {
        public void ShouldParseParametersWithAtSymbol(IParameterParser parameterParser)
        {
            string source = "Id = @Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain("Parameter");
        }

        public void ShouldParseParametersWithColonSymbol(IParameterParser parameterParser)
        {
            string source = "Id = :Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain("Parameter");
        }

        public void ShouldParseMultipleParameters(IParameterParser parameterParser)
        {
            string source = "Id = @Parameter, AnotherId = @AnotherParameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldBe(2);
            parameters.ShouldContain("Parameter");
            parameters.ShouldContain("AnotherParameter");
        }
    }
}