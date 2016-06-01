namespace DbReader.Interfaces
{
    using System;
    using System.Data;

    public interface IManyToOneMethodBuilder<in T>
    {
        Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}