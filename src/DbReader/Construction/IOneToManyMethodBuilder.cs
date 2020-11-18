namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Readers;

    /// <summary>
    /// Represents a class that dynamically creates a method used to
    /// populate collection properties of a given type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> for which to create the dynamic method.</typeparam>
    public interface IOneToManyMethodBuilder<in T>
    {
        /// <summary>
        /// Creates a dynamic method that populates mapped collection properties.
        /// </summary>
        /// <param name="dataRecord">The source <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The property prefix used to identify the fields in the <see cref="IDataRecord"/>.</param>
        /// <returns>A delegate representing a dynamic method that populates mapped collection properties.</returns>
        Action<IDataRecord, T, IGenericInstanceReaderFactory> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}