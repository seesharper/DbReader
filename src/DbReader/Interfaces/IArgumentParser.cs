namespace DbReader.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Selectors;

    public interface IArgumentParser
    {
        IReadOnlyDictionary<string, object> Parse(object value);
    }

    public class ArgumentParser : IArgumentParser
    {
        private readonly IPropertySelector readablePropertySelector;

        public ArgumentParser(IPropertySelector readablePropertySelector)
        {
            this.readablePropertySelector = readablePropertySelector;
        }

        public IReadOnlyDictionary<string, object> Parse(object value)
        {
            if (value == null)
            {
                return new Dictionary<string, object>();
            }
            var properties = readablePropertySelector.Execute(value.GetType());
            return properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(value) ?? DBNull.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}