namespace DbClient.Selectors
{
    using System;
    using System.Reflection;
    using DbClient.Extensions;

    /// <summary>
    /// An <see cref="IConstructorSelector"/> that looks for a parameterless
    /// constructor for a given <see cref="Type"/>.
    /// </summary>
    public class ParameterlessConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Gets the parameterless constructor from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get a parameterless constructor.</param>
        /// <returns><see cref="ConstructorInfo"/></returns>
        public ConstructorInfo Execute(Type type)
        {
            if (type.IsRecordType())
            {
                return type.GetConstructors()[0];
            }
            return type.GetConstructor(Type.EmptyTypes);
        }
    }
}