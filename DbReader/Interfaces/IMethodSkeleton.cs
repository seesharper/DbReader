namespace DbReader.Interfaces
{
    using System;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents the skeleton of a dynamic method.
    /// </summary>
    public interface IMethodSkeleton
    {
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> for this method.
        /// </summary>
        /// <returns><see cref="ILGenerator"/>.</returns>
        ILGenerator GetGenerator();
        
        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method.</returns>
        Delegate CreateDelegate(Type delegateType); 
    }
}