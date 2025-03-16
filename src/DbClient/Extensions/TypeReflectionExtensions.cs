namespace DbClient.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="Type"/> class.
    /// </summary>
    public static class TypeReflectionExtensions
    {
        private static readonly Type[] SimpleTypes = new Type[6];

        private static readonly ConcurrentDictionary<Type, Type> ProjectionTypeCache = new ConcurrentDictionary<Type, Type>();

        private static readonly Type[] OpenGenericTupleTypes =
            {
                typeof(Tuple<>), typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>),
                typeof(Tuple<,,,,>), typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>),
                typeof(Tuple<,,,,,,,>)
            };

        static TypeReflectionExtensions()
        {
            SimpleTypes[0] = typeof(string);
            SimpleTypes[1] = typeof(Guid);
            SimpleTypes[2] = typeof(decimal);
            SimpleTypes[3] = typeof(DateTime);
            SimpleTypes[4] = typeof(byte[]);
            SimpleTypes[5] = typeof(char[]);
        }

        /// <summary>
        /// Creates a closed generic <see cref="Tuple"/> type based on the given <paramref name="typeArguments"/>.
        /// </summary>
        /// <param name="typeArguments">The types to be used to create the closed generic <see cref="Tuple"/> type.</param>
        /// <returns><see cref="Tuple"/>.</returns>
        public static Type ToTupleType(this Type[] typeArguments)
        {
            Type openGenericType = OpenGenericTupleTypes[typeArguments.Length - 1];
            return openGenericType.MakeGenericType(typeArguments);
        }

        /// <summary>
        /// Gets the underlying <see cref="Type"/> for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get the underlying <see cref="Type"/>.</param>
        /// <returns>The underlying <see cref="Type"/> for the given <paramref name="type"/>.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.GetUnderlyingType(type);
            }

            if (type.IsNullable())
            {
                return Nullable.GetUnderlyingType(type);
            }

            return type;
        }

        /// <summary>
        /// Determines if the a given type is a <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> is a <see cref="Nullable{T}"/> type, otherwise <b>false</b></returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Determines if a given type is a "simple" type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> is a "simple" type, otherwise <b>false</b></returns>
        public static bool IsSimpleType(this Type type)
        {
            type = type.GetUnderlyingType();
            return type.GetTypeInfo().IsPrimitive || SimpleTypes.Contains(type) || ValueConverter.CanConvert(type) || ArgumentProcessor.CanProcess(type);
        }

        /// <summary>
        /// Determines if a given type implements the <see cref="IEnumerable{T}"/> interface.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> implements <see cref="ICollection{T}"/>, otherwise <b>false</b></returns>
        public static bool IsEnumerable(this Type type)
        {
            return IsEnumerableOfT(type) || (type.GetInterfaces().Any(IsEnumerableOfT));
        }

        /// <summary>
        /// Determines if the given type implements <see cref="IEnumerable{T}"/> and that the the element type is a simple type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> implements <see cref="IEnumerable{T}"/> and the element type is a simple type, otherwise <b>false</b></returns>
        public static bool IsEnumerableOfSimpleType(this Type type)
        {
            if (!type.IsEnumerable())
            {
                return false;
            }

            return type.GetProjectionType().IsSimpleType();
        }

        /// <summary>
        /// Returns the actual projected type from the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get the projection type.</param>
        /// <returns>If the <paramref name="type"/> is a collection, the collection element type, otherwise <paramref name="type"/>.</returns>
        public static Type GetProjectionType(this Type type)
        {
            return ProjectionTypeCache.GetOrAdd(type, ResolveProjectionType);
        }

        /// <summary>
        /// Determines if the given <paramref name="type"/> is a record type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to be checked.</param>
        /// <returns>true if the type if a record type, otherwise false.</returns>
        public static bool IsRecordType(this Type type)
        {
            return type.GetMethods().Any(m => m.Name == "<Clone>$");
        }

        private static bool IsEnumerableOfT(Type @interface)
        {
            return @interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Type ResolveProjectionType(Type type)
        {
            Type projectionType = type;
            if (type.GetTypeInfo().IsInterface && type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                projectionType = type.GetGenericArguments()[0];
            }

            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    projectionType = interfaceType.GetGenericArguments()[0];
                }
            }

            return projectionType;
        }
    }
}