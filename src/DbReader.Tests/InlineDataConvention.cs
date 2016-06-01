namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Fixie;

    public class Conventions : TestAssembly
    {
        public Conventions()
        {
            Apply<LightInjectConvention>();
            Apply<InlineDataConvention>();
        }
    }

    internal class InlineDataConvention : LightInjectConvention    
    {
        public InlineDataConvention()
        {
            Methods.Where(m => m.IsDefined(typeof(InlineAttribute), true));
            Parameters.Add<InlineParameterSource>();
        }

        protected override bool UseMethodInjection
        {
            get
            {
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InlineAttribute : Attribute
    {
        public object[] Values { get; private set; }

        public InlineAttribute(params object[] values)
        {
            Values = values;            
        }
    }

    public class InlineParameterSource : ParameterSource
    {
        public IEnumerable<object[]> GetParameters(MethodInfo method)
        {
            var inlineAttributes = method.GetCustomAttributes(typeof(InlineAttribute)).Cast<InlineAttribute>();
            return inlineAttributes.Select(a => a.Values).ToArray();
        }
    }

}