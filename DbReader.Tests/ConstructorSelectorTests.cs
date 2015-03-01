namespace DbReader.Tests
{
    using System;

    using DbReader.Interfaces;

    using Should;
    using Should.Core.Assertions;

    using Xunit.Extensions;

    public class ConstructorSelectorTests
    {
        [Theory, InjectData]
        public void Execute_PublicParameterLess_ReturnsConstructor(IConstructorSelector parameterlessConstructorSelector)
        {
            parameterlessConstructorSelector.Execute(typeof(ClassWithPublicParameteressConstructor)).ShouldNotBeNull();
        }

        [Theory, InjectData]
        public void Execute_PrivateParameterLess_ThrowsException(IConstructorSelector parameterlessConstructorSelector)
        {
            Assert.Throws<InvalidOperationException>(
                () => parameterlessConstructorSelector.Execute(typeof(ClassWithPrivateParameterlessConstructor)));
        }

        [Theory]
        [InjectData]
        public void Execute_TupleClass_ReturnsConstructor(IConstructorSelector firstConstructorSelector)
        {
            firstConstructorSelector.Execute(typeof(Tuple<int>)).ShouldNotBeNull();
        }
    }

    public class ClassWithPublicParameteressConstructor
    {
        public ClassWithPublicParameteressConstructor()
        {
        }
    }

    public class ClassWithPrivateParameterlessConstructor
    {
        private ClassWithPrivateParameterlessConstructor()
        {
        }
    }
}