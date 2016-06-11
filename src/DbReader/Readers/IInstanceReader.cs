namespace DbReader.Readers
{
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of transforming an <see cref="IDataRecord"/> into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be read from the <see cref="IDataRecord"/>.</typeparam>
    public interface IInstanceReader<out T>
    {
        /// <summary>
        /// Reads an instance of <typeparamref name="T"/> from the given <paramref name="dataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> from which to read an instance of <typeparamref name="T"/>.</param>
        /// <param name="currentPrefix">The current prefix.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Read(IDataRecord dataRecord, string currentPrefix);
    }
}