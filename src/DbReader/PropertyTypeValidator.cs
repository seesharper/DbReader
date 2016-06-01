namespace DbReader
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using DbReader.Interfaces;

    public class PropertyTypeValidator : IPropertyMapper
    {
        private readonly IPropertyMapper propertyMapper;

        public PropertyTypeValidator(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
        }

        public MappingInfo[] Execute(Type type, IDataRecord dataRecord, string prefix)
        {
            var mappings = propertyMapper.Execute(type, dataRecord, prefix);
            foreach (var mappingInfo in mappings.Where(m => m.ColumnInfo.Ordinal != -1))
            {
                if (!mappingInfo.Property.PropertyType.GetTypeInfo().IsAssignableFrom(mappingInfo.ColumnInfo.Type.GetTypeInfo()))
                {
                    throw new InvalidOperationException(
                        ErrorMessages.IncompatibleTypes.FormatWith(
                            mappingInfo.Property,
                            mappingInfo.ColumnInfo,
                            mappingInfo.ColumnInfo.Type,
                            mappingInfo.Property.PropertyType));
                }
            }
            return mappings;
        }
    }
}