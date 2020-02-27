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
        /// Orders the <paramref name="members"/> by their declaration order.
        /// </summary>
        /// <param name="members">The properties for which to be ordered.</param>
        /// <returns>The <paramref name="members"/> ordered by declaration.</returns>
        public static IEnumerable<MemberInfo> OrderByDeclaration(this IEnumerable<MemberInfo> members)
        {
            return members.OrderBy(p => p, new MetadataTokenComparer());
        }

        private class MetadataTokenComparer : IComparer<MemberInfo>
        {
            public int Compare(MemberInfo x, MemberInfo y)
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