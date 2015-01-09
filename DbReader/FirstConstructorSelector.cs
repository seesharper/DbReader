namespace DbReader
{
    using System;
    using System.Reflection;

    public class FirstConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Execute(Type type)
        {
            return type.GetConstructors()[0];
        }
    }
}