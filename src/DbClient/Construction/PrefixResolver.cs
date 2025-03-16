namespace DbClient.Construction
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Mapping;

    /// <summary>
    /// A class that is capable of resolving the prefix 
    /// for a navigation property.
    /// </summary>
    public class PrefixResolver : IPrefixResolver
    {
        private readonly IPropertyMapper propertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixResolver"/> class.
        /// </summary>
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> instance used to determine if a given prefix 
        /// can be used to map fields/columns to the target <see cref="Type"/>.</param>        
        public PrefixResolver(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
        }

        /// <summary>
        /// Returns the prefix for the given <paramref name="navigationProperty"/>.
        /// </summary>
        /// <param name="navigationProperty">The property for which to get the prefix.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <param name="currentPrefix">The current prefix that the resolved prefix will be appended to.</param>
        /// <returns>A <see cref="string"/> value that represents the property prefix.</returns>
        public string GetPrefix(PropertyInfo navigationProperty, IDataRecord dataRecord, string currentPrefix)
        {
            string nextPrefix;
           
            if (TryGetPrefixBasedOnPropertyName(navigationProperty, dataRecord, currentPrefix, out nextPrefix))
            {
                return nextPrefix;
            }

            if (TryGetPrefixBasedOnUpperCaseLettersFromPropertyName(navigationProperty, dataRecord, currentPrefix, out nextPrefix))
            {
                return nextPrefix;
            }
            
            //if (IsAtRootLevel(currentPrefix) && HasAtLeastOneMappedPropertyAtTheRootLevel(navigationProperty, dataRecord))
            //{
            //    return string.Empty;
            //}

            return null;
        }
      
        private static string CreatePrefix(string currentPrefix, string nextPrefix)
        {
            if (string.IsNullOrEmpty(currentPrefix))
            {
                return nextPrefix;
            }

            return currentPrefix + "_" + nextPrefix;
        }
                           
        private bool TryGetPrefixBasedOnPropertyName(PropertyInfo navigationProperty, IDataRecord dataRecord, string currentPrefix, out string nextPrefix)
        {
            nextPrefix = CreatePrefix(currentPrefix, navigationProperty.Name);
            return HasAtLeastOneMappedProperty(navigationProperty.PropertyType, dataRecord, nextPrefix);
        }

        private bool TryGetPrefixBasedOnUpperCaseLettersFromPropertyName(
            PropertyInfo navigationProperty, IDataRecord dataRecord, string currentPrefix, out string nextPrefix)
        {
            string shortPrefix = navigationProperty.Name.GetUpperCaseLetters();
            if (!string.IsNullOrEmpty(shortPrefix))
            {
                nextPrefix = CreatePrefix(currentPrefix, shortPrefix);
                if (HasAtLeastOneMappedProperty(navigationProperty.PropertyType, dataRecord, nextPrefix))
                {
                    return true;
                }
            }

            nextPrefix = null;
            return false;
        }

        private bool HasAtLeastOneMappedProperty(Type type, IDataRecord dataRecord, string prefix)
        {
            return propertyMapper.Execute(type.GetProjectionType(), dataRecord, prefix).Any(pm => pm.ColumnInfo.Ordinal > -1);
        }
    }
}