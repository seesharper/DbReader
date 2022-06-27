using System.Runtime.InteropServices;
using Xunit;

namespace DbReader.Tests
{
    public class OnlyOnIntelFactAttribute : FactAttribute
    {
        public OnlyOnIntelFactAttribute()
        {
            if (RuntimeInformation.OSArchitecture != Architecture.X64)
            {
                Skip = "Can run only on Intel X64";
            }
        }
    }
}