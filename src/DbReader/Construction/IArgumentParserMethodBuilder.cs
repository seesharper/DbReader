namespace DbReader.Construction
{
    using System;
    using System.ComponentModel;
    using System.Data;

    public interface IArgumentParserMethodBuilder
    {
        Func<object, Func<IDataParameter> ,  IDataParameter[]> CreateMethod(string sql, Type argumentsType);
    }


    public class ArgumentValue
    {
        public object Value;
        public Type Type;
    }
}