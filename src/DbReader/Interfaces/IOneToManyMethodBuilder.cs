namespace DbReader.Interfaces
{
    using System;
    using System.Data;

    public interface IOneToManyMethodBuilder<in T>
    {
        Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}