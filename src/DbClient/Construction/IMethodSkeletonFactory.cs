namespace DbClient.Construction
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
        /// <param name="name">The name of the method.</param>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        /// <returns>An <see cref="IMethodSkeleton"/> instance.</returns>
        IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes);

        /// <summary>
        /// Gets an <see cref="IMethodSkeleton"/> instance.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        /// <param name="owner">A <see cref="Type"/> with which the dynamic method is logically associated. The dynamic method has access to all members of the type.</param>
        /// <returns>An <see cref="IMethodSkeleton"/> instance.</returns>
        IMethodSkeleton GetMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Type owner);
    }
}