using System;
using System.Collections.Concurrent;

namespace DbReader.Readers
{
    /// <summary>
    /// A class that is capable of 
    /// producing an <see cref="IInstanceReader{T}"/> based on a given <see cref="Type"/> and prefix.
    /// </summary>
    public class InstanceReaderFactory : IInstanceReaderFactory
    {
        private readonly ConcurrentDictionary<Tuple<Type, string>, object> readers = new ConcurrentDictionary<Tuple<Type, string>, object>();

        private readonly Func<Type, object> createReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceReaderFactory"/> class.
        /// </summary>
        /// <param name="createReader">A function delegate used to create an <see cref="IInstanceReader{T}"/> based on a given <see cref="Type"/>.</param>
        public InstanceReaderFactory(Func<Type, object> createReader)
        {
            this.createReader = createReader;
        }

        /// <summary>
        /// Gets an <see cref="IInstanceReader{T}"/> for the given <paramref name="type"/> and <paramref name="prefix"/>.
        /// </summary>
        /// <param name="type">The type for which to get an <see cref="IInstanceReader{T}"/>.</param>
        /// <param name="prefix">The prefix for which to get an <see cref="IInstanceReader{T}"/>.</param>
        /// <returns></returns>
        public object GetInstanceReader(Type type, string prefix)
        {
            return readers.GetOrAdd(Tuple.Create(type, prefix), t => createReader(t.Item1));
        }
    }
}