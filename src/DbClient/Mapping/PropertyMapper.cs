namespace DbClient.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using DbClient.Extensions;
    using Selectors;

    /// <summary>
    /// A class that maps the fields from an <see cref="IDataRecord"/> to
    /// the properties of a <see cref="Type"/>.
    /// </summary>
    public class PropertyMapper : IPropertyMapper
    {
        private readonly IPropertySelector simplePropertySelector;
        private readonly IFieldSelector fieldSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapper"/> class.
        /// </summary>
        /// <param name="simplePropertySelector">The <see cref="IPropertySelector"/> that is responsible for
        /// selecting the "simple" properties from a given <see cref="Type"/>.</param>
        /// <param name="fieldSelector">The <see cref="IFieldSelector"/> that is responsible for 
        /// selecting the field names and their ordinals.</param>
        public PropertyMapper(IPropertySelector simplePropertySelector, IFieldSelector fieldSelector)
        {
            this.simplePropertySelector = simplePropertySelector;
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
            IReadOnlyDictionary<string, ColumnInfo> fieldOrdinals = fieldSelector.Execute(dataRecord);
            var simpleProperties = simplePropertySelector.Execute(type);

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

        private static ColumnInfo GetColumnInfo(PropertyInfo property, IReadOnlyDictionary<string, ColumnInfo> fields, string prefix)
        {
            string fieldName = GetFieldName(property, prefix);
            ColumnInfo columnInfo;
            if (!fields.TryGetValue(fieldName, out columnInfo))
            {
                fieldName = fieldName.GetUpperCaseLetters();
                if (!fields.TryGetValue(fieldName, out columnInfo))
                {
                    columnInfo = new ColumnInfo(-1, null, null);
                }
            }

            return columnInfo;
        }

        private ColumnInfo TryMapProperty(
            PropertyInfo property,
            string prefix,
            IReadOnlyDictionary<string, ColumnInfo> fields)
        {
            ColumnInfo columnInfo = GetColumnInfo(property, fields, prefix);
            return columnInfo;
        }
    }
}