namespace DbReader
{
    using System;
    using System.Data;
    using System.Reflection;

    using DbReader.Interfaces;

    public class MethodSelector : IMethodSelector
    {
        /// <summary>
        /// Selects the <see cref="IDataRecord"/> get method that will 
        /// be used to read a value of the given <paramref name="type"/>./>.
        /// </summary>
        /// <param name="type">The type for which to return a <see cref="MethodInfo"/>.</param>        
        /// <returns>The get method to be used to read the value.</returns>
        public MethodInfo Execute(Type type)
        {
            type = type.GetUnderlyingType();
            
            if (ValueConverter.CanConvert(type))
            {
                return ValueConverter.GetConvertMethod(type);
            }
            
            if (type == typeof(bool))
            {
                return typeof(IDataRecord).GetMethod("GetBoolean");
            }

            if (type == typeof(bool?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableBoolean");
            }

            if (type == typeof(byte))
            {
                return typeof(IDataRecord).GetMethod("GetByte");
            }

            if (type == typeof(byte?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableByte");
            }

            if (type == typeof(char))
            {
                return typeof(IDataRecord).GetMethod("GetChar");
            }

            if (type == typeof(char?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableChar");
            }

            if (type == typeof(char[]))
            {
                return typeof(DataRecordExtensions).GetMethod("GetChars");
            }

            if (type == typeof(DateTime))
            {
                return typeof(IDataRecord).GetMethod("GetDateTime");
            }

            if (type == typeof(DateTime?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableDateTime");
            }

            if (type == typeof(decimal))
            {
                return typeof(IDataRecord).GetMethod("GetDecimal");
            }

            if (type == typeof(decimal?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableDecimal");
            }

            if (type == typeof(double))
            {
                return typeof(IDataRecord).GetMethod("GetDouble");
            }

            if (type == typeof(double?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableDouble");
            }

            if (type == typeof(float))
            {
                return typeof(IDataRecord).GetMethod("GetFloat");
            }

            if (type == typeof(float?))
            {
                return typeof(DataRecordExtensions).GetMethod("GetNullableFloat");
            }

            if (type == typeof(byte[]))
            {
                return typeof(DataRecordExtensions).GetMethod("GetBytes", new[] { typeof(IDataRecord), typeof(int) });
            }

            if (type == typeof(char))
            {
                return typeof(IDataRecord).GetMethod("GetChar");
            }

            if (type == typeof(char[]))
            {
                return typeof(IDataRecord).GetMethod("GetChars");
            }

            if (type == typeof(string))
            {
                return typeof(IDataRecord).GetMethod("GetString");
            }

            if (type == typeof(int))
            {
                return typeof(IDataRecord).GetMethod("GetInt32");
            }

            throw new ArgumentOutOfRangeException("type", string.Format("Unable to determine the get method for {0}", type));
        }        
    }
}