namespace DbReader
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;

    public class OneToManyPropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(p => !p.PropertyType.IsSimpleType() && p.PropertyType.IsEnumerable()).ToArray();
        }
    }
}