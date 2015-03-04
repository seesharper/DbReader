namespace DbReader.Tests
{
    using Should;
    using Xunit.Extensions;

    public class RegExParameterParserTests
    {
        [Theory, InjectData]
        public void ShouldParseParametersWithAtSymbol(IParameterParser parameterParser)
        {
            string source = "Id = @Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain("Parameter");            
        }

        [Theory, InjectData]
        public void ShouldParseParametersWithColonSymbol(IParameterParser parameterParser)
        {
            string source = "Id = :Parameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.ShouldContain("Parameter");
        }

        [Theory, InjectData]
        public void ShouldParseMultipleParameters(IParameterParser parameterParser)
        {
            string source = "Id = @Parameter, AnotherId = @AnotherParameter";
            var parameters = parameterParser.GetParameters(source);
            parameters.Length.ShouldEqual(2);
            parameters.ShouldContain("Parameter");
            parameters.ShouldContain("AnotherParameter");
        }
    }
}