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

        public static bool? GetNullableBoolean(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetBoolean(i);
        }

        public static byte? GetNullableByte(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetByte(i);
        }

        public static char? GetNullableChar(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetChar(i);
        }

        public static char[] GetChars(this IDataRecord dataRecord, int i)
        {
            long length = dataRecord.GetChars(i, 0, null, 0, int.MaxValue);
            var buffer = new char[length];
            dataRecord.GetChars(i, 0, buffer, 0, (int)length);
            return buffer;
        }

        public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetDateTime(i);
        }

        public static decimal? GetNullableDecimal(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetDecimal(i);
        }

        public static double? GetNullableDouble(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetDouble(i);
        }

        public static float? GetNullableFloat(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetFloat(i);
        }

        public static Guid? GetNullableGuid(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetGuid(i);
        }

        public static short? GetNullableInt16(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetInt16(i);
        }

        public static int? GetNullableInt32(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetInt32(i);
        }

        public static long? GetNullableInt64(this IDataRecord dataRecord, int i)
        {
            return dataRecord.GetInt64(i);
        }
    }
}