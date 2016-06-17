namespace DbReader
{
    using System;
    using System.Data;

    public static class DataRecordExtensions
    {
        public static byte[] GetBytes(this IDataRecord dataRecord, int i, int length)
        {
            byte[] buffer = new byte[length];
            dataRecord.GetBytes(i, 0, buffer, 0, length);
            return buffer;
        }

        public static byte[] GetBytes(this IDataRecord dataRecord, int i)
        {
            long length = dataRecord.GetBytes(i, 0, null, 0, int.MaxValue);
            var buffer = new byte[length];
            dataRecord.GetBytes(i, 0, buffer, 0, (int)length);
            return buffer;
        }

       

       

        public static char[] GetChars(this IDataRecord dataRecord, int i)
        {
            long length = dataRecord.GetChars(i, 0, null, 0, int.MaxValue);
            var buffer = new char[length];
            dataRecord.GetChars(i, 0, buffer, 0, (int)length);
            return buffer;
        }             
    }
}