using System;
using System.Reflection;
using DbReader.Extensions;

namespace DbReader.Selectors
{
    /// <summary>
    /// An <see cref="IPropertySelector"/> decorator that ensures that 
    /// a given argument type only contains properties that can be passed as arguments.
    /// </summary>
    public class ReadableArgumentPropertiesValidator : IPropertySelector
    {
        private readonly IPropertySelector readablePropertySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadableArgumentPropertiesValidator"/> class.
        /// </summary>
        /// <param name="readablePropertySelector">The <see cref="IPropertySelector"/> that is
        /// responsible for selecting readable properties.</param>
        public ReadableArgumentPropertiesValidator(IPropertySelector readablePropertySelector) 
            => this.readablePropertySelector = readablePropertySelector;

        /// <summary>
        /// Executes the selector and returns a list of readable properties that are passable as arguments.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            var properties = readablePropertySelector.Execute(type);

            foreach (var property in properties)
            {
                EnsureIsSimpleProperty(property);
            }

            return properties;
        }

        private static void EnsureIsSimpleProperty(PropertyInfo property)
        {
            if (!IsPassable(property))
            {
                throw new InvalidOperationException(ErrorMessages.UnknownArgumentType.FormatWith(property,property.PropertyType));
            }
        }

        private static bool IsPassable(PropertyInfo property)
        {
            return property.PropertyType.IsSimpleType() || property.IsDataParameter();
        }
    }
}
