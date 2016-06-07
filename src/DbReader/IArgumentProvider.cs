namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;
    using Selectors;

    public interface IArgumentProvider
    {
        IReadOnlyDictionary<string, object> GetArguments(string sql, object arguments);
    }

    public class ArgumentProvider : IArgumentProvider
    {
        private readonly IParameterParser parameterParser;

        private readonly IPropertySelector readablePropertySelector;

        public ArgumentProvider(IParameterParser parameterParser, IPropertySelector readablePropertySelector)
        {
            this.parameterParser = parameterParser;
            this.readablePropertySelector = readablePropertySelector;
        }

        public IReadOnlyDictionary<string, object> GetArguments(string sql, object arguments)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (arguments == null)
            {
                return result;
            }
            var parameterNames = parameterParser.GetParameters(sql);            
            Dictionary<string, PropertyInfo> properties = readablePropertySelector.Execute(arguments.GetType()).ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var parameterName in parameterNames)
            {
                PropertyInfo propertyInfo;
                if (properties.TryGetValue(parameterName, out propertyInfo))
                {
                    var value = propertyInfo.GetValue(arguments) ?? DBNull.Value;
                    result.Add(parameterName, value);
                }
            }
            return result;
        }
    }
}