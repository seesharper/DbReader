namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;
    using Construction;
    using LightInject;
    using Extensions;
    using Readers;
    using Selectors;

    /// <summary>
    /// Extends the <see cref="IDataReader"/> interface.
    /// </summary>
    public static class DataReaderExtensions
    {
        private static Lazy<IServiceContainer> containerFactory = new Lazy<IServiceContainer>(CreateContainer);

        private static IServiceContainer CreateContainer()
        {
            var container = new ServiceContainer();
            container.RegisterFrom<CompositionRoot>();
            return container;
        }

        /// <summary>
        /// Reads the data from the given <paramref name="dataReader"/>
        /// and translates the data into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to be created from the reader.</typeparam>
        /// <param name="dataReader">The target <see cref="IDataReader"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the data translated into objects.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Read<T>(this IDataReader dataReader)
        {
            return TypeEvaluator<T>.HasNavigationProperties
                ? ReadWithNavigationProperties<T>(dataReader)
                : ReadWithoutNavigationProperties<T>(dataReader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<T> ReadWithNavigationProperties<T>(IDataReader dataReader)
        {
            List<T> result = new List<T>();
            var hasRows = dataReader.Read();
            if (!hasRows)
            {
                return result;
            }

            var container = containerFactory.Value;
            using (container.BeginScope())
            {
                var instanceReader = container.GetInstance<IInstanceReader<T>>();
                result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                while (dataReader.Read())
                {
                    result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                }
            }
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<T> ReadWithoutNavigationProperties<T>(IDataReader dataReader)
        {
            var result = new List<T>();

            var hasRows = dataReader.Read();
            if (!hasRows)
            {
                return result;
            }

            var propertyReaderDelegate = PropertyReaderDelegateCache<T>.Get(SqlStatement.Current);
            if (propertyReaderDelegate == null)
            {
                var container = containerFactory.Value;
                using (container.BeginScope())
                {
                    var propertyReaderMethodBuilder =
                        container.GetInstance<IReaderMethodBuilder<T>>("PropertyReaderMethodBuilder");
                    var ordinalsSelector = container.GetInstance<IOrdinalSelector>();

                    propertyReaderDelegate = new PropertyReaderDelegate<T>()
                    {
                        Ordinals = ordinalsSelector.Execute(typeof(T), dataReader, string.Empty),
                        ReadMethod = propertyReaderMethodBuilder.CreateMethod()
                    };

                    PropertyReaderDelegateCache<T>.Put(SqlStatement.Current, propertyReaderDelegate);
                }
            }

            var ordinals = propertyReaderDelegate.Ordinals;
            var readMethod = propertyReaderDelegate.ReadMethod;

            result.Add(readMethod(dataReader, ordinals));
            while (dataReader.Read())
            {
                result.Add(readMethod(dataReader, ordinals));
            }

            return result;
        }

        internal static void SetContainer(IServiceContainer existingContainer)
        {
            containerFactory = new Lazy<IServiceContainer>(() => existingContainer);
        }
    }
}