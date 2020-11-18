namespace DbReader.Construction
{
    using System;
    using System.Data;
    using DbReader.Readers;

    /// <summary>
    /// Represents a class that dynamically creates a method used to
    /// populate "many-to-one" properties of a given type.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> for which to create the dynamic method.</typeparam>
    public interface IManyToOneMethodBuilder<in T>
    {
        /// <summary>
        ///  Creates a dynamic method that populates mapped "many-to-one" properties.
        /// </summary>
        /// <param name="dataRecord">The source <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The property prefix used to identify the fields in the <see cref="IDataRecord"/>.</param>
        /// <returns>A delegate representing a dynamic method that populates mapped "many-to-one" properties.</returns>
        Action<T, IDataRecord, IInstanceReaderFactory> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}