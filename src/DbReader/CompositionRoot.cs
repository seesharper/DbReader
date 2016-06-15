namespace DbReader
{
    using System;
    using System.Collections;
    using System.Reflection;
    using Construction;
    using Database;
    using DbReader.Caching;
    using DbReader.Interfaces;
    using DbReader.LightInject;
    using DbReader.Readers;
    using Mapping;
    using Selectors;
    using IConstructorSelector = Selectors.IConstructorSelector;
    using IMethodSkeleton = Construction.IMethodSkeleton;
    using IPropertySelector = Selectors.IPropertySelector;

    internal class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IMethodSkeletonFactory, MethodSkeletonFactory>(new PerContainerLifetime());
            serviceRegistry.Register<string, Type, Type[], IMethodSkeleton>((factory, name, returnType, parameterTypes) => new DynamicMethodSkeleton(name, returnType, parameterTypes));

            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(PropertyReaderMethodBuilder<>), "PropertyReaderMethodBuilder", new PerContainerLifetime());
            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(ConstructorReaderMethodBuilder<>), "ConstructorReaderMethodBuilder", new PerContainerLifetime());
            serviceRegistry.Decorate(typeof(IReaderMethodBuilder<>), typeof(CachedReaderMethodBuilder<>));

            serviceRegistry.Register<IFieldSelector, FieldSelector>(new PerScopeLifetime());
            serviceRegistry.Decorate<IFieldSelector, CachedFieldSelector>();

            serviceRegistry.Register<IPropertySelector, SimplePropertySelector>("SimplePropertySelector", new PerContainerLifetime());
            serviceRegistry.Register<IPropertySelector, ManyToOnePropertySelector>("ManyToOnePropertySelector", new PerContainerLifetime());
            serviceRegistry.Register<IPropertySelector, OneToManyPropertySelector>("OneToManyPropertySelector", new PerContainerLifetime());
            serviceRegistry.Decorate<IPropertySelector, CachedPropertySelector>();



            serviceRegistry.Register<IMethodSelector, MethodSelector>(new PerContainerLifetime());
            //Caching?


            serviceRegistry.Register<IConstructorSelector, ParameterlessConstructorSelector>("ParameterlessConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Register<IConstructorSelector, FirstConstructorSelector>("FirstConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Decorate(typeof(IConstructorSelector), typeof(ConstructorValidator), sr => sr.ImplementingType == typeof(ParameterlessConstructorSelector));

            serviceRegistry.Register<IPrefixResolver, PrefixResolver>(new PerScopeLifetime());

            serviceRegistry.Register(typeof(IInstanceReader<>), typeof(InstanceReader<>), new PerScopeLifetime());
            serviceRegistry.Decorate(typeof(IInstanceReader<>), typeof(CachedInstanceReader<>));
            serviceRegistry.Register(typeof(IInstanceReaderMethodBuilder<>), typeof(InstanceReaderMethodBuilder<>), new PerScopeLifetime());
            serviceRegistry.Decorate(typeof(IInstanceReaderMethodBuilder<>), typeof(CachedInstanceReaderMethodBuilder<>));

            serviceRegistry.Register<Func<Type, object>>(factory => type => factory.GetInstance(type));
            serviceRegistry.Register<Func<Type, IReaderMethodBuilder<IStructuralEquatable>>>(
                factory => type => CreateConstructorReaderMethodBuilder(type, factory), new PerContainerLifetime());

            serviceRegistry.Register<IPropertyMapper, KeyPropertyMapper>("KeyPropertyMapper", new PerScopeLifetime());
            serviceRegistry.Decorate<IPropertyMapper, PropertyTypeValidator>();
            serviceRegistry.Register<IPropertyMapper, PropertyMapper>("PropertyMapper", new PerScopeLifetime());
            serviceRegistry.Decorate<IPropertyMapper, CachedPropertyMapper>();
            serviceRegistry.Decorate(typeof(IPropertyMapper), typeof(KeyPropertyMapperValidator), sr => sr.ImplementingType == typeof(KeyPropertyMapper));


            serviceRegistry.Register(factory => IsKeyProperty());


            serviceRegistry.Register<IKeyReader, KeyReader>(new PerScopeLifetime());
            serviceRegistry.Register<IKeyReaderMethodBuilder, KeyReaderMethodBuilder>(new PerScopeLifetime());
            serviceRegistry.Decorate<IKeyReaderMethodBuilder, CachedKeyReaderMethodBuilder>();


            serviceRegistry.Register(typeof(IManyToOneMethodBuilder<>), typeof(ManyToOneMethodBuilder<>), new PerScopeLifetime());

            serviceRegistry.Register(typeof(IOneToManyMethodBuilder<>), typeof(OneToManyMethodBuilder<>), new PerScopeLifetime());
            serviceRegistry.Decorate(typeof(IOneToManyMethodBuilder<>), typeof(CachedOneToManyMethodBuilder<>));



            serviceRegistry.Register<IOrdinalSelector, OrdinalSelector>();

            serviceRegistry.Register(factory => DbReaderOptions.ParameterParser, new PerContainerLifetime());

            serviceRegistry.Register<IDbCommandFactory, DbCommandFactory>(new PerContainerLifetime());

            serviceRegistry.Register<IArgumentMapper, ArgumentMapper>(new PerContainerLifetime());

            serviceRegistry.Register<IPropertySelector, ReadablePropertySelector>("ReadablePropertySelector", new PerContainerLifetime());

            serviceRegistry.Register<ICacheKeyFactory, CacheKeyFactory>(new PerContainerLifetime());            
        }

        private static Func<PropertyInfo, bool> IsKeyProperty()
        {
            return
                p =>
                p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase);
        }

        private static IReaderMethodBuilder<IStructuralEquatable> CreateConstructorReaderMethodBuilder(Type type, IServiceFactory factory)
        {
            Type closedGenericMethodBuilderType = typeof(IReaderMethodBuilder<>).MakeGenericType(type);
            return
                (IReaderMethodBuilder<IStructuralEquatable>)
                factory.GetInstance(closedGenericMethodBuilderType, "ConstructorReaderMethodBuilder");
        }
    }
}