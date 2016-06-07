namespace DbReader.Construction
{
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    /// A class that is capable of creating a cache key 
    /// used to cache dynamically created methods.
    /// </summary>
    public class CacheKeyFactory : ICacheKeyFactory
    {
        /// <summary>
        /// Creates a based on the given <paramref name="type"/>, <paramref name="dataRecord"/> and <paramref name="prefix"/>.
        /// </summary>
        /// <param name="type">The current <see cref="Type"/></param>
        /// <param name="dataRecord">The current <see cref="IDataRecord"/></param>
        /// <param name="prefix">The current prefix.</param>
        /// <returns>A cache key used to cache dynamically created methods.</returns>
        public string CreateKey(Type type, IDataRecord dataRecord, string prefix)
        {                        
            var sb = new StringBuilder(type.FullName);
            sb.Append(prefix);
            sb.Append(dataRecord.FieldCount);              
            return sb.ToString();
        }
    }   
}