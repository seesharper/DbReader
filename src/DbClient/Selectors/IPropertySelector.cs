namespace DbClient.Selectors
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of selecting a set of properties from a given <see cref="Type"/>.
    /// </summary>
    public interface IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        PropertyInfo[] Execute(Type type);
    }
}