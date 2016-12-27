namespace DbReader.Construction
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents the cached value of a property reader delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyReaderDelegate<T>
    {
        /// <summary>
        /// The propery reader delegate.
        /// </summary>
        public Func<IDataRecord, int[], T> ReadMethod;

        /// <summary>
        /// The ordinals used to invoke the delegate.
        /// </summary>
        public int[] Ordinals;        
    }
}