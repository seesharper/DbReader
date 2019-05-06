namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Database;
    using DbReader.Extensions;
    using Interfaces;
    using Selectors;

    /// <summary>
    /// Allows custom behavior to be specified
    /// </summary>
    public static class DbReaderOptions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> KeyProperties =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static Func<PropertyInfo, bool> keyConvention;

        /// <summary>
        /// Gets or sets a delegate that gets invoked after the <see cref="IDbCommand"/>
        /// for given query has been created.
        /// </summary>
        public static Action<IDbCommand> CommandInitializer { get; set; }

        static DbReaderOptions()
        {
            KeyConvention = p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase);
            ParameterParser = new RegExParameterParser(@"(:\w+)|(@\w+)", @"IN\s*\((\s*(?:@|:)\w+)\s*\)");

        }

        /// <summary>
        /// Allows a custom conversion specified when reading a property of type <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty">The property type for which to apply the conversion.</typeparam>
        /// <returns><see cref="ReadDelegate{TProperty}"/>.</returns>
        public static ReadDelegate<TProperty> WhenReading<TProperty>()
        {
            return new ReadDelegate<TProperty>();
        }

        /// <summary>
        /// Allows a custom conversion to be specified when passing an argument of type <typeparamref name="TArgument"/>.
        /// </summary>
        /// <typeparam name="TArgument">The type of argument for which to apply the conversion.</typeparam>
        /// <returns><see cref="PassDelegate{TArgument}"/></returns>
        public static PassDelegate<TArgument> WhenPassing<TArgument>()
        {
            return new PassDelegate<TArgument>();
        }

        /// <summary>
        /// Gets or sets the convention to be used for determining if a given property is a key property.
        /// </summary>
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
                    || (KeyProperties.ContainsKey(info.DeclaringType)
                        && KeyProperties[info.DeclaringType].Contains(info));
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IParameterParser"/> that is
        /// responsible for parsing the parameter names from a given sql statement.
        /// </summary>
        public static IParameterParser ParameterParser { get; set; }

        /// <summary>
        /// Specifies the key properties for a the type of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="keyExpressions">The expressions that specifies the key property.</param>
        /// <typeparam name="T">The type for which to specify key properties.</typeparam>
        public static void KeySelector<T>(params Expression<Func<T, object>>[] keyExpressions)
        {
            PropertyInfo[] properties = new PropertyInfo[keyExpressions.Length];

            for (int index = 0; index < keyExpressions.Length; index++)
            {
                var memberExpression = (MemberExpression)keyExpressions[index].AsEnumerable().Where(e => e is MemberExpression).First();
                var property = (PropertyInfo)memberExpression.Member;
                properties[index] = property;

            }

            KeyProperties.AddOrUpdate(typeof(T), type => properties, (type, infos) => properties);
        }
    }
}