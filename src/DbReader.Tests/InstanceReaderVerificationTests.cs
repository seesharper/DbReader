namespace DbReader.Tests
{
    using System;
    using System.IO;
    using LightInject;
    using IMethodSkeleton = Construction.IMethodSkeleton;


    public class InstanceReaderVerificationTests : InstanceReaderTests
    {
        public static void Configure(IServiceContainer container)
        {            
            container.Register<string, Type, Type[], IMethodSkeleton>((factory, name, returnType, parameterTypes) => new MethodBuilderMethodSkeleton(name, returnType, parameterTypes));
        }
    }
}