namespace DbClient.Selectors
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Represents a class that returns an <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the field name and the ordinal.
    /// </summary>
    public interface IFieldSelector
    {
        /// <summary>
        /// Selects the field name and the corresponding ordinal from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> for which to select the fieldname and ordinal.</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the field name and the ordinal.</returns>
        IReadOnlyDictionary<string, ColumnInfo> Execute(IDataRecord dataRecord);
    }
}