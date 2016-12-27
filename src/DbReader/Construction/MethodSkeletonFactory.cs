namespace DbReader.Construction
{
    using System;
    using System.Reflection;

    /// <summary>
    /// A class that is capable of providing an <see cref="IMethodSkeleton"/> instance.
    /// </summary>
    public class DynamicMethodSkeletonFactory : IMethodSkeletonFactory
    {               
        /// <summary>
        /// Gets an <see cref="IMethodSkeleton"/> instance.
        /// </summary>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        /// <returns>An <see cref="IMethodSkeleton"/> instance.</returns>
        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes)
        {
            return new DynamicMethodSkeleton(name, returnType, parameterTypes, typeof(IMethodSkeletonFactory).GetTypeInfo().Module);
        }

        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            return new DynamicMethodSkeleton(name, returnType, parameterTypes, owner);
        }
    }
}