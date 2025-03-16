namespace DbClient.Readers
{
    /// <summary>
    /// Represents a class that is capable of creating an <see cref="IInstanceReader{T}"/> based on the given type.
    /// </summary>
    public interface IInstanceReaderFactory
    {
        /// <summary>
        /// Gets an <see cref="IInstanceReader{T}"/> based on the type described as <typeparamref name="T"/>
        /// </summary>
        /// <param name="prefix">The prefix for which to get an <see cref="IInstanceReader{T}"/>.</param>
        /// <typeparam name="T">The type for which to get an <see cref="IInstanceReader{T}"/>.</typeparam>
        /// <returns></returns>
        IInstanceReader<T> GetInstanceReader<T>(string prefix);
    }
}