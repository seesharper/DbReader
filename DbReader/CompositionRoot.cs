namespace DbReader
{
    using System;

    using DbReader.LightInject;
    using DbReader.Readers;

    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {                        
            serviceRegistry.Register<IMethodSkeletonFactory, MethodSkeletonFactory>(new PerContainerLifetime());
            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(PropertyReaderMethodBuilder<>), "PropertyReaderMethodBuilder");
            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(KeyReaderMethodBuilder<>), "KeyReaderMethodBuilder");
            serviceRegistry.Register(typeof(IReader<>), typeof(PropertyReader<>), "PropertyReader");            
            serviceRegistry.Register<Type, Type[], IMethodSkeleton>((factory, returnType, parameterTypes) => new DynamicMethodSkeleton(returnType, parameterTypes));
            serviceRegistry.Register<IFieldSelector, FieldSelector>(new PerScopeLifetime());
            serviceRegistry.Register<IPropertySelector, SimplePropertySelector>("SimplePropertySelector", new PerContainerLifetime());
            serviceRegistry.Register<IPropertySelector, ManyToOnePropertySelector>("ManyToOnePropertySelector", new PerContainerLifetime());
            
            serviceRegistry.Register<IPropertyMapper>(factory => new PropertyMapper(factory.GetInstance<IPropertySelector>("SimplePropertySelector"), factory.GetInstance<IFieldSelector>()), new PerScopeLifetime());
            serviceRegistry.Register<IMethodSelector, MethodSelector>(new PerContainerLifetime());

            serviceRegistry.Register<IConstructorSelector, ParameterlessConstructorSelector>("ParameterlessConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Register<IConstructorSelector, FirstConstructorSelector>("FirstConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Decorate(typeof(IConstructorSelector), typeof(ConstructorValidator), sr => sr.ImplementingType == typeof(ParameterlessConstructorSelector));

            serviceRegistry.Register(typeof(IRelationMethodBuilder<>), typeof(ManyToOneMethodBuilder<>), "ManyToOneMethodBuilder", new PerScopeLifetime());
            serviceRegistry.Register<IPrefixResolver, PrefixResolver>(new PerScopeLifetime());           
        }
    }
}