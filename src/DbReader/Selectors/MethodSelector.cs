namespace DbReader.Selectors
{
    using System;
    using System.Data;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A class that is capable of selecting the
    /// appropriate <see cref="IDataRecord"/> get method.
    /// </summary>
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
            if (ValueConverter.CanConvert(type))
            {
                return ValueConverter.GetConvertMethod(type);
            }

            type = type.GetUnderlyingType();

            if (ValueConverter.CanConvert(type))
            {
                return ValueConverter.GetConvertMethod(type);
            }

            if (type == typeof(bool))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetBoolean");
            }

            if (type == typeof(byte))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetByte");
            }

            if (type == typeof(char))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetChar");
            }

            if (type == typeof(char[]))
            {
                return typeof(DataRecordExtensions).GetTypeInfo().GetMethod("GetChars");
            }

            if (type == typeof(DateTime))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetDateTime");
            }

            if (type == typeof(decimal))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetDecimal");
            }

            if (type == typeof(double))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetDouble");
            }

            if (type == typeof(float))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetFloat");
            }

            if (type == typeof(byte[]))
            {
                return typeof(DataRecordExtensions).GetTypeInfo().GetMethod("GetBytes", new[] { typeof(IDataRecord), typeof(int) });
            }

            if (type == typeof(string))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetString");
            }

            if (type == typeof(int))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetInt32");
            }

            if (type == typeof(long))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetInt64");
            }

            if (type == typeof(short))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetInt16");
            }

            if (type == typeof(Guid))
            {
                return typeof(IDataRecord).GetTypeInfo().GetMethod("GetGuid");
            }

            throw new ArgumentOutOfRangeException("type", string.Format("Unable to determine the get method for {0}", type));
        }
    }
}