namespace DbReader.Interfaces
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of reading the key columns for a given <see cref="Type"/>.
    /// </summary>    
    public interface IKeyReader
    {
        /// <summary>
        /// Reads the key columns from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to read the key columns.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the key columns.</param>
        /// <param name="prefix">The current column prefix.</param>
        /// <returns>An <see cref="IStructuralEquatable"/> that represent the key for an instance of the given <paramref name="type"/>.</returns>
        IStructuralEquatable Read(Type type, IDataRecord dataRecord, string prefix);
    }
}