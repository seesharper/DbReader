namespace DbReader
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extends the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {        
        private static readonly Type[] SimpleTypes = new Type[6];

        private static readonly ConcurrentDictionary<Type, Type> ProjectionTypeCache = new ConcurrentDictionary<Type, Type>(); 

        static TypeExtensions()
        {
            SimpleTypes[0] = typeof(string);
            SimpleTypes[1] = typeof(Guid);
            SimpleTypes[2] = typeof(decimal);
            SimpleTypes[3] = typeof(DateTime);
            SimpleTypes[4] = typeof(byte[]);
            SimpleTypes[4] = typeof(char[]);
        }
                  
        

        /// <summary>
        /// Gets the underlying <see cref="Type"/> for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get the underlying <see cref="Type"/>.</param>
        /// <returns>The underlying <see cref="Type"/> for the given <paramref name="type"/>.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type.IsEnum)
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
            return type.IsPrimitive || SimpleTypes.Contains(type) || ValueConverter.CanConvert(type);
        }

        /// <summary>
        /// Determines if a given type implements the <see cref="ICollection{T}"/> interface.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> implements <see cref="ICollection{T}"/>, otherwise <b>false</b></returns>       
        public static bool IsCollectionType(this Type type)
        {
            return IsEnumerableOfT(type) || (!type.IsArray && type.GetInterfaces().Any(IsEnumerableOfT));
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

        private static bool IsEnumerableOfT(Type @interface)
        {
            return @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Type ResolveProjectionType(Type type)
        {
            Type projectionType = type;
            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                projectionType = type.GetGenericArguments()[0];
            }

            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    projectionType = interfaceType.GetGenericArguments()[0];
                }
            }

            return projectionType;
        }
    }
}