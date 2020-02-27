using Xunit;
using Shouldly;
namespace DbReader.Tests
{
    public class ArgumentsBuilderTests
    {
        [Fact]
        public void ShouldAddAndGetMember()
        {
            var dynamicObject = new ArgumentsBuilder().Add("Id", 42).Build();
            dynamicObject.Get<int>("Id");
            dynamicObject.Get<int>("Id").ShouldBe(42);
        }

        [Fact]
        public void ShouldBeSameTypeWhenMembersAreEqual()
        {
            var dynamicObject1 = new ArgumentsBuilder().Add("Id", 42).Build();
            var dynamicObject2 = new ArgumentsBuilder().Add("Id", 42).Build();
            dynamicObject1.GetType().ShouldBeSameAs(dynamicObject2.GetType());
        }

        [Fact]
        public void ShouldNotBeSameTypeWhenTypesAreDifferent()
        {
            var dynamicObject1 = new ArgumentsBuilder().Add("Id", 42).Build();
            var dynamicObject2 = new ArgumentsBuilder().Add("Id", (double)42).Build();
            dynamicObject1.GetType().ShouldNotBeSameAs(dynamicObject2.GetType());
        }

        [Fact]
        public void ShouldNotBeSameTypeWhenMemberNamesAreDifferent()
        {
            var dynamicObject1 = new ArgumentsBuilder().Add("Id1", 42).Build();
            var dynamicObject2 = new ArgumentsBuilder().Add("Id2", 42).Build();
            dynamicObject1.GetType().ShouldNotBeSameAs(dynamicObject2.GetType());
        }

        [Fact]
        public void ShouldCopyValuesFromObjectWithPropertiesAndFields()
        {
            var dynamicObject = new ArgumentsBuilder().From(new ObjectWithPropertiesAndFields() { Id = 42, Name = "SomeName" }).Build();
            dynamicObject.Get<int>("Id").ShouldBe(42);
            dynamicObject.Get<string>("Name").ShouldBe("SomeName");
        }

        // [Fact]
        // public void ShouldBeEqualWhenMemberAndValueMatches()
        // {
        //     var dynamicObject1 = new DynamicTypeBuilder().Add("Id1", 42).Build();
        //     var dynamicObject2 = new DynamicTypeBuilder().Add("Id1", 42).Build();
        //     dynamicObject1.Should().Be(dynamicObject2);
        // }

        public class ObjectWithPropertiesAndFields
        {
            public int Id { get; set; }

            public string Name;
        }
    }
}