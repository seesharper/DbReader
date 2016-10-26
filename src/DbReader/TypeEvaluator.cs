namespace DbReader
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Selectors;

    public static class TypeEvaluator<T>
    {
        private static readonly Lazy<bool> EvaluatedValue;

        static TypeEvaluator()
        {
            EvaluatedValue = new Lazy<bool>(EvaluateType);
        }

        public static bool HasNavigationProperties => EvaluatedValue.Value;

        private static bool EvaluateType()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Any(p => !p.PropertyType.IsSimpleType());            
        }
    }
}