namespace DbReader.Construction
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a delegate that creates and populates an instance of <typeparamref name="T"/> from an 
    /// <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam> 
    public interface IReaderMethodBuilder<out T>  
    {
        /// <summary>
        /// Creates a new method that initializes and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.
        /// </summary>
        /// <returns>A delegate that creates and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.</returns>
        Func<IDataRecord, int[], T> CreateMethod(); 
    }
}