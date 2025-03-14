namespace DbClient.Selectors
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// An <see cref="IFieldSelector"/> decorator that caches selected field from an <see cref="IDataRecord"/>.
    /// </summary>
    public class CachedFieldSelector : IFieldSelector
    {
        private readonly IFieldSelector fieldSelector;

        private IReadOnlyDictionary<string, ColumnInfo> cachedFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedFieldSelector"/> class.
        /// </summary>
        /// <param name="fieldSelector">The target <see cref="IFieldSelector"/>.</param>
        public CachedFieldSelector(IFieldSelector fieldSelector)
        {
            this.fieldSelector = fieldSelector;        
        }

        /// <summary>
        /// Selects the field name and the corresponding ordinal from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> for which to select the fieldname and ordinal.</param>
        /// <returns>An <see cref="IReadOnlyDictionary{TKey,TValue}"/> containing the field name and the ordinal.</returns>
        public IReadOnlyDictionary<string, ColumnInfo> Execute(IDataRecord dataRecord)
        {
            if (cachedFields == null)
            {
                cachedFields = fieldSelector.Execute(dataRecord);
            }
            return cachedFields;
        }
    }
}