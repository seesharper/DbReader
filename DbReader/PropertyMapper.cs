namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;

    /// <summary>
    /// A class that maps the fields from an <see cref="IDataRecord"/> to
    /// the properties of a <see cref="Type"/>.
    /// </summary>
    public class PropertyMapper : IPropertyMapper
    {
        private readonly IPropertySelector propertySelector;
        private readonly IFieldSelector fieldSelector;
        private readonly List<int> mappedOrdinals = new List<int>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapper"/> class.
        /// </summary>
        /// <param name="propertySelector">The <see cref="IPropertySelector"/> that is responsible for
        /// selecting the "simple" properties from a given <see cref="Type"/>.</param>
        /// <param name="fieldSelector">The <see cref="IFieldSelector"/> that is responsible for 
        /// selecting the field names and their ordinals.</param>
        public PropertyMapper(IPropertySelector propertySelector, IFieldSelector fieldSelector)
        {
            this.propertySelector = propertySelector;
            this.fieldSelector = fieldSelector;
        }

        /// <summary>
        /// Maps the fields from the <paramref name="dataRecord"/> to the properties of the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the properties to be mapped.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the fields to be mapped.</param>
        /// <param name="prefix">The prefix to use when mapping columns to properties.</param>
        /// <returns>A list of <see cref="MappingInfo"/> instances that represents the mapping between a field and a property.</returns>
        public MappingInfo[] Execute(Type type, IDataRecord dataRecord, string prefix)
        {
            var fieldOrdinals = fieldSelector.Execute(dataRecord);
            var simpleProperties = propertySelector.Execute(type);
            
            return
                simpleProperties.Select(
                    property => new MappingInfo(property, TryMapProperty(property, prefix, fieldOrdinals))).ToArray();
        }

        private static string GetFieldName(PropertyInfo property, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return property.Name;
            }
            
            return prefix + "_" + property.Name;
        }

        private static int GetProperyOrdinal(PropertyInfo property, IReadOnlyDictionary<string, int> fieldOrdinals, string prefix)
        {
            string fieldName = GetFieldName(property, prefix);
            int ordinal;
            if (!fieldOrdinals.TryGetValue(fieldName, out ordinal))
            {
                ordinal = -1;
            }

            return ordinal;
        }

        private int TryMapProperty(
            PropertyInfo property,
            string prefix,
            IReadOnlyDictionary<string, int> fieldOrdinals)
        {
            int ordinal = GetProperyOrdinal(property, fieldOrdinals, prefix);
            if (ordinal != -1 && mappedOrdinals.Contains(ordinal))
            {
                ordinal = -1;
            }
            else
            {
                mappedOrdinals.Add(ordinal);
            }

            return ordinal;
        }      
    }
}