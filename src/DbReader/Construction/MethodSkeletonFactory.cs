namespace DbReader.Construction
{
    using System;

    /// <summary>
    /// A class that is capable of providing an <see cref="IMethodSkeleton"/> instance.
    /// </summary>
    public class MethodSkeletonFactory : IMethodSkeletonFactory
    {
        private readonly Func<string, Type, Type[], IMethodSkeleton> factoryDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodSkeletonFactory"/> class.
        /// </summary>
        /// <param name="factoryDelegate">A factory delegate used to create an <see cref="IMethodSkeleton"/> instance</param>
        public MethodSkeletonFactory(Func<string, Type, Type[], IMethodSkeleton> factoryDelegate)
        {
            this.factoryDelegate = factoryDelegate;
        }

        /// <summary>
        /// Gets an <see cref="IMethodSkeleton"/> instance.
        /// </summary>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        /// <returns>An <see cref="IMethodSkeleton"/> instance.</returns>
        public IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes)
        {
            return factoryDelegate(name, returnType, parameterTypes);
        }
    }
}