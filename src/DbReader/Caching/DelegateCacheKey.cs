namespace DbReader.Caching
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    public static class SqlStatement
    {
        [ThreadStatic]
        private static string current;

        public static string Current
        {
            get { return current; }
            set { current = value; }
        }
    }


    public static class SimpleCache<TDelegate>
    {
        private static ConcurrentDictionary<Tuple<Type, string, string>, TDelegate> cache =
            new ConcurrentDictionary<Tuple<Type, string, string>, TDelegate>(); 

        public static TDelegate GetOrAdd(Type type, string prefix, Func<TDelegate> delegateFactory)
        {
            var key = Tuple.Create(type, SqlStatement.Current, prefix);
            return cache.GetOrAdd(key, _ => delegateFactory());
        }
    }
}