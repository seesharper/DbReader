namespace DbReader.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Adds functionality for ordering properties by their declaration order.
    /// </summary>
    public static class PropertyInfoEnumerableExtensions
    {
        /// <summary>
        /// Orders the <paramref name="properties"/> by their declaration order.
        /// </summary>
        /// <param name="properties">The properties for which to be ordered.</param>
        /// <returns>The <paramref name="properties"/> ordered by declaration.</returns>
        public static IEnumerable<PropertyInfo> OrderByDeclaration(this IEnumerable<PropertyInfo> properties)
        {
            return properties.OrderBy(p => p, new MetadataTokenComparer());
        }

        private class MetadataTokenComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y)
            {

                var xToken = x.MetadataToken;
                var ytoken = y.MetadataToken;

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