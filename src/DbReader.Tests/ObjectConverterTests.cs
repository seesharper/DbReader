using DbReader.Construction;
using Shouldly;
using Xunit;

namespace DbReader.Tests
{
    public class ObjectConverterTests : ContainerFixture
    {
        private IObjectConverter objectConverter;

        [Fact]
        public void ShouldConvertObject()
        {
            var foo = new Foo() { Id = 42, Name = "SomeValue" };
            var result = objectConverter.Convert(foo);

            result["Id"].ShouldBe(42);
            result["Name"].ShouldBe("SomeValue");
        }


        public class Foo
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}