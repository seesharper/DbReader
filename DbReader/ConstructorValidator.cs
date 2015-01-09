namespace DbReader
{
    using System;
    using System.Reflection;

    public class ConstructorValidator : IConstructorSelector
    {
        private readonly IConstructorSelector constructorSelector;

        public ConstructorValidator(IConstructorSelector constructorSelector)
        {
            this.constructorSelector = constructorSelector;
        }

        public ConstructorInfo Execute(Type type)
        {
            var constructor = constructorSelector.Execute(type);
            if (constructor == null)
            {
                throw new ArgumentOutOfRangeException("type", ErrorMessages.ConstructorNotFound.FormatWith(type));
            }
            return constructor;
        }
    }
}