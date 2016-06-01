namespace DbReader.Readers
{
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;

    using DbReader.Interfaces;

    /// <summary>
    /// A <see cref="IReaderMethodBuilder{T}"/> that builds a method that creates and populates
    /// an instance of <typeparamref name="T"/> using constructor arguments.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam>
    public class ConstructorReaderMethodBuilder<T> : ReaderMethodBuilder<T> 
    {
        private readonly IConstructorSelector constructorSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorReaderMethodBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeletonFactory">The <see cref="IMethodSkeletonFactory"/> that is responsible for
        /// creating an <see cref="IMethodSkeleton"/>.</param>
        /// <param name="methodSelector">The <see cref="IMethodSelector"/> that is responsible for selecting the <see cref="IDataRecord"/> 
        /// get method that corresponds to the property type.</param>
        /// <param name="firstConstructorSelector">The <see cref="IConstructorSelector"/> that is responsible for selecting the first available constructor.</param>      
        public ConstructorReaderMethodBuilder(IMethodSkeletonFactory methodSkeletonFactory, IMethodSelector methodSelector, IConstructorSelector firstConstructorSelector)
            : base(methodSkeletonFactory, methodSelector)
        {
            constructorSelector = firstConstructorSelector;
        }

        /// <summary>
        /// Builds the methods required to create and populate an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="il">THe <see cref="ILGenerator"/> used to emit instructions.</param>
        protected override void BuildMethod(ILGenerator il)
        {
            var constructor = GetConstructor();
            LoadConstructorArguments(il, constructor);
            EmitNewInstance(il, constructor);
            EmitReturn(il);
        }

        private void LoadConstructorArguments(ILGenerator il, ConstructorInfo constructor)
        {
            var tryLoadNullValue = il.DefineLabel();
            var end = il.DefineLabel();

            ParameterInfo[] parameters = constructor.GetParameters();

            for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
            {
                EmitCheckForValidOrdinal(il, parameterIndex, tryLoadNullValue);
                EmitCheckForDbNull(il, parameterIndex, tryLoadNullValue);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                LoadDataRecordValue(il, parameters[i], i);
            }

            EmitGoto(il, end);
            il.MarkLabel(tryLoadNullValue);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(end);
        }

        private void LoadDataRecordValue(ILGenerator il, ParameterInfo parameter, int index)
        {
            MethodInfo getMethod = MethodSelector.Execute(parameter.ParameterType);
            EmitGetValue(il, index, getMethod, parameter.ParameterType);
        }

        private void EmitNewInstance(ILGenerator il, ConstructorInfo constructor)
        {
            il.Emit(OpCodes.Newobj, constructor);
        }

        private void EmitGoto(ILGenerator il, Label falseLabel)
        {
            il.Emit(OpCodes.Br, falseLabel);
        }

        private ConstructorInfo GetConstructor()
        {
            return constructorSelector.Execute(typeof(T));
        }
    }
}