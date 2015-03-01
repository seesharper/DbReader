namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    using DbReader.Interfaces;

    /// <summary>
    /// A class that returns a dictionary containing the column name and the column ordinal.
    /// </summary>
    public class FieldSelector : IFieldSelector
    {
        /// <summary>
        /// Selects the field name and the corresponding ordinal from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> for which to select the fieldname and ordinal.</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the field name and the ordinal.</returns>
        public IReadOnlyDictionary<string, int> Execute(IDataRecord dataRecord)
        {
            int fieldCount = dataRecord.FieldCount;
            var result = new Dictionary<string, int>(fieldCount, StringComparer.InvariantCultureIgnoreCase);
            
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = dataRecord.GetName(i);                
                if (result.ContainsKey(fieldName))
                {
                    throw new ArgumentOutOfRangeException("dataRecord", ErrorMessages.DuplicateFieldName.FormatWith(fieldName));
                }

                result.Add(dataRecord.GetName(i), i);                
            }

            return new ReadOnlyDictionary<string, int>(result);
        }
    }
}