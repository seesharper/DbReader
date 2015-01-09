namespace DbReader
{
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents the mapping between a <see cref="PropertyInfo"/> and the ordinal in the target <see cref="IDataRecord"/>.
    /// </summary>
    public class MappingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingInfo"/> class.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <param name="ordinal">The ordinal for the <paramref name="property"/>.</param>
        public MappingInfo(PropertyInfo property, int ordinal)
        {
            Property = property;
            Ordinal = ordinal;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> that has an ordinal in the target <see cref="IDataRecord"/>.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets the ordinal for the <see cref="Property"/>.
        /// </summary>
        public int Ordinal { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>       
        public override string ToString()
        {
            return "[{0}] Property: {1}, Ordinal: {2}".FormatWith(Property.DeclaringType, Property, Ordinal);            
        }
    }
}