﻿namespace DbReader
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    using DbReader.Interfaces;
    using DbReader.LightInject;

    public static class DataReaderExtensions
    {
        private static readonly IServiceContainer Container = new ServiceContainer();

        static DataReaderExtensions()
        {            
            Container.RegisterFrom<CompositionRoot>();
        }

        public static IEnumerable<T> Read<T>(IDataReader dataReader)
        {
            var result = new Collection<T>();
            using (Container.BeginScope())
            {
                var instanceReader = Container.GetInstance<IInstanceReader<T>>();
                while (dataReader.Read())
                {
                    result.TryAdd(instanceReader.Read(dataReader, string.Empty));
                }
            }
            return result;
        }        
    }
}