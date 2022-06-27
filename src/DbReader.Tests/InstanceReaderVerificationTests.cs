#if NET6_0
namespace DbReader.Tests
{
    using System;
    using System.IO;
    using Construction;
    using LightInject;
    using IMethodSkeleton = Construction.IMethodSkeleton;


    public class InstanceReaderVerificationTests : InstanceReaderTests
    {
        internal static void Configure(IServiceContainer container)
        {
            container.Register<IMethodSkeletonFactory, VerifiableMethodSkeletonFactory>(new PerContainerLifetime());
        }

        internal override void Configure(IServiceRegistry serviceRegistry)
        {
            base.Configure(serviceRegistry);
            serviceRegistry.Register<IMethodSkeletonFactory, VerifiableMethodSkeletonFactory>(new PerContainerLifetime());

        }
    }
}
#endif