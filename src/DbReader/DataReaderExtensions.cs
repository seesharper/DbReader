namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Construction;
    using Extensions;
    using LightInject;
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
            using (var scope = container.BeginScope())
            {
                var instanceReader = scope.GetInstance<IInstanceReader<T>>();
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

            if (typeof(T).IsSimpleType())
            {
                if (ValueConverter.CanConvert(typeof(T)))
                {
                    result.Add((T)ValueConverter.Convert<T>(dataReader, 0));
                    while (dataReader.Read())
                    {
                        result.Add((T)ValueConverter.Convert<T>(dataReader, 0));
                    }
                }
                else
                {
                    result.Add((T)dataReader.GetValue(0));
                    while (dataReader.Read())
                    {
                        result.Add((T)dataReader.GetValue(0));
                    }
                }

                return result;
            }

            var propertyReaderDelegate = PropertyReaderDelegateCache<T>.Get(SqlStatement.Current);
            if (propertyReaderDelegate == null)
            {
                var container = containerFactory.Value;
                using (var scope = container.BeginScope())
                {
                    var propertyReaderMethodBuilder =
                        scope.GetInstance<IReaderMethodBuilder<T>>("PropertyReaderMethodBuilder");
                    var ordinalsSelector = scope.GetInstance<IOrdinalSelector>();

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

        /// <summary>
        /// Reads the data from the given <paramref name="dataReader"/> asynchronously
        /// and translates the data into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to be created from the reader.</typeparam>
        /// <param name="dataReader">The target <see cref="DbDataReader"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing an <see cref="IEnumerable{T}"/> that represents the data translated into objects.</returns>
        public static Task<IEnumerable<T>> ReadAsync<T>(this DbDataReader dataReader, CancellationToken cancellationToken)
        {
            return TypeEvaluator<T>.HasNavigationProperties
                ? ReadWithNavigationPropertiesAsync<T>(dataReader, cancellationToken)
                : ReadWithoutNavigationPropertiesAsync<T>(dataReader, cancellationToken);
        }

        private static async Task<IEnumerable<T>> ReadWithNavigationPropertiesAsync<T>(DbDataReader dataReader, CancellationToken cancellationToken)
        {
            List<T> result = new List<T>();
            if (!await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                return result;
            }

            var container = containerFactory.Value;
            using (var scope = container.BeginScope())
            {
                var instanceReader = scope.GetInstance<IInstanceReader<T>>();
                result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                }
            }
            return result;
        }

        private static async Task<IEnumerable<T>> ReadWithoutNavigationPropertiesAsync<T>(DbDataReader dataReader, CancellationToken cancellationToken)
        {
            var result = new List<T>();

            if (!await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                return result;
            }

            if (typeof(T).IsSimpleType())
            {
                if (ValueConverter.CanConvert(typeof(T)))
                {
                    result.Add((T)ValueConverter.Convert<T>(dataReader, 0));
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        result.Add((T)ValueConverter.Convert<T>(dataReader, 0));
                    }
                }
                else
                {
                    result.Add((T)dataReader.GetValue(0));
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        result.Add((T)dataReader.GetValue(0));
                    }
                }

                return result;
            }

            var propertyReaderDelegate = PropertyReaderDelegateCache<T>.Get(SqlStatement.Current);
            if (propertyReaderDelegate == null)
            {
                var container = containerFactory.Value;
                using (var scope = container.BeginScope())
                {
                    var propertyReaderMethodBuilder =
                        scope.GetInstance<IReaderMethodBuilder<T>>("PropertyReaderMethodBuilder");
                    var ordinalsSelector = scope.GetInstance<IOrdinalSelector>();

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
            while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
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