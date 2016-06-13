namespace DbReader.Construction
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;
    using Selectors;

    /// <summary>
    /// Base class containing the common methods for <see cref="IReaderMethodBuilder{T}"/>
    /// implementations.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam> 
    public abstract class ReaderMethodBuilder<T> : IReaderMethodBuilder<T> 
    {        
        private readonly IMethodSkeletonFactory methodSkeletonFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for
        /// creating an <see cref="IMethodSkeleton"/>.</param>
        /// <param name="methodSelector">The <see cref="IMethodSelector"/> that is responsible for selecting the <see cref="IDataRecord"/> 
        /// get method that corresponds to the property type.</param>
        protected ReaderMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IMethodSelector methodSelector)
        {
            MethodSelector = methodSelector;
            this.methodSkeletonFactory = methodSkeletonFactory;
        }
        
        /// <summary>
        /// Gets the <see cref="IMethodSelector"/> that is responsible for selecting the <see cref="IDataRecord"/> 
        /// get method that corresponds to the property type.
        /// </summary>
        protected IMethodSelector MethodSelector { get; private set; }

        /// <summary>
        /// Creates a new method that initializes and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.
        /// </summary>
        /// <returns>A delegate that creates and populates an instance of <typeparamref name="T"/> from an 
        /// <see cref="IDataRecord"/>.</returns>
        public Func<IDataRecord, int[], T> CreateMethod()
        {
            var methodSkeleton = GetMethodSkeleton();
            var il = methodSkeleton.GetGenerator();
            BuildMethod(il);
            return CreateDelegate(methodSkeleton);
        }

        /// <summary>
        /// Builds the methods required to create and populate an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="il">THe <see cref="ILGenerator"/> used to emit instructions.</param>
        protected abstract void BuildMethod(ILGenerator il);

        /// <summary>
        /// Emits a check to see if the current ordinal equals -1.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> used to emit instructions.</param>
        /// <param name="index">The index of the ordinal to check.</param>
        /// <param name="trueLabel">The <see cref="Label"/> that represents where to jump if the ordinal value equals -1.</param>
        protected void EmitCheckForValidOrdinal(ILGenerator il, int index, Label trueLabel)
        {
            LoadOrdinal(il, index);
            LoadIntegerValueOfMinusOne(il);
            EmitCompareValues(il);
            EmitGotoEndLabelIfValueIsTrue(il, trueLabel);
        }

        /// <summary>
        /// Emits a check to see if the current value to be read from the <see cref="IDataRecord"/> is <see cref="DBNull"/>
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> used to emit instructions.</param>
        /// <param name="index">The index of the ordinal to check.</param>
        /// <param name="trueLabel">The <see cref="Label"/> that represents where to jump if 
        /// the value about to be read from the <see cref="IDataRecord"/> is <see cref="DBNull"/>.</param>
        protected void EmitCheckForDbNull(ILGenerator il, int index, Label trueLabel)
        {
            LoadDataRecord(il);
            LoadOrdinal(il, index);
            EmitCallIsDbNullMethod(il);
            EmitGotoEndLabelIfValueIsTrue(il, trueLabel);
        }

        /// <summary>
        /// Emits the code needed to retrieve the requested value from the <see cref="IDataRecord"/> instance.
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> used to emit instructions.</param>
        /// <param name="index">The index of the current ordinal.</param>
        /// <param name="getMethod">The get method to be used to retrieve the value.</param>
        /// <param name="targetType">The <see cref="Type"/> of the target <see cref="PropertyInfo"/> or <see cref="ParameterInfo"/>.</param>
        protected void EmitGetValue(ILGenerator il, int index, MethodInfo getMethod, Type targetType)
        {
            if (targetType.IsNullable())
            {
                EmitGetNullableValue(il, index, getMethod, targetType);
            }
            else
            {
                EmitGetNonNullableValue(il, index, getMethod);
            }
        }

        /// <summary>
        /// Emits a <see cref="OpCodes.Ret"/> instruction into the current method body. 
        /// </summary>
        /// <param name="il">The <see cref="ILGenerator"/> used to emit instructions.</param>
        protected void EmitReturn(ILGenerator il)
        {
            il.Emit(OpCodes.Ret);
        }

        private static void EmitGetNullableValue(ILGenerator il, int index, MethodInfo getMethod, Type targetType)
        {
            var local = il.DeclareLocal(targetType);
            il.Emit(OpCodes.Ldloca, local);
            LoadDataRecord(il);
            LoadOrdinal(il, index);
            EmitCallGetMethod(il, getMethod);
            var nullableConstructor = GetNullableConstructor(targetType.GetUnderlyingType());
            il.Emit(OpCodes.Call, nullableConstructor);
            il.Emit(OpCodes.Ldloc, local);
        }

        private static void EmitGetNonNullableValue(ILGenerator il, int index, MethodInfo getMethod)
        {
            LoadDataRecord(il);
            LoadOrdinal(il, index);
            EmitCallGetMethod(il, getMethod);
        }

        private static ConstructorInfo GetNullableConstructor(Type type)
        {
            return typeof(Nullable<>).MakeGenericType(type).GetConstructor(new[] { type });
        }

        private static void EmitCallGetMethod(ILGenerator il, MethodInfo getMethod)
        {
            il.Emit(getMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt, getMethod);
        }

        private static void EmitCallIsDbNullMethod(ILGenerator il)
        {
            il.Emit(OpCodes.Callvirt, GetIsDbNullMethod());
        }

        private static MethodInfo GetIsDbNullMethod()
        {
            return typeof(IDataRecord).GetMethod("IsDBNull");
        }

        private static void LoadOrdinal(ILGenerator il, int index)
        {
            il.Emit(OpCodes.Ldarg_1);
            il.EmitFastInt(index);
            il.Emit(OpCodes.Ldelem_I4);
        }

        private static void LoadIntegerValueOfMinusOne(ILGenerator il)
        {
            il.EmitFastInt(-1);
        }

        private static void EmitCompareValues(ILGenerator il)
        {
            il.Emit(OpCodes.Ceq);
        }

        private static void EmitGotoEndLabelIfValueIsTrue(ILGenerator il, Label endLabel)
        {
            il.Emit(OpCodes.Brtrue, endLabel);
        }

        private static Func<IDataRecord, int[], T> CreateDelegate(IMethodSkeleton methodSkeleton)
        {
            return (Func<IDataRecord, int[], T>)methodSkeleton.CreateDelegate(typeof(Func<IDataRecord, int[], T>));
        }
       
        private static void LoadDataRecord(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
        }

        private IMethodSkeleton GetMethodSkeleton()
        {
            var methodSkeleton = methodSkeletonFactory.GetMethodSkeleton(
                "ReaderDynamicMethod",
                typeof(T),
                new[] { typeof(IDataRecord), typeof(int[]) });
            return methodSkeleton;
        }
    }
}