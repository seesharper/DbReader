namespace DbReader
{
    using System;
    using System.Collections.Concurrent;

    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;


    public static class DbReaderOptions
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> keyProperties =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static Func<PropertyInfo, bool> keyConvention;

        static DbReaderOptions()
        {
            KeyConvention = p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase);
            ParameterParser = new RegExParameterParser(@":(\w+)|@(\w+)");
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

        /// <summary>
        /// Gets or sets the <see cref="IParameterParser"/> that is 
        /// reponsible for parsing the parameter names from a given sql statement.
        /// </summary>
        public static IParameterParser ParameterParser { get; set; }

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