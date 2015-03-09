namespace DbReader
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;

    public interface ICacheKeyFactory
    {
        string CreateKey(Type type, IDataRecord dataRecord, string prefix);
    }
    
    
    public class CacheKeyFactory : ICacheKeyFactory
    {
        public string CreateKey(Type type, IDataRecord dataRecord, string prefix)
        {
            var sb = new StringBuilder(type.FullName);
            sb.Append(prefix);
            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                sb.Append(dataRecord.GetName(i));
            }            
            return sb.ToString();
        }
    }   
}