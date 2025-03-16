#if NET7_0
namespace DbClient.Tests
{
    using System;
    using Construction;
    public class VerifiableMethodSkeletonFactory : IMethodSkeletonFactory
    {
        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes)
        {
            return new VerifiableMethodSkeleton(name, returnType, parameterTypes);
        }

        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            return new VerifiableMethodSkeleton(name, returnType, parameterTypes);
        }
    }
}
#endif