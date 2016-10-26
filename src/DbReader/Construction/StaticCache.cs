namespace DbReader.Construction
{
    using System;
    using System.Collections.Concurrent;
    using Database;
    using LightInject;

    /// <summary>
    /// A static CachedValues used to CachedValues things related 
    /// to creating dynamic methods.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public static class StaticCache<TValue>
    {        
        private static readonly ConcurrentDictionary<Tuple<Type, string, string>, TValue> CachedValues =
            new ConcurrentDictionary<Tuple<Type, string, string>, TValue>();

        /// <summary>
        /// Gets or adds a value of <typeparamref name="TValue"/> based on the 
        /// given <paramref name="type"/> and <paramref name="prefix"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which a dynamic method is currently being constructed.</param>
        /// <param name="prefix">The prefix for which a dynamic method is currently being constructed.</param>
        /// <param name="delegateFactory">A factory delegate used to provide a value of <typeparamref name="TValue"/> 
        /// if the value does not exist in the CachedValues.</param>
        /// <returns>A value of <typeparamref name="TValue"/> based on the 
        /// given <paramref name="type"/> and <paramref name="prefix"/>.</returns>
        public static TValue GetOrAdd(Type type, string prefix, Func<TValue> delegateFactory)
        {            
            var key = Tuple.Create(type, prefix, SqlStatement.Current);            
            return CachedValues.GetOrAdd(key, _ => delegateFactory());
        }
    }
}