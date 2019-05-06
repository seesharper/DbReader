using System;
using Shouldly;
using Xunit;

namespace DbReader.Tests
{
    public class UtilityTests
    {

        [Fact]
        public void ShouldThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => Require.IsNotNull<string>(null, "someParameter"));
        }

        [Fact]
        public void ShouldThrowInvalidOperationException()
        {
            Should.Throw<InvalidOperationException>(() => Ensure.IsNotNull<string>(null, "some message"));
        }
    }
}