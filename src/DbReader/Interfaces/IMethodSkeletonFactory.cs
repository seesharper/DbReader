namespace DbReader.Interfaces
{
    using System;

    /// <summary>
    /// Represents a class that is capable of providing an <see cref="IMethodSkeleton"/> instance.
    /// </summary>
    public interface IMethodSkeletonFactory
    {
        /// <summary>
        /// Gets an <see cref="IMethodSkeleton"/> instance.
        /// </summary>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        /// <returns>An <see cref="IMethodSkeleton"/> instance.</returns>
        IMethodSkeleton GetMethodSkeleton(Type returnType, Type[] parameterTypes);
    }
}