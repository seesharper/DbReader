namespace DbReader.Selectors
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that selects readable properties.
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
            return type.GetProperties().Where(p => p.IsReadable()).ToArray();
        }        
    }
}