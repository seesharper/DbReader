namespace DbReader.Selectors
{
    using System;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of selecting the 
    /// appropriate <see cref="IDataRecord"/> get method.
    /// </summary>
    public interface IMethodSelector
    {
        /// <summary>
        /// Selects the <see cref="IDataRecord"/> get method that will 
        /// be used to read a value of the given <paramref name="type"/>./>.
        /// </summary>
        /// <param name="type">The type for which to return a <see cref="MethodInfo"/>.</param>        
        /// <returns>The get method to be used to read the value.</returns>
        MethodInfo Execute(Type type);
    }
}