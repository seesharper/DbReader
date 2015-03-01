namespace DbReader.Interfaces
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of mapping the fields from an <see cref="IDataRecord"/>
    /// to the properties of a given <see cref="Type"/>.
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// Maps the fields from the <paramref name="dataRecord"/> to the properties of the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the properties to be mapped.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the fields to be mapped.</param>
        /// <param name="prefix">The prefix to use when mapping columns to properties.</param>
        /// <returns>A list of <see cref="MappingInfo"/> instances that represents the mapping between a field and a property.</returns>
        MappingInfo[] Execute(Type type, IDataRecord dataRecord, string prefix);
    }
}