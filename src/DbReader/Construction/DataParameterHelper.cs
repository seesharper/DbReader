namespace DbReader.Construction
{
    using System.Data;

    /// <summary>
    /// A helper class to set the <see cref="IDataParameter.ParameterName"/>.
    /// </summary>
    public static class DataParameterHelper
    {
        /// <summary>
        /// Assigns the given <paramref name="name"/> to the <see cref="IDataParameter.ParameterName"/> property.
        /// </summary>
        /// <param name="dataParameter">The target <see cref="IDataParameter"/>.</param>
        /// <param name="name">The name to be used as the parameter name.</param>
        public static void SetName(IDataParameter dataParameter, string name)
        {
            if (dataParameter.ParameterName == null)
            {
                dataParameter.ParameterName = name;
            }
        }
    }
}