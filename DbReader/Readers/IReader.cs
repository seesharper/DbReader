namespace DbReader
{
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of transforming an <see cref="IDataRecord"/> into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the reader.</typeparam>
    public interface IReader<out T> where T : new()
    {
        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the data for a new instance of <typeparamref name="T"/>.</param>
        /// <param name="ordinals">The column name prefix used to identify source columns.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Read(IDataRecord dataRecord, int[] ordinals);
    }
}