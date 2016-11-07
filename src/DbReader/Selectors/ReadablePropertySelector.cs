namespace DbReader.Selectors
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that selects readable properties that is considered
    /// a "simple" property. 
    /// </summary>
    public class ReadablePropertySelector : IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            return properties.Where(p => p.PropertyType.IsSimpleType() && p.IsReadable()).ToArray();
        }        
    }
}