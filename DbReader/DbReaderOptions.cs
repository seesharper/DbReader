namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Xml.Serialization;

    public static class DbReaderOptions
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> keyProperties =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static Func<PropertyInfo, bool> keyConvention;

        static DbReaderOptions()
        {
            KeyConvention = p =>
                p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.InvariantCultureIgnoreCase);
        }

        public static Func<PropertyInfo, bool> KeyConvention
        {
            get
            {
                return keyConvention;
            }
            set
            {
                keyConvention =
                    info =>
                    value(info)
                    || (keyProperties.ContainsKey(info.DeclaringType)
                        && keyProperties[info.DeclaringType].Contains(info));
            }
        }

        public static void KeySelector<T>(params Expression<Func<T, object>>[] keyExpressions)
        {
            PropertyInfo[] properties = new PropertyInfo[keyExpressions.Length];

            for (int index = 0; index < keyExpressions.Length; index++)
            {
                var keyExpression = keyExpressions[index];
                var property = ((PropertyInfo)((MemberExpression)((UnaryExpression)keyExpression.Body).Operand).Member);
                properties[index] = property;

            }

            keyProperties.AddOrUpdate(typeof(T), type => properties, (type, infos) => properties);
        }
    }
}