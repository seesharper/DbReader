namespace DbReader.Caching
{
    using System.Collections.Generic;
    using System.Data;

    using DbReader.Interfaces;

    public class CachedFieldSelector : IFieldSelector
    {
        private readonly IFieldSelector fieldSelector;

        private IReadOnlyDictionary<string, ColumnInfo> cachedFields;        

        public CachedFieldSelector(IFieldSelector fieldSelector)
        {
            this.fieldSelector = fieldSelector;        
        }

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