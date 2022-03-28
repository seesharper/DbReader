namespace DbReader.Mapping
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// An <see cref="IPropertyMapper"/> decorator that ensures that the mapping
    /// between fields and properties are compatible with regards to datatypes.
    /// </summary>
    public class PropertyTypeValidator : IPropertyMapper
    {
        private readonly IPropertyMapper propertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTypeValidator"/> class.
        /// </summary>
        /// <param name="propertyMapper">The target <see cref="IPropertyMapper"/>.</param>
        public PropertyTypeValidator(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
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
            var mappings = propertyMapper.Execute(type, dataRecord, prefix);
            foreach (var mappingInfo in mappings.Where(m => m.ColumnInfo.Ordinal != -1))
            {
                if (mappingInfo.ColumnInfo.Type == typeof(DBNull))
                {
                    continue;
                }

                var propertyType = mappingInfo.Property.PropertyType;

                if (!propertyType.IsAssignableFrom(mappingInfo.ColumnInfo.Type.GetTypeInfo()) && !ValueConverter.CanConvert(propertyType))
                {
                    if (propertyType.IsEnum || propertyType.IsNullable())
                    {
                        Execute(propertyType.GetUnderlyingType(), dataRecord, prefix);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                        ErrorMessages.IncompatibleTypes.FormatWith(
                            mappingInfo.Property,
                            mappingInfo.ColumnInfo,
                            mappingInfo.ColumnInfo.Type,
                            mappingInfo.Property.PropertyType));
                    }
                }
            }
            return mappings;
        }
    }
}