namespace DbReader
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using DbReader.Interfaces;

    /// <summary>
    /// An <see cref="IPropertyMapper"/> that looks for properties
    /// marked with the <see cref="KeyAttribute"/> to determine the key 
    /// properties for a given <see cref="Type"/>.
    /// </summary>
    public class KeyPropertyMapper : IPropertyMapper
    {
        private readonly IPropertyMapper propertyMapper;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPropertyMapper"/> class.
        /// </summary>
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> that is responsible 
        /// for mapping properties from a given <see cref="Type"/> to the ordinals from an <see cref="IDataRecord"/>.</param>        
        public KeyPropertyMapper(IPropertyMapper propertyMapper)
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
            MappingInfo[] propertyMappings = propertyMapper.Execute(type, dataRecord, prefix);
            
            MappingInfo[] keyPropertyMappings =
                propertyMappings.Where(pm => DbReaderOptions.KeySelector(pm.Property)).ToArray();

            return keyPropertyMappings;
        }
    }
}