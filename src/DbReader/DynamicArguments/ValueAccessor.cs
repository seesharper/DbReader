using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace DbReader.DynamicArguments
{
    /// <summary>
    /// Enables access to <see cref="IDynamicType"/> values by the field name.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public static class ValueAccessor<TTarget>
    {
        private static ConcurrentDictionary<string, Delegate> delegateCache = new ConcurrentDictionary<string, Delegate>();

        private static Delegate cachedDelegate;

        /// <summary>
        /// Gets the field value based on the given <paramref name="fieldName"/>.
        /// </summary>
        /// <param name="target">The target object (<see cref="IDynamicType"/>)</param>
        /// <param name="fieldName">The name of the field for which to get the value.</param>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetValue<TValue>(TTarget target, string fieldName)
        {
            var accessorDelegate = (Func<TTarget, TValue>)delegateCache.GetOrAdd(fieldName, n => CreateAccessorDelegate<TValue>(n));
            return accessorDelegate(target);
        }

        private static Delegate CreateAccessorDelegate<TValue>(string fieldName)
        {
            var fieldInfo = typeof(TTarget).GetField("_" + fieldName);

            var valueAccessorMethod = new DynamicMethod("GetValue", typeof(TValue), new[] { typeof(TTarget) }, typeof(ArgumentsBuilder), true);
            var generator = valueAccessorMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.Emit(OpCodes.Ret);

            var accessorDelegate = valueAccessorMethod.CreateDelegate(typeof(Func<TTarget, TValue>));
            return accessorDelegate;
        }
    }

}
