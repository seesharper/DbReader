namespace DbClient.Mapping
{
    using System;
    using System.Data;
    using Extensions;

    /// <summary>
    /// A <see cref="IPropertyMapper"/> decorator that validates 
    /// the the result from an <see cref="IPropertyMapper"/> that maps 
    /// key properties to fields.
    /// </summary>
    public class KeyPropertyMapperValidator : IKeyPropertyMapper
    {
        private readonly IKeyPropertyMapper keyPropertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPropertyMapperValidator"/> class.
        /// </summary>
        /// <param name="keyPropertyMapper">The target <see cref="IPropertyMapper"/>.</param>
        public KeyPropertyMapperValidator(IKeyPropertyMapper keyPropertyMapper)
        {
            this.keyPropertyMapper = keyPropertyMapper;
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
            var result = keyPropertyMapper.Execute(type, dataRecord, prefix);
           
            if (result.Length == 0)
            {
                throw new InvalidOperationException(ErrorMessages.MissingKeyProperties.FormatWith(type));
            }

            foreach (MappingInfo keyMappingInfo in result)
            {
                if (keyMappingInfo.ColumnInfo.Ordinal == -1)
                {
                    throw new InvalidOperationException(ErrorMessages.UnmappedKeyProperty.FormatWith(keyMappingInfo.Property.GetFullName()));
                }
            }

            return result;
        }
    }
}