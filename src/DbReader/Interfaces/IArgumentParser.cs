namespace DbReader.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Construction;
    using Selectors;

    public interface IArgumentParser
    {
        IDataParameter[] Parse(string sql, object value, Func<IDataParameter> parameterFactory);
    }

    public class ArgumentParser : IArgumentParser
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;

        public ArgumentParser(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }

        public IDataParameter[] Parse(string sql, object value, Func<IDataParameter> parameterFactory)
        {                        
            var argumentParseMethod = argumentParserMethodBuilder.CreateMethod(sql, value.GetType());
            return argumentParseMethod(value, parameterFactory);
        }       
    }
}