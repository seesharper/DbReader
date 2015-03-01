namespace DbReader.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using DbReader.Interfaces;

    using Should;

    public class InstanceReaderTests
    {
        [ScopedTheory, InjectData]
        public void ShouldReadInstance(IInstanceReader<ClassWithNoRelations> instanceReader)
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldEqual(42);
        }

        [ScopedTheory, InjectData]
        public void ShouldReadInstanceWithManyToOneRelation(IInstanceReader<ClassWithManyToOneRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, ManyToOneRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldEqual(42);
            instance.ManyToOneRelation.Id.ShouldEqual(84);
        }

        [ScopedTheory, InjectData]
        public void ShouldReadInstanceWithOneToManyRelation(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldEqual(1);
        }

        [ScopedTheory, InjectData]
        public void ShouldAddOneToManyRelationOnlyOnce(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            instanceReader.Read(dataRecord, string.Empty);
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldEqual(1);
        }

        [ScopedTheory, InjectData]
        public void ShouldReadInstanceWithNestedOneToManyRelation(IInstanceReader<ClassWithNestedOnToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, TopLevelOneToManyRelation_Id = 84, TopLevelOneToManyRelation_OneToManyRelation_Id = 168 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.TopLevelOneToManyRelation.Count().ShouldEqual(1);
            instance.TopLevelOneToManyRelation.First().Id.ShouldEqual(84);
            instance.TopLevelOneToManyRelation.First().OneToManyRelation.Count().ShouldEqual(1);
            instance.TopLevelOneToManyRelation.First().OneToManyRelation.First().Id.ShouldEqual(168);
        }
    }


    public class ClassWithNoRelations
    {
        public int Id { get; set; }
    }

    public class ClassWithManyToOneRelation
    {
        public int Id { get; set; }

        public ClassWithNoRelations ManyToOneRelation { get; set; }
    }

    public class ClassWithOneToManyRelation
    {
        public int Id { get; set; }

        public IEnumerable<ClassWithNoRelations> OneToManyRelation { get; set; }
    }

    public class ClassWithNestedOnToManyRelation
    {
        public int Id { get; set; }

        public IEnumerable<ClassWithOneToManyRelation> TopLevelOneToManyRelation { get; set; }
    }
}
