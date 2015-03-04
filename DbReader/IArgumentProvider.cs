namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;

    public interface IArgumentProvider
    {
        IReadOnlyDictionary<string, object> GetArguments(string sql, object arguments);
    }

    public class ArgumentProvider : IArgumentProvider
    {
        private readonly IParameterParser parameterParser;
        private readonly IPropertySelector simplePropertySelector;

        public ArgumentProvider(IParameterParser parameterParser, IPropertySelector simplePropertySelector)
        {
            this.parameterParser = parameterParser;
            this.simplePropertySelector = simplePropertySelector;
        }

        public IReadOnlyDictionary<string, object> GetArguments(string sql, object arguments)
        {
            var result = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            var parameterNames = parameterParser.GetParameters(sql);
            Dictionary<string, PropertyInfo> properties = simplePropertySelector.Execute(arguments.GetType()).ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);
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