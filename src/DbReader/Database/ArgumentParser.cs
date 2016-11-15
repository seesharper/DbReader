namespace DbReader.Database
{
    using System;
    using System.Data;
    using Construction;

    public class ArgumentParser : IArgumentParser
    {
        private readonly IArgumentParserMethodBuilder argumentParserMethodBuilder;


        /// <summary>
        /// Initializes 
        /// </summary>
        /// <param name="argumentParserMethodBuilder"></param>
        public ArgumentParser(IArgumentParserMethodBuilder argumentParserMethodBuilder)
        {
            this.argumentParserMethodBuilder = argumentParserMethodBuilder;
        }

        public IDataParameter[] Parse(string sql, object arguments, Func<IDataParameter> parameterFactory)
        {
            if (arguments == null)
            {
                return new IDataParameter[] {};
            }
            var argumentParseMethod = argumentParserMethodBuilder.CreateMethod(sql, arguments.GetType());
            return argumentParseMethod(arguments, parameterFactory);
        }       
    }
}