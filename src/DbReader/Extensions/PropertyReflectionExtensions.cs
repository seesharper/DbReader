namespace DbReader.Extensions
{
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="PropertyInfo"/> class.
    /// </summary>
    public static class PropertyReflectionExtensions
    {
        /// <summary>
        /// Determines if the <paramref name="property"/> is writeable.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <returns>true, if the <paramref name="property"/> is writeable, otherwise, false.</returns>
        public static bool IsWriteable(this PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            return setMethod != null && !setMethod.IsStatic && setMethod.IsPublic;
        }

        /// <summary>
        /// Determines if the <paramref name="property"/> is readable.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <returns>true, if the <paramref name="property"/> is readable, otherwise, false.</returns>
        public static bool IsReadable(this PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            return getMethod != null && !getMethod.IsStatic && getMethod.IsPublic;            
        }

        /// <summary>
        /// Determines if the <paramref name="property"/> is an <see cref="IDataParameter"/>.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <returns>true, if the <paramref name="property"/> is an <see cref="IDataParameter"/> , otherwise, false.</returns>
        public static bool IsDataParameter(this PropertyInfo property)
        {
            return typeof(IDataParameter).GetTypeInfo().IsAssignableFrom(property.PropertyType);
        }

        /// <summary>
        /// Gets the "full name" of the <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> for which to get the full name.</param>
        /// <returns>The "full name" of the <paramref name="property"/>.</returns>
        public static string GetFullName(this PropertyInfo property)
        {
            return $"{property.PropertyType} {property.DeclaringType}.{property.Name}";
        }
    }
}