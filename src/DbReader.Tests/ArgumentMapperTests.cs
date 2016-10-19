namespace DbReader.Tests
{
    using System;
    using System.Linq;
    using Database;
    using Shouldly;

    public class ArgumentMapperTests
    {
        public void ShouldMapParameters(IArgumentMapper argumentMapper)
        {
            var result = argumentMapper.Map(":firstParameter, :secondParameter", new {FirstParameter = 1, SecondParameter = 2});
            result.First().Value.ShouldBe(1);
            result.Last().Value.ShouldBe(2);
        }

        public void ShouldThrowExceptionWhenParameterNameIsNotPresentInArgumentObject(IArgumentMapper argumentMapper)
        {
            var exception = Should.Throw<InvalidOperationException>(
                () =>
                    argumentMapper.Map(":firstParameter, :secondParameter",
                        new {FirstParameter = 1, InvalidParameter = 2}));

            exception.Message.ShouldStartWith("Unable to resolve an argument value for parameter");            
        }


        public void ShouldMapMissingParameters(IArgumentMapper argumentMapper)
        {
            var result = argumentMapper.Map(string.Empty, new { FirstParameter = 1, SecondParameter = 2 });
            result.First().Value.ShouldBe(1);
            result.Last().Value.ShouldBe(2);
        }
    }
}