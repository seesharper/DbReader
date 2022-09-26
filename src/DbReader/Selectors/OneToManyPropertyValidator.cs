namespace DbReader.Selectors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Extensions;

    /// <summary>
    /// An <see cref="IPropertySelector"/> decorator that validates that the 
    /// properties selected for one-to-many relationsships are valid.
    /// </summary>
    public class OneToManyPropertyValidator : IPropertySelector
    {
        private readonly IPropertySelector oneToManyPropertySelector;

        private static readonly Type[] ValidCollectionTypes = { typeof(IEnumerable<>), typeof(ICollection<>), typeof(Collection<>) };
        private static readonly Lazy<string> ValidCollectionTypesMessage;

        static OneToManyPropertyValidator()
        {
            ValidCollectionTypesMessage = new Lazy<string>(() =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (var validCollectionType in ValidCollectionTypes)
                {
                    sb.AppendLine(validCollectionType.ToString());
                }
                return sb.ToString();
            });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyPropertyValidator"/> class.
        /// </summary>
        /// <param name="oneToManyPropertySelector">The target <see cref="IPropertySelector"/>.</param>
        public OneToManyPropertyValidator(IPropertySelector oneToManyPropertySelector)
        {
            this.oneToManyPropertySelector = oneToManyPropertySelector;
        }

        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            var properties = oneToManyPropertySelector.Execute(type);
            ValidateCollectionType(properties);
            ValidateCollectionItemType(properties);
            return properties;
        }

        private static void ValidateCollectionType(PropertyInfo[] properties)
        {
            var firstInvalidProperty = properties.FirstOrDefault(p => !IsValidPropertyType(p.PropertyType));
            if (firstInvalidProperty != null)
            {
                throw new InvalidOperationException(
                    ErrorMessages.InvalidCollectionType.FormatWith(
                        $"{firstInvalidProperty.DeclaringType}.{firstInvalidProperty.Name}",
                        ValidCollectionTypesMessage.Value));
            }
        }

        private static void ValidateCollectionItemType(PropertyInfo[] properties)
        {
            var firstInvalidProperty = properties.FirstOrDefault(p => !IsValidProjectionType(p.PropertyType));
            if (firstInvalidProperty != null)
            {
                throw new InvalidOperationException(
                    ErrorMessages.SimpleProjectType.FormatWith(
                        $"{firstInvalidProperty.DeclaringType}.{firstInvalidProperty.Name}"));
            }
        }

        private static bool IsValidPropertyType(Type propertyType)
        {
            if (!propertyType.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            var openGenericTypeDefinition = propertyType.GetGenericTypeDefinition();
            return ValidCollectionTypes.Contains(openGenericTypeDefinition);
        }

        private static bool IsValidProjectionType(Type propertyType)
        {
            return !propertyType.GetTypeInfo().GenericTypeArguments[0].IsSimpleType();
        }
    }
}