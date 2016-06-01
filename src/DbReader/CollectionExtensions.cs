namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="ICollection{T}"/> interface..
    /// </summary>
    public static class CollectionExtensions
    {
        private static MethodInfo openGenericTryAddMethod;

        private static ConcurrentDictionary<Type, MethodInfo> tryAddMethods =
            new ConcurrentDictionary<Type, MethodInfo>();

        static CollectionExtensions()
        {
            openGenericTryAddMethod = typeof(CollectionExtensions).GetMethod(
                "TryAdd",
                BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Adds the <paramref name="value"/> to the <paramref name="collection"/>
        /// only of the <paramref name="value"/> does not already exist.
        /// </summary>
        /// <typeparam name="T">The element type of the collection.</typeparam>
        /// <param name="collection">The target <see cref="ICollection{T}"/>.</param>
        /// <param name="value">The value to be added to the collection.</param>
        public static void TryAdd<T>(this ICollection<T> collection, T value)
        {
            if (!collection.Contains(value))
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that represents calling the <see cref="TryAdd{T}"/> method 
        /// with the given <paramref name="elementType"/>.
        /// </summary>
        /// <param name="elementType">The element type used to create the method.</param>
        /// <returns>A closed generic <see cref="TryAdd{T}"/> method.</returns>
        public static MethodInfo GetTryAddMethod(Type elementType)
        {
            return tryAddMethods.GetOrAdd(elementType, CreateClosedGenericTryAddMethod);
        }

        private static MethodInfo CreateClosedGenericTryAddMethod(Type elementType)
        {
            return openGenericTryAddMethod.MakeGenericMethod(elementType);
        }

    }
}