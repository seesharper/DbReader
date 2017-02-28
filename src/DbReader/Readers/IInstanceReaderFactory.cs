namespace DbReader.Readers
{
    using System;
    using System.Collections.Concurrent;

    public interface IInstanceReaderFactory
    {
        object GetInstanceReader(Type type, string name);
    }

    public class InstanceReaderFactory : IInstanceReaderFactory
    {
        private readonly Func<Type, object> createReader;

        public InstanceReaderFactory(Func<Type, object> createReader)
        {
            this.createReader = createReader;
        }

        private readonly ConcurrentDictionary<Tuple<Type, string>, object> readers = new ConcurrentDictionary<Tuple<Type, string>, object>();

        public object GetInstanceReader(Type type, string prefix)
        {
            return readers.GetOrAdd(Tuple.Create(type, prefix), t => createReader(t.Item1));
        }
    }
}