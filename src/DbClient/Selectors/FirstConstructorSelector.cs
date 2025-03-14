namespace DbClient.Selectors
{
    using System;
    using System.Reflection;

    /// <summary>
    /// An <see cref="IConstructorSelector"/> that selected the first available public constructor.
    /// </summary>
    public class FirstConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Gets a <see cref="ConstructorInfo"/> from the given <paramref name="type."/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get a <see cref="ConstructorInfo"/>.</param>
        /// <returns><see cref="ConstructorInfo"/></returns>
        public ConstructorInfo Execute(Type type)
        {
            return type.GetConstructors()[0];
        }
    }
}