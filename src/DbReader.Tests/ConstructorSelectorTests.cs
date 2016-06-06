namespace DbReader.Tests
{
    using System;

    using DbReader.Interfaces;

    using Fixie;
    
    using Shouldly;

    public class ConstructorSelectorTests 
    {        
        public void Execute_PublicParameterLess_ReturnsConstructor(IConstructorSelector parameterlessConstructorSelector)
        {            
            parameterlessConstructorSelector.Execute(typeof(ClassWithPublicParameteressConstructor)).ShouldNotBeNull();
        }
        
        public void Execute_PrivateParameterLess_ThrowsException(IConstructorSelector parameterlessConstructorSelector)
        {
            Should.Throw<InvalidOperationException>(
                () => parameterlessConstructorSelector.Execute(typeof(ClassWithPrivateParameterlessConstructor)));
        }
       
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