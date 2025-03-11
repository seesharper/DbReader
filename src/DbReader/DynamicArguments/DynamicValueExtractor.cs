using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DbReader.Extensions;
using DbReader.Tracking;

namespace DbReader.DynamicArguments
{
    /// <summary>
    /// Extracts values from an existing object and creates an array of <see cref="DynamicMemberInfo"/>.
    /// </summary>
    public class DynamicValueExtractor
    {
        private static MethodInfo AddMethod = typeof(List<DynamicMemberInfo>).GetMethod(nameof(List<DynamicMemberInfo>.Add));
        private static ConcurrentDictionary<Type, Delegate> delegateCache = new ConcurrentDictionary<Type, Delegate>();
        private static ConcurrentDictionary<string, Delegate> trackedDelegateCache = new ConcurrentDictionary<string, Delegate>();

        /// <summary>
        /// Creates an array of <see cref="DynamicMemberInfo"/> based upon the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value for which to extract members and their values.</param>
        /// <typeparam name="T">The type of object for which to extract members and their values.</typeparam>
        /// <returns>An array of <see cref="DynamicMemberInfo"/> based upon the given <paramref name="value"/>.</returns>
        public DynamicMemberInfo[] GetDynamicMembers<T>(T value)
        {
            var dynamicMembers = new List<DynamicMemberInfo>();
            Delegate extractDelegate = null;
            if (value is ITrackedObject trackedObject)
            {

                var modifiedProperties = trackedObject.GetModifiedProperties();
                string[] cacheKeyParts = new string[modifiedProperties.Count + 1];
                modifiedProperties.CopyTo(cacheKeyParts, 0);
                cacheKeyParts[modifiedProperties.Count] = typeof(T).FullName;
                var cacheKey = cacheKeyParts.CreateCacheKey();
                extractDelegate = trackedDelegateCache.GetOrAdd(cacheKey, d => CreateExtractDelegate<T>(value));
            }
            else
            {
                extractDelegate = delegateCache.GetOrAdd(typeof(T), d => CreateExtractDelegate<T>(value));
            }



            var typedDelegate = (Action<T, List<DynamicMemberInfo>>)extractDelegate;
            typedDelegate(value, dynamicMembers);
            return dynamicMembers.ToArray();
        }

        private static Delegate CreateExtractDelegate<T>(T value)
        {
            MemberInfo[] fieldsAndProperties = null;

            if (typeof(ITrackedObject).IsAssignableFrom(typeof(T)))
            {
                var modifiedProperties = ((ITrackedObject)value).GetModifiedProperties();
                fieldsAndProperties = typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                .Where(m => modifiedProperties.Contains(m.Name)).OrderByDeclaration().ToArray();

            }
            else
            {
                fieldsAndProperties = typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public).
                Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property).OrderByDeclaration().ToArray();
            }



            var extractMethod = new DynamicMethod("Extract", typeof(void), new Type[] { typeof(T), typeof(List<DynamicMemberInfo>) }, typeof(DynamicValueExtractor), true);
            var generator = extractMethod.GetILGenerator();
            Type memberType = null;
            foreach (var memberInfo in fieldsAndProperties)
            {
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldstr, memberInfo.Name);
                generator.Emit(OpCodes.Ldarg_0);
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)memberInfo;
                    var getMethod = propertyInfo.GetGetMethod();
                    memberType = propertyInfo.PropertyType;

                    generator.Emit(OpCodes.Callvirt, getMethod);
                }
                else
                {
                    var fieldInfo = (FieldInfo)memberInfo;
                    memberType = fieldInfo.FieldType;
                    generator.Emit(OpCodes.Ldfld, fieldInfo);
                }

                var closedGenericMemberInfoType = typeof(DynamicMemberInfo<>).MakeGenericType(memberType);
                var constructorInfo = closedGenericMemberInfoType.GetConstructors()[0];
                generator.Emit(OpCodes.Newobj, constructorInfo);
                generator.Emit(OpCodes.Callvirt, AddMethod);
            }

            generator.Emit(OpCodes.Ret);
            var extractDelegate = extractMethod.CreateDelegate(typeof(Action<T, List<DynamicMemberInfo>>));
            return extractDelegate;
        }
    }

}
