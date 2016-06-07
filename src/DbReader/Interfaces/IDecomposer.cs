namespace DbReader.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Selectors;

    public interface IDecomposer
    {
        IReadOnlyDictionary<string, object> Decompose(object value);
    }

    public class Decomposer : IDecomposer
    {
        private readonly IPropertySelector readablePropertySelector;

        public Decomposer(IPropertySelector readablePropertySelector)
        {
            this.readablePropertySelector = readablePropertySelector;
        }

        public IReadOnlyDictionary<string, object> Decompose(object value)
        {
            if (value == null)
            {
                return new Dictionary<string, object>();
            }
            var properties = readablePropertySelector.Execute(value.GetType());
            return properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(value) ?? DBNull.Value);
        }
    }
}