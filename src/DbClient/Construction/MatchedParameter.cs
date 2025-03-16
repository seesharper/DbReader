using System.Reflection;
using DbClient.Database;

namespace DbClient.Construction
{
    /// <summary>
    /// Represents a match between an argument object property and parameter found in a given sql statement.
    /// </summary>
    public class MatchedParameter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchedParameter"/> class.
        /// </summary>
        /// <param name="dataParameter">The <see cref="DataParameterInfo"/> containing information about a parsed data parameter.</param>
        /// <param name="property">The property that matches the <paramref name="dataParameter"/>.</param>
        public MatchedParameter(DataParameterInfo dataParameter, PropertyInfo property)
        {
            DataParameter = dataParameter;
            Property = property;
        }

        /// <summary>
        /// Gets the <see cref="DataParameterInfo"/> containing information about a parsed data parameter.
        /// </summary>
        public DataParameterInfo DataParameter { get; }

        /// <summary>
        /// Gets the property that matches the data parameter.
        /// </summary>
        public PropertyInfo Property { get; }
    }
}
