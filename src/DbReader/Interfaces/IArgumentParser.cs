namespace DbReader.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Construction;
    using Selectors;

    public interface IArgumentParser
    {
        IReadOnlyDictionary<string, ArgumentValue> Parse(object value);
    }

    public class ArgumentParser : IArgumentParser
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;

        public ArgumentParser(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }

        public IReadOnlyDictionary<string, ArgumentValue> Parse(object value)
        {            
            Dictionary<string, ArgumentValue> map = new Dictionary<string, ArgumentValue>(StringComparer.OrdinalIgnoreCase);
            var argumentParseMethod = argumentParserMethodBuilder.CreateMethod(value.GetType());
            argumentParseMethod(value, map);
            return map;
        }

       
    }
}