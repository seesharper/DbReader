namespace DbReader.Interfaces
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a method that reads the fields from an <see cref="IDataRecord"/>
    /// that maps to the key properties of a given <see cref="Type"/>.
    /// </summary>
    public interface IKeyReaderMethodBuilder
    {
        /// <summary>
        /// Creates a method that reads the key fields from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to read the key fields.</param>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="prefix">The field prefix used to identify the key fields.</param>
        /// <returns>A method that reads the key fields from the given <paramref name="dataRecord"/>.</returns>
        Func<IDataRecord, IStructuralEquatable> CreateMethod(Type type, IDataRecord dataRecord, string prefix);
    }
}