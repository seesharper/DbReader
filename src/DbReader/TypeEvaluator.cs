namespace DbReader
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Provides fast and lazy type evaluation.
    /// </summary>
    /// <typeparam name="T">The type to evaluate.</typeparam>
    public static class TypeEvaluator<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Lazy<bool> EvaluatedValue;

        static TypeEvaluator()
        {
            EvaluatedValue = new Lazy<bool>(EvaluateType);
        }

        /// <summary>
        /// true if the <typeparamref name="T"/> has navigation properties, otherwise false.
        /// </summary>
        public static bool HasNavigationProperties => EvaluatedValue.Value;

        private static bool EvaluateType()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Any(p => !p.PropertyType.IsSimpleType());
        }
    }
}