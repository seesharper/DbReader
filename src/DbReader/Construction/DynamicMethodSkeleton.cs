namespace DbReader.Construction
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// A <see cref="IMethodSkeleton"/> that uses the <see cref="DynamicMethod"/> class.
    /// </summary>
    public class DynamicMethodSkeleton : IMethodSkeleton
    {
        private readonly DynamicMethod dynamicMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMethodSkeleton"/> class.
        /// </summary>
        /// <param name="returnType">The return type of the dynamic method.</param>
        /// <param name="parameterTypes">The parameter types of the dynamic method.</param>
        public DynamicMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Module module)
        {
            dynamicMethod = new DynamicMethod(
                name,
                returnType,
                parameterTypes,
                module);
        }

        public DynamicMethodSkeleton(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            dynamicMethod = new DynamicMethod(
                name,
                returnType,
                parameterTypes,
                owner);
        }

        /// <summary>
        /// Gets the <see cref="ILGenerator"/> for this method.
        /// </summary>
        /// <returns><see cref="ILGenerator"/>.</returns>
        public ILGenerator GetGenerator()
        {
            return dynamicMethod.GetILGenerator();
        }

        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
        /// <returns>A delegate of the specified type, which can be used to execute the dynamic method.</returns>
        public Delegate CreateDelegate(Type delegateType)
        {
            return dynamicMethod.CreateDelegate(delegateType);
        }
    }
}