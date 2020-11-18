using DbReader.LightInject;

namespace DbReader.Readers
{

    /// <summary>
    /// A class that is capable of creating an <see cref="IInstanceReader{T}"/> based on the given type.
    /// </summary>
    public class InstanceReaderFactory : IInstanceReaderFactory
    {
        private readonly IServiceFactory serviceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceReaderFactory"/> class.
        /// </summary>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> used to resolve <see cref="IInstanceReader{T}"/> instances.</param>
        internal InstanceReaderFactory(IServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        ///<inheritdoc/>
        public IInstanceReader<T> GetInstanceReader<T>(string prefix)
        {
            return serviceFactory.GetInstance<IInstanceReader<T>>();

            // NOTE: We used to cache the readers on the prefix. Have no idea why anymore;
            /* OLD CODE
                private readonly ConcurrentDictionary<Tuple<Type, string>, object> readers = new ConcurrentDictionary<Tuple<Type, string>, object>();
                return (IInstanceReader<T>)readers.GetOrAdd(Tuple.Create(typeof(T), prefix), t => serviceFactory.GetInstance<IInstanceReader<T>>());
            */
        }
    }
}