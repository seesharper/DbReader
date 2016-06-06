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

#if NETSTANDARD15
                var xToken = x.GetMetadataToken();
                var ytoken = y.GetMetadataToken();
#else
                var xToken = x.MetadataToken;
                var ytoken = y.MetadataToken;
#endif
                if (xToken < ytoken)
                {
                    return -1;
                }
                if (xToken > ytoken)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}