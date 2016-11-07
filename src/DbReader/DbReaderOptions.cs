namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using Database;
    using Interfaces;
    using Selectors;

    public static class DbReaderOptions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> KeyProperties =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static Func<PropertyInfo, bool> keyConvention;

        public static Action<IDbCommand> CommandInitializer { get; set; }

        static DbReaderOptions()
        {
            KeyConvention = p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase);
            ParameterParser = new RegExParameterParser(@":(\w+)|@(\w+)");            
        }
        
        public static ReadDelegate<TProperty> WhenReading<TProperty>()
        {
            return new ReadDelegate<TProperty>();
        }

        public static PassDelegate<TArgument> WhenPassing<TArgument>()
        {
            return new PassDelegate<TArgument>();
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
                    || (KeyProperties.ContainsKey(info.DeclaringType)
                        && KeyProperties[info.DeclaringType].Contains(info));
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

            KeyProperties.AddOrUpdate(typeof(T), type => properties, (type, infos) => properties);
        }


    }


    public class ReadDelegate<TProperty>
    {
        public void Use(Func<IDataRecord, int, TProperty> readFunction)
        {
            ValueConverter.RegisterReadDelegate(readFunction);            
        }
    }

    public class PassDelegate<TArgument>
    {
        public void Use(Action<IDataParameter, TArgument> convertFunction)
        {            
            ArgumentProcessor.RegisterProcessDelegate(convertFunction);
        }
    }
}