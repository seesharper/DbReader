#if NET462
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
            container.Register<IMethodSkeletonFactory, MethodBuilderMethodSkeletonFactory>(new PerContainerLifetime());
        }
    }
}
#endif