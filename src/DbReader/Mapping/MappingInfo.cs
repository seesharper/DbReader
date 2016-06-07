namespace DbReader.Mapping
{
    using System.Reflection;
    using Extensions;
    using Selectors;

    /// <summary>
    /// Represents the mapping between a <see cref="PropertyInfo"/> and a <see cref="ColumnInfo"/>
    /// </summary>
    public class MappingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingInfo"/> class.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <param name="columnInfo">The target <see cref="ColumnInfo"/>.</param>
        public MappingInfo(PropertyInfo property, ColumnInfo columnInfo)
        {
            Property = property;
            ColumnInfo = columnInfo;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> that represents the property mapped to a <see cref="ColumnInfo"/>.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the <see cref="ColumnInfo"/> that represents the field mapped to a <see cref="PropertyInfo"/>.
        /// </summary>
        public ColumnInfo ColumnInfo { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>       
        public override string ToString()
        {
            return "[{0}] Property: {1}, Ordinal: {2}".FormatWith(Property.DeclaringType, Property, ColumnInfo.Ordinal);            
        }
    }
}