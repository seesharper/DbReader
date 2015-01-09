namespace DbReader
{
    using System;
    using System.Data;

    public interface IRelationMethodBuilder<in T>
    {
        Action<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}