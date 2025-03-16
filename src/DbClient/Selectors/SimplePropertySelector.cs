namespace DbClient.Selectors
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that selects writeable properties that is considered
    /// a "simple" property. 
    /// </summary>
    public class SimplePropertySelector : IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of writeable properties that is considered a "simple" property.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            return properties.Where(p => TypeReflectionExtensions.IsSimpleType(p.PropertyType) && PropertyReflectionExtensions.IsWriteable(p)).ToArray();
        }
    }
}