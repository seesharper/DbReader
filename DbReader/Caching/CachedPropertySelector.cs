namespace DbReader.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    using DbReader.Interfaces;

    public class CachedPropertySelector : IPropertySelector
    {
        private readonly IPropertySelector propertySelector;

        private readonly ConcurrentDictionary<Type, PropertyInfo[]> cache 
            = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public CachedPropertySelector(IPropertySelector propertySelector)
        {
            this.propertySelector = propertySelector;
        }

        public PropertyInfo[] Execute(Type type)
        {
            return cache.GetOrAdd(type, propertySelector.Execute);
        }
    }
}