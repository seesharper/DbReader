namespace DbClient.Construction
{
    using LightInject;


    /// <summary>
    /// A simple static cache.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Cache<TKey, TValue>
    {
        private static ImmutableHashTree<TKey, TValue> tree = ImmutableHashTree<TKey, TValue>.Empty;

        /// <summary>
        /// Gets the <typeparamref name="TValue"/> represented by the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key for which to get the value.</param>
        /// <returns>An <typeparamref name="TValue"/> if found, otherwise the default value.</returns>
        public static TValue Get(TKey key)
        {            
            return tree.Search(key);
        }

        /// <summary>
        /// Puts the <paramref name="value"/> into the cache.
        /// </summary>
        /// <param name="key">The key to be used to store the value.</param>
        /// <param name="value">The value to be put into the cache.</param>
        public static void Put(TKey key, TValue value)
        {           
            tree = tree.Add(key, value);
        }        
    }
}
