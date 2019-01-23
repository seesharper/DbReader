#if NET462
namespace DbReader.Tests
{
    using System;
    using Construction;
    public class MethodBuilderMethodSkeletonFactory : IMethodSkeletonFactory
    {
        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes)
        {
            return new MethodBuilderMethodSkeleton(name, returnType, parameterTypes);
        }

        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            return new MethodBuilderMethodSkeleton(name, returnType, parameterTypes);
        }
    }
}
#endif