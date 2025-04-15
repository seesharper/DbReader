/*****************************************************************************   
    The MIT License (MIT)
    Copyright (c) 2014 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    DbReader version 1.0.0.1
    https://github.com/seesharper/DbReader
    http://twitter.com/bernhardrichter
******************************************************************************/
namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Used to register a custom conversion from a <see cref="IDataRecord"/> to an instance of a given <see cref="Type"/>.
    /// </summary>
    public static class ValueConverter
    {
        private static readonly ConcurrentDictionary<Type, Delegate> ReadDelegates =
            new ConcurrentDictionary<Type, Delegate>();

        private static readonly ConcurrentDictionary<Type, object> DefaultValues = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Registers a function delegate that creates a value of <typeparamref name="T"/> from an <see cref="IDataRecord"/>
        /// at the specified ordinal (column index).
        /// </summary>        
        /// <typeparam name="T">The type for which to register a convert function.</typeparam>
        /// <param name="convertFunction">The function delegate used to convert the value.</param>
        public static void RegisterReadDelegate<T>(Func<IDataRecord, int, T> convertFunction)
        {
            ReadDelegates.AddOrUpdate(typeof(T), type => convertFunction, (type, del) => convertFunction);
        }

        public static void RegisterDefaultValue<T>(T defaultValue)
        {
            DefaultValues.AddOrUpdate(typeof(T), type => defaultValue, (type, value) => defaultValue);
        }


        /// <summary>
        /// Determines if the given <paramref name="type"/> can be converted.
        /// </summary>        
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>true, if the <paramref name="type"/> can be converted, otherwise, false.</returns>
        internal static bool CanConvert(Type type)
        {
            return ReadDelegates.ContainsKey(type);
        }

        /// <summary>
        /// Determines if the given <paramref name="type"/> has a default value to be used when the value from the database is null.
        /// </summary>
        /// <param name="type">The type to be checked for a default value.</param>
        /// <returns>true, if there is a default value registration for the given type, otherwise, false.</returns>
        internal static bool HasDefaultValue(Type type)
        {
            return DefaultValues.ContainsKey(type);
        }

        internal static T GetDefaultValue<T>()
        {
            return (T)DefaultValues[typeof(T)];
        }


        /// <summary>
        /// Converts the value from the <paramref name="dataRecord"/> at the given <paramref name="ordinal"/>
        /// to an instance of <typeparamref name="T"/>.
        /// </summary>        
        /// <typeparam name="T">The return type from the convert function.</typeparam>
        /// <param name="dataRecord">The current <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The current ordinal (column index).</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T Convert<T>(IDataRecord dataRecord, int ordinal)
        {
            if (dataRecord is CommandWrappingDataReader reader)
            {
                return ((Func<IDataRecord, int, T>)ReadDelegates[typeof(T)])(reader.InnerReader, ordinal);
            }
            else
            {
                return ((Func<IDataRecord, int, T>)ReadDelegates[typeof(T)])(dataRecord, ordinal);
            }
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that represents the <see cref="Convert"/> method.
        /// </summary>   
        /// <param name="type">The <see cref="Type"/> for which to get the convert method.</param>     
        /// <returns>The <see cref="MethodInfo"/> that represents the <see cref="Convert"/> method.</returns>
        internal static MethodInfo GetConvertMethod(Type type)
        {
            var openGenericConvertMethod = typeof(ValueConverter).GetMethod("Convert", BindingFlags.Static | BindingFlags.Public);
            return openGenericConvertMethod.MakeGenericMethod(type);
        }
    }
}