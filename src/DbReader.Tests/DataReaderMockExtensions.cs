namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Provides extension methods for IDataReader
    /// </summary>
    public static class DataReaderMockExtensions
    {
        /// <summary>
        /// Returns a string if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A string, or null if the column's value is NULL</returns>
        public static string GetNullableString(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (string)null : reader.GetString(index);
        }

        /// <summary>
        /// Returns a bool if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A bool, or null if the column's value is NULL</returns>
        public static bool? GetNullableBool(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (bool?)null : reader.GetBoolean(index);
        }

        /// <summary>
        /// Returns a DateTime if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A DateTime, or null if the column's value is NULL</returns>
        public static DateTime? GetNullableDateTime(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);
        }

        /// <summary>
        /// Returns a byte if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A byte, or null if the column's value is NULL</returns>
        public static byte? GetNullableByte(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (byte?)null : reader.GetByte(index);
        }

        /// <summary>
        /// Returns a short if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A short, or null if the column's value is NULL</returns>
        public static short? GetNullableInt16(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (short?)null : reader.GetInt16(index);
        }

        /// <summary>
        /// Returns an int if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>An int, or null if the column's value is NULL</returns>
        public static int? GetNullableInt32(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (int?)null : reader.GetInt32(index);
        }

        /// <summary>
        /// Returns a float if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A float, or null if the column's value is NULL</returns>
        public static float? GetNullableFloat(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (float?)null : reader.GetFloat(index);
        }

        /// <summary>
        /// Returns a double if one is present, or null if not
        /// </summary>
        /// <param name="reader">The IDbReader to read from</param>
        /// <param name="index">The index of the column to read from</param>
        /// <returns>A double, or null if the column's value is NULL</returns>
        public static double? GetNullableDouble(this IDataRecord reader, int index)
        {
            return reader.IsDBNull(index) ? (double?)null : reader.GetDouble(index);
        }

        /// <summary>
        /// Returns an implementation of IDataReader based on the public properties of T,
        /// given an IEnumerable&lt;T&gt; />
        /// </summary>
        /// <typeparam name="T">A type with properties from which to read</typeparam>
        /// <param name="items">A collection of instances of T</param>
        /// <returns>An implementation of IDataReader based on the public properties of T</returns>
        public static IDataReader ToDataReader<T>(this IEnumerable<T> items)
        {
            SqlStatement.Current = Guid.NewGuid().ToString();
            return new DataReader<T>(items);
        }

        public static IDataRecord ToDataRecord<T>(this T row)
        {
            List<T> rows = new List<T>(new T[] { row });
            IDataReader dataReader = rows.ToDataReader();
            dataReader.Read();
            return dataReader;
        }
    


        /// <summary>
        /// Private implementation of IDataReader for an IEnumerable&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">A type with properties from which to read</typeparam>
        private class DataReader<T> : IDataReader
        {
            public DataReader(IEnumerable<T> items)
            {
                enumerator = items.GetEnumerator();

                foreach (var prop in typeof(T).GetProperties())
                {
                    properties[prop.Name] = prop;
                }
            }

            private readonly Dictionary<string, PropertyInfo> properties
                = new Dictionary<string, PropertyInfo>();

            private readonly IEnumerator<T> enumerator;

            private MemoryStream bytesStream;

            private MemoryStream charsStream;

            #region IDataReader Members

            public void Close()
            {
            }

            public int Depth
            {
                get { return 0; }
            }

            public DataTable GetSchemaTable()
            {
                throw new NotImplementedException();
            }

            public bool IsClosed
            {
                get { return false; }
            }

            public bool NextResult()
            {
                return false;
            }

            public bool Read()
            {
                bytesStream = null;
                return this.enumerator.MoveNext();
            }

            public int RecordsAffected
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IDataRecord Members

            public int FieldCount   
            {
                get
                {
                    Console.WriteLine(this.properties.Count);
                    return this.properties.Count;
                }
            }

            /// <summary>
            /// Gets the value of the specified column as a Boolean.
            /// </summary>
            /// <returns>
            /// The value of the column.
            /// </returns>
            /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
            public bool GetBoolean(int i)
            {
                return (bool)this.GetValue(i);
            }

            /// <summary>
            /// Gets the 8-bit unsigned integer value of the specified column.
            /// </summary>
            /// <returns>
            /// The 8-bit unsigned integer value of the specified column.
            /// </returns>
            /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
            public byte GetByte(int i)
            {
                return (byte)this.GetValue(i);
            }

            /// <summary>
            /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
            /// </summary>
            /// <returns>
            /// The actual number of bytes read.
            /// </returns>
            /// <param name="i">The zero-based column ordinal. </param><param name="fieldOffset">The index within the field from which to start the read operation. </param><param name="buffer">The buffer into which to read the stream of bytes. </param><param name="bufferoffset">The index for <paramref name="buffer"/> to start the read operation. </param><param name="length">The number of bytes to read. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                if (bytesStream == null)
                {
                    bytesStream = new MemoryStream((byte[])GetValue(i));
                }

                if (buffer == null)
                {
                    return bytesStream.Length;
                }

                return bytesStream.Read(buffer, bufferoffset, length);
            }

            /// <summary>
            /// Gets the character value of the specified column.
            /// </summary>
            /// <returns>
            /// The character value of the specified column.
            /// </returns>
            /// <param name="i">The zero-based column ordinal. </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. </exception><filterpriority>2</filterpriority>
            public char GetChar(int i)
            {
                return (char)this.GetValue(i);
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                if (charsStream == null)
                {
                    charsStream = new MemoryStream(Encoding.ASCII.GetBytes((char[])GetValue(i)));
                }

                if (buffer == null)
                {
                    return charsStream.Length;
                }

                var byteBuffer = new byte[length];

                var bytesRead = charsStream.Read(byteBuffer, bufferoffset, length);
                var chars = Encoding.ASCII.GetChars(byteBuffer);
                for (int j = bufferoffset; j < bufferoffset + length; j++)
                {
                    buffer[j] = chars[j];
                }

                return bytesRead;
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i)
            {
                var prop = this.properties.Values.ToArray()[i];
                return prop.PropertyType.ToString();
            }

            public DateTime GetDateTime(int i)
            {
                return (DateTime)this.GetValue(i);
            }

            public decimal GetDecimal(int i)
            {
                return (decimal)this.GetValue(i);
            }

            public double GetDouble(int i)
            {
                return (double)this.GetValue(i);
            }

            public Type GetFieldType(int i)
            {
                var prop = this.properties.Values.ToArray()[i];
                return prop.PropertyType;
            }

            public float GetFloat(int i)
            {
                return (float)this.GetValue(i);
            }

            public Guid GetGuid(int i)
            {
                return (Guid)this.GetValue(i);
            }

            public short GetInt16(int i)
            {
                return (short)this.GetValue(i);
            }

            public int GetInt32(int i)
            {
                return (int)this.GetValue(i);
            }

            public long GetInt64(int i)
            {
                return (long)this.GetValue(i);
            }

            public string GetName(int i)
            {
                return this.properties.Keys.ToArray()[i];
            }

            public int GetOrdinal(string name)
            {
                return this.properties.Keys.ToList().IndexOf(name);
            }

            public string GetString(int i)
            {
                return (string)this.GetValue(i);
            }

            public object GetValue(int i)
            {
                var prop = this.properties.Values.ToArray()[i];
                return prop.GetValue(this.enumerator.Current, null);
            }

            public int GetValues(object[] values)
            {
                throw new NotImplementedException();
            }

            public bool IsDBNull(int i)
            {
                var prop = this.properties.Values.ToArray()[i];
                var val = prop.GetValue(this.enumerator.Current, null);
                return (val == null || val == DBNull.Value);
            }

            public object this[string name]
            {
                get
                {
                    return this.properties[name].GetValue(this.enumerator.Current,
                        null);
                }
            }

            public object this[int i]
            {
                get
                {
                    var prop = this.properties.Values.ToArray()[i];
                    return prop.GetValue(this.enumerator.Current, null);
                }
            }

            #endregion
        }
    }
}
