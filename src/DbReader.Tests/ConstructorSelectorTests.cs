namespace DbReader.Tests
{
    using System;

    using DbReader.Interfaces;
    using Selectors;
    using Shouldly;
    using Xunit;
    using DbReader.LightInject;

    public class ConstructorSelectorTests : ContainerFixture
    {
        private readonly Selectors.IConstructorSelector parameterlessConstructorSelector;
        private readonly Selectors.IConstructorSelector firstConstructorSelector;

        public ConstructorSelectorTests()
        {
            parameterlessConstructorSelector = ServiceFactory.GetInstance<Selectors.IConstructorSelector>("ParameterlessConstructorSelector");
            firstConstructorSelector = ServiceFactory.GetInstance<Selectors.IConstructorSelector>("firstConstructorSelector");
        }

        [Fact]
        public void Execute_PublicParameterLess_ReturnsConstructor()
        {
            parameterlessConstructorSelector.Execute(typeof(ClassWithPublicParameteressConstructor)).ShouldNotBeNull();
        }

        [Fact]
        public void Execute_PrivateParameterLess_ThrowsException()
        {
            Should.Throw<InvalidOperationException>(
                () => parameterlessConstructorSelector.Execute(typeof(ClassWithPrivateParameterlessConstructor)));
        }

        [Fact]
        public void Execute_TupleClass_ReturnsConstructor()
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