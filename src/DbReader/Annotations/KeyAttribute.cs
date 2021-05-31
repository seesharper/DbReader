using System;

namespace DbReader.Annotations
{
    [AttributeUsage(validOn: AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class KeyAttribute : Attribute
    {
    }
}