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
        public void Use(Func<IDataRecord, int, TProperty> readFunction)
        {
            ValueConverter.RegisterReadDelegate(readFunction);            
        }
    }
}