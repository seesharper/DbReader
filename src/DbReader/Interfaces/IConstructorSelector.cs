namespace DbReader.Interfaces
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of selecting 
    /// a constructor from a given <see cref="Type"/>.
    /// </summary>
    public interface IConstructorSelector
    {
        /// <summary>
        /// Gets a <see cref="ConstructorInfo"/> from the given <paramref name="type."/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get a <see cref="ConstructorInfo"/>.</param>
        /// <returns><see cref="ConstructorInfo"/></returns>
        ConstructorInfo Execute(Type type);
    }
}