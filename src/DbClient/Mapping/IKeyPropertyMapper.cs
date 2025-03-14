using System;

namespace DbClient.Mapping
{
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of mapping the key fields from a given <see cref="IDataRecord"/>
    /// according to the specified <see cref="DbClientOptions.KeyConvention"/>.    
    /// </summary>
    public interface IKeyPropertyMapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataRecord"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        MappingInfo[] Execute(Type type, IDataRecord dataRecord, string prefix);
    }
}
