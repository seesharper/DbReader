namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    using DbReader.Interfaces;
    using DbReader.LightInject;
    using Extensions;
    using Readers;

    public static class DataReaderExtensions
    {
        private static Lazy<IServiceContainer> containerFactory = new Lazy<IServiceContainer>(CreateContainer);
       
        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }

        public static IEnumerable<T> Read<T>(this IDataReader dataReader)
        {
            var result = new Collection<T>();
            var container = containerFactory.Value;
            using (container.BeginScope())
            {
                var instanceReader = container.GetInstance<IInstanceReader<T>>();
                while (dataReader.Read())
                {
                    result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                }
            }
            return result;
        }

        internal static void SetContainer(IServiceContainer existingContainer)
        {
            containerFactory = new Lazy<IServiceContainer>(() => existingContainer);
        }
    }
}