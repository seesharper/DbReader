namespace DbReader
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class ManyToOnePropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            return
                type.GetProperties()
                    .Where(p => !p.PropertyType.IsSimpleType() && !p.PropertyType.IsCollectionType() && p.IsWriteable())
                    .ToArray();
        }
    }
}