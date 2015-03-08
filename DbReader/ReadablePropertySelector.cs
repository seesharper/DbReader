namespace DbReader
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;

    public class ReadablePropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            return properties.Where(p => p.PropertyType.IsSimpleType() && p.IsReadable()).ToArray();
        }        
    }
}