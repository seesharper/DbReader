namespace DbReader.Interfaces
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that creates an instance of <typeparamref name="T"/>
    /// based on a given <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the method.</typeparam>
    public interface IInstanceReaderMethodBuilder<out T>
    {
        /// <summary>
        /// Creates a method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the data for the instance.</param>
        /// <param name="prefix">The current prefix.</param>
        /// <returns>A method that creates an instance of <typeparamref name="T"/>
        /// based on the given <paramref name="dataRecord"/>.</returns>
        Func<IDataRecord, T> CreateMethod(IDataRecord dataRecord, string prefix);
    }
}