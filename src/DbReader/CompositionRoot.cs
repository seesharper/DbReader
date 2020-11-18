namespace DbReader
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using Construction;
    using Database;
    using Interfaces;
    using LightInject;
    using Readers;
    using Mapping;
    using Selectors;
    using IConstructorSelector = Selectors.IConstructorSelector;
    using IPropertySelector = Selectors.IPropertySelector;

    internal class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            RegisterSingletonServices(serviceRegistry);
            RegisterScopedServices(serviceRegistry);
        }

        private static IReaderMethodBuilder<IStructuralEquatable> CreateConstructorReaderMethodBuilder(Type type, IServiceFactory factory)
        {
            Type closedGenericMethodBuilderType = typeof(IReaderMethodBuilder<>).MakeGenericType(type);
            return
                (IReaderMethodBuilder<IStructuralEquatable>)
                factory.GetInstance(closedGenericMethodBuilderType, "ConstructorReaderMethodBuilder");
        }


        private static void RegisterSingletonServices(IServiceRegistry registry)
        {
            registry
                .Register<IMethodSkeletonFactory, DynamicMethodSkeletonFactory>(new PerContainerLifetime())
                .Register(typeof(IReaderMethodBuilder<>), typeof(PropertyReaderMethodBuilder<>),
                    "PropertyReaderMethodBuilder", new PerContainerLifetime())
                .Register(typeof(IReaderMethodBuilder<>), typeof(ConstructorReaderMethodBuilder<>),
                    "ConstructorReaderMethodBuilder", new PerContainerLifetime())
                .Decorate(typeof(IReaderMethodBuilder<>), typeof(CachedReaderMethodBuilder<>))

                .Register<IArgumentParser, ArgumentParser>(new PerContainerLifetime())
                .Register<IArgumentParserMethodBuilder, ArgumentParserMethodBuilder>(new PerContainerLifetime())
                .Decorate<IArgumentParserMethodBuilder, CachedArgumentParserMethodBuilder>()

                .Register<IPropertySelector, ReadablePropertySelector>("ReadablePropertySelector", new PerContainerLifetime())
                .Register<IPropertySelector, SimplePropertySelector>("SimplePropertySelector", new PerContainerLifetime())
                .Register<IPropertySelector, ManyToOnePropertySelector>("ManyToOnePropertySelector", new PerContainerLifetime())
                .Register<IPropertySelector, OneToManyPropertySelector>("OneToManyPropertySelector", new PerContainerLifetime())
                .Decorate(typeof(IPropertySelector), typeof(OneToManyPropertyValidator), sr => sr.ImplementingType == typeof(OneToManyPropertySelector))
                .Decorate(typeof(IPropertySelector), typeof(ReadableArgumentPropertiesValidator), sr => sr.ImplementingType == typeof(ReadablePropertySelector))
                .Decorate<IPropertySelector, CachedPropertySelector>()

                .Register<Func<Type, IReaderMethodBuilder<IStructuralEquatable>>>(
                    factory => type => CreateConstructorReaderMethodBuilder(type, factory), new PerContainerLifetime())

                .Register<IMethodSelector, MethodSelector>(new PerContainerLifetime())
            //Caching?


                .Register<IConstructorSelector, ParameterlessConstructorSelector>("ParameterlessConstructorSelector", new PerContainerLifetime())
                .Register<IConstructorSelector, FirstConstructorSelector>("FirstConstructorSelector", new PerContainerLifetime())
                .Decorate(typeof(IConstructorSelector), typeof(ConstructorValidator), sr => sr.ImplementingType == typeof(ParameterlessConstructorSelector))

                .Register(factory => DbReaderOptions.ParameterParser, new PerContainerLifetime())
                .RegisterSingleton<IParameterMatcher, ParameterMatcher>();
        }

        private void RegisterScopedServices(IServiceRegistry registry)
        {
            registry
                .Register<IFieldSelector, FieldSelector>(new PerScopeLifetime())
                .Decorate<IFieldSelector, CachedFieldSelector>()

                .Register<IPrefixResolver, PrefixResolver>(new PerScopeLifetime())

                .Register<IInstanceReaderFactory>(f => new InstanceReaderFactory(f.GetInstance), new PerScopeLifetime())
                .Register<IGenericInstanceReaderFactory>(f => new GenericInstanceReaderFactory(f.GetInstance), new PerScopeLifetime())

                //.Register(typeof(IInstanceReader<>), typeof(InstanceReader<>))
                .Register(typeof(IInstanceReader<>), typeof(InstanceReader<>), new PerScopeLifetime())
                .Decorate(typeof(IInstanceReader<>), typeof(CachedInstanceReader<>))

                .Register(typeof(IInstanceReaderMethodBuilder<>), typeof(InstanceReaderMethodBuilder<>), new PerScopeLifetime())
                .Decorate(typeof(IInstanceReaderMethodBuilder<>), typeof(CachedInstanceReaderMethodBuilder<>))


                .Register<IKeyPropertyMapper, KeyPropertyMapper>(new PerScopeLifetime())
                .Decorate<IKeyPropertyMapper, KeyPropertyMapperValidator>()



                .Register<IPropertyMapper, PropertyMapper>(new PerScopeLifetime())
                .Decorate<IPropertyMapper, PropertyTypeValidator>()
                .Decorate<IPropertyMapper, CachedPropertyMapper>()



                .Register<IKeyReader, KeyReader>(new PerScopeLifetime())
                .Register<IKeyReaderMethodBuilder, KeyReaderMethodBuilder>(new PerScopeLifetime())
                .Decorate<IKeyReaderMethodBuilder, CachedKeyReaderMethodBuilder>()

                .Register(typeof(IManyToOneMethodBuilder<>), typeof(ManyToOneMethodBuilder<>), new PerScopeLifetime())
                .Register(typeof(IOneToManyMethodBuilder<>), typeof(OneToManyMethodBuilder<>), new PerScopeLifetime())
                .Decorate(typeof(IOneToManyMethodBuilder<>), typeof(CachedOneToManyMethodBuilder<>))

                .Register<IOrdinalSelector, OrdinalSelector>(new PerScopeLifetime());

        }

    }
}