namespace DbReader.Construction
{
    using System.Data;

    internal static class DataParameterHelper
    {
        public static void SetName(IDataParameter dataParameter, string name)
        {
            if (dataParameter.ParameterName == null)
            {
                dataParameter.ParameterName = name;
            }
        }
    }
}