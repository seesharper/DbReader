using DbReader.DynamicArguments;
using DbReader.Tracking;
using DbReader.Tracking.SampleAssembly;
using ILVerifier;
using Shouldly;
using Xunit;

namespace DbReader.Tests;

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
    public void ShouldNotBeSameTypeWhenNumberOfMembersAreDifferent()
    {
        var dynamicObject1 = new ArgumentsBuilder().Add("Id", 42).Build();
        var dynamicObject2 = new ArgumentsBuilder().Add("Id", 42).Add("Id2", 42).Build();
        dynamicObject1.GetType().ShouldNotBeSameAs(dynamicObject2.GetType());
    }

    [Fact]
    public void ShouldNotBeSameTypeWhenMemberNamesAreDifferent()
    {
        var dynamicObject1 = new ArgumentsBuilder().Add("Id1", 42).Build();
        var dynamicObject2 = new ArgumentsBuilder().Add("Id2", 42).Build();
        dynamicObject1.GetType().ShouldNotBeSameAs(dynamicObject2.GetType());
        object.Equals(dynamicObject1, dynamicObject2).ShouldBeFalse();
    }

    [Fact]
    public void ShouldCopyValuesFromObjectWithPropertiesAndFields()
    {
        var dynamicObject = new ArgumentsBuilder().From(new ObjectWithPropertiesAndFields() { Id = 42, Name = "SomeName" }).Build();
        dynamicObject.Get<int>("Id").ShouldBe(42);
        dynamicObject.Get<string>("Name").ShouldBe("SomeName");
    }

    [Fact]
    public void Should()
    {
        var positionalRecord = new PositionalRecord(42, "SomeName");
        // set value to 42 via reflection
        //positionalRecord.GetType().GetProperty("Id")!.SetValue(positionalRecord, 84);
        //((ITrackedObject)positionalRecord).GetModifiedProperties().ShouldContain("Id");
        new Verifier()
               .WithAssemblyReferenceFromType<VerifiableMethodSkeleton>()
               .WithAssemblyReference(typeof(DbConnectionExtensions).Assembly)
               .Verify(typeof(PositionalRecord).Assembly);

    }


    [Fact]
    public void ShouldUseEqualsForDynamicMember()
    {
        var firstMember = new DynamicMemberInfo("Id", typeof(int));
        var secondMember = new DynamicMemberInfo("Id", typeof(int));
        object.Equals(firstMember, secondMember).ShouldBeTrue();
    }

    public class DynamicMemberInfoArrayEqualityComparerTests
    {
        [Fact]
        public void ShouldNotBeEqualForDifferentTypes()
        {
            var comparer = new DynamicMemberInfoArrayEqualityComparer();
            var firstList = new DynamicMemberInfo[] { new DynamicMemberInfo("Id1", typeof(int)), new DynamicMemberInfo("Id2", typeof(int)) };
            var secondList = new DynamicMemberInfo[] { new DynamicMemberInfo("Id1", typeof(int)), new DynamicMemberInfo("Id2", typeof(string)) };
            comparer.Equals(firstList, secondList).ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotBeEqualForDifferentLengths()
        {
            var comparer = new DynamicMemberInfoArrayEqualityComparer();
            var firstList = new DynamicMemberInfo[] { new DynamicMemberInfo("Id1", typeof(int)), new DynamicMemberInfo("Id2", typeof(int)) };
            var secondList = new DynamicMemberInfo[] { new DynamicMemberInfo("Id1", typeof(int)) };
            comparer.Equals(firstList, secondList).ShouldBeFalse();
        }
    }

    public class ObjectWithPropertiesAndFields
    {
        public int Id { get; set; }

        public string Name;
    }
}