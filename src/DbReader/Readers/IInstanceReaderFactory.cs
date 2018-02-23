namespace DbReader.Readers
{
    using System;

    /// <summary>
    /// Represents a class that is capable of 
    /// producing an <see cref="IInstanceReader{T}"/> based on a given <see cref="Type"/> and prefix.
    /// </summary>
    public interface IInstanceReaderFactory
    {
        /// <summary>
        /// Gets an <see cref="IInstanceReader{T}"/> for the given <paramref name="type"/> and <paramref name="prefix"/>.
        /// </summary>
        /// <param name="type">The type for which to get an <see cref="IInstanceReader{T}"/>.</param>
        /// <param name="prefix">The prefix for which to get an <see cref="IInstanceReader{T}"/>.</param>
        /// <returns></returns>
        object GetInstanceReader(Type type, string prefix);
    }
}