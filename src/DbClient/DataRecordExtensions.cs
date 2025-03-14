namespace DbClient
{
    using System;
    using System.Data;

    /// <summary>
    /// Extends the <see cref="IDataRecord"/> interface.
    /// </summary>
    public static class DataRecordExtensions
    {
        /// <summary>
        /// Gets a byte array from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The ordinal of the column that contains the byte array.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>An array of bytes read from the data record.</returns>
        public static byte[] GetBytes(this IDataRecord dataRecord, int ordinal, int length)
        {
            byte[] buffer = new byte[length];
            dataRecord.GetBytes(ordinal, 0, buffer, 0, length);
            return buffer;
        }

        /// <summary>
        /// Gets a byte array from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The ordinal of the column that contains the byte array.</param>
        /// <returns>An array of bytes read from the data record.</returns>
        public static byte[] GetBytes(this IDataRecord dataRecord, int ordinal)
        {
            long length = dataRecord.GetBytes(ordinal, 0, null, 0, int.MaxValue);
            var buffer = new byte[length];
            dataRecord.GetBytes(ordinal, 0, buffer, 0, (int)length);
            return buffer;
        }

        /// <summary>
        /// Gets a char array from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The ordinal of the column that contains the char array.</param>
        /// <returns>An array of chars read from the data record.</returns>      
        public static char[] GetChars(this IDataRecord dataRecord, int ordinal)
        {
            long length = dataRecord.GetChars(ordinal, 0, null, 0, int.MaxValue);
            var buffer = new char[length];
            dataRecord.GetChars(ordinal, 0, buffer, 0, (int)length);
            return buffer;
        }             
    }
}