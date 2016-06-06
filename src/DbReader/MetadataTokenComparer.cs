namespace DbReader
{
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    public static class PropertyInfoEnumerableExtensions
    {
        public static IEnumerable<PropertyInfo> OrderByDeclaration(this IEnumerable<PropertyInfo> properties)
        {
            return properties.OrderBy(p => p, new MetadataTokenComparer());
        }

        private class MetadataTokenComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y)
            {                
                if (x.MetadataToken < y.MetadataToken)
                {
                    return -1;
                }
                if (x.MetadataToken > y.MetadataToken)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}