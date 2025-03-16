namespace DbClient.Selectors
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that capable of returning the 
    /// ordinals used to map a type to a data record.
    /// </summary>
    public interface IOrdinalSelector
    {
        /// <summary>
        /// Executes the selector and returns the ordinals required to read the columns from the data record.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return the ordinals.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <param name="prefix">The column prefix to use.</param>
        /// <returns>A list of ordinals.</returns>
        int[] Execute(Type type, IDataRecord dataRecord, string prefix);
    }
}