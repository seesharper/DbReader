namespace DbReader
{
    using System;
    using System.Data;

    /// <summary>
    /// Specifies the read delegate to be using for reading a property of <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public class ReadDelegate<TProperty>
    {
        /// <summary>
        /// Specifies the <paramref name="readFunction"/> to be used to read a value of type <typeparamref name="TProperty"/>
        /// </summary>
        /// <param name="readFunction">The function to be used to read the value.</param>
        public DefaultValue<TProperty> Use(Func<IDataRecord, int, TProperty> readFunction)
        {
            ValueConverter.RegisterReadDelegate(readFunction);
            return new DefaultValue<TProperty>();
        }
    }

    /// <summary>
    /// Specifies the default value to be used when the value from the database is <see cref="DBNull"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of property for which to specify a default value.</typeparam>
    public class DefaultValue<TProperty>
    {
        /// <summary>
        /// Specifies the <paramref name="defaultValue"/> to be used when the value from the database is <see cref="DBNull"/>.
        /// </summary>
        /// <param name="defaultValue">The value to be used when the value from the database is <see cref="DBNull"/>.</param>
        public void WithDefaultValue(TProperty defaultValue)
        {
            ValueConverter.RegisterDefaultValue(defaultValue);
        }
    }
}