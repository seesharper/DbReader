namespace DbClient.Selectors
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    /// <summary>
    /// An <see cref="IPropertySelector"/> decorator that caches selected properties.
    /// </summary>
    public class CachedPropertySelector : IPropertySelector
    {
        private readonly IPropertySelector propertySelector;

        private readonly ConcurrentDictionary<Type, PropertyInfo[]> cache 
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertySelector"/> class.
        /// </summary>
        /// <param name="propertySelector">The target <see cref="IPropertySelector"/>.</param>
        public CachedPropertySelector(IPropertySelector propertySelector)
        {
            this.propertySelector = propertySelector;
        }

        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return cache.GetOrAdd(type, propertySelector.Execute);
        }
    }
}