namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DbReader.Construction;

    using Readers;
    using Shouldly;
    using Xunit;

    public class InstanceReaderTests : ContainerFixture
    {
        static InstanceReaderTests()
        {
            DbReaderOptions.WhenReading<CustomValueType>().Use((record, ordinal) => new CustomValueType(record.GetInt32(ordinal)));
        }

        private IReaderMethodBuilder<ClassWithProperty<string>> propertyReaderMethodBuilder;
        private IInstanceReader<T> GetReader<T>()
        {
            return GetInstance<IInstanceReader<T>>();
        }

        [Fact]
        public void ShouldReadStringValue()
        {
            var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<string>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe("SomeValue");
        }

        [Fact]
        public void ShouldReadInt64Value()
        {
            var dataRecord = new { Id = 42, Property = (long)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<long>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadNullableInt64Value()
        {
            var dataRecord = new { Id = 42, Property = (long?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<long?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadInt32Value()
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<int>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadNullableInt32Value()
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<int?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadInt16Value()
        {
            var dataRecord = new { Id = 42, Property = (short)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<short>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((short)84);
        }

        [Fact]
        public void ShouldReadNullableInt16Value()
        {
            var dataRecord = new { Id = 42, Property = (short?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<short?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((short)84);
        }

        [Fact]
        public void ShouldReadByteValue()
        {
            var dataRecord = new { Id = 42, Property = (byte)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<byte>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((byte)84);
        }

        [Fact]
        public void ShouldReadNullableByteValue()
        {
            var dataRecord = new { Id = 42, Property = (byte?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<byte?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((byte)84);
        }

        [Fact]
        public void ShouldReadByteArrayValue()
        {
            var dataRecord = new { Id = 42, Property = new byte[] { 1, 2, 3 } }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<byte[]>>().Read(dataRecord, string.Empty);
            instance.Property.SequenceEqual(new byte[] { 1, 2, 3 });
        }

        [Fact]
        public void ShouldNullableReadDateTimeValue()
        {
            var dataRecord = new { Id = 42, Property = new DateTime(2014, 2, 5) }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<DateTime?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(new DateTime(2014, 2, 5));
        }

        [Fact]
        public void ShouldReadDateTimeValue()
        {
            var dataRecord = new { Id = 42, Property = new DateTime(2014, 2, 5) }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<DateTime>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(new DateTime(2014, 2, 5));
        }

        [Fact]
        public void ShouldReadDecimalValue()
        {
            var dataRecord = new { Id = 42, Property = (decimal)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<decimal>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadNullableDecimalValue()
        {
            var dataRecord = new { Id = 42, Property = (decimal?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<decimal?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadDoubleValue()
        {
            var dataRecord = new { Id = 42, Property = (double)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<double>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadNullableDoubleValue()
        {
            var dataRecord = new { Id = 42, Property = (double?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<double?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadFloatValue()
        {
            var dataRecord = new { Id = 42, Property = (float)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<float>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadNullableFloatValue()
        {
            var dataRecord = new { Id = 42, Property = (float?)84 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<float?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadGuidValue()
        {
            Guid guid = Guid.NewGuid();
            var dataRecord = new { Id = 42, Property = guid }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<Guid>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(guid);
        }

        [Fact]
        public void ShouldReadNullableGuidValue()
        {
            Guid? guid = Guid.NewGuid();
            var dataRecord = new { Id = 42, Property = guid }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<Guid?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(guid);
        }

        [Fact]
        public void ShouldReadCharValue()
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<char>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe('A');
        }

        [Fact]
        public void ShouldReadNullableCharValue()
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<char?>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe('A');
        }

        [Fact]
        public void ShouldReadCharArrayValue()
        {
            var dataRecord = new { Id = 42, Property = new[] { 'A', 'B', 'C' } }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<char[]>>().Read(dataRecord, string.Empty);
            instance.Property.SequenceEqual(new[] { 'A', 'B', 'C' });
        }

        [Fact]
        public void ShouldReadEnum()
        {
            var dataRecord = new { Id = 1, Property = 1 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<SampleEnum>>().Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(SampleEnum.One);
        }


        [Fact]
        public void ShouldReadCustomValueType()
        {
            var dataRecord = new { Id = 42, Property = 42 }.ToDataRecord();
            var instance = GetReader<ClassWithProperty<CustomValueType>>().Read(dataRecord, string.Empty);
            instance.Property.Value.ShouldBe(42);
        }

        [Fact]
        public void ShouldReadInstance()
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var instance = GetReader<ClassWithNoRelations>().Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(42);
        }

        [Fact]
        public void ShouldReadInstanceWithManyToOneRelation()
        {
            var dataRecord = new { Id = 42, ManyToOneRelation_Id = 84 }.ToDataRecord();
            var instance = GetReader<ClassWithManyToOneRelation>().Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(42);
            instance.ManyToOneRelation.Id.ShouldBe(84);
        }

        [Fact]
        public void ShouldReadInstanceWithOneToManyRelation()
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            var instance = GetReader<ClassWithOneToManyRelation>().Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldBe(1);
        }



        [Fact]
        public void ShouldReadInstanceWithRecursiveOneToManyRelation()
        {
            var dataRecord = new[] { new { Id = 42, OneToManyRelation_Id = 64 }, new { Id = 42, OneToManyRelation_Id = 128 } }.ToDataReader();

            ClassWithRecursiveOneToManyRelation instance = null;
            var reader = GetReader<ClassWithRecursiveOneToManyRelation>();
            while (dataRecord.Read())
            {
                instance = reader.Read(dataRecord, string.Empty);
            }

            instance.OneToManyRelation.Count().ShouldBe(2);
        }

        [Fact]
        public void ShouldAddOneToManyRelationOnlyOnce()
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            var reader = GetReader<ClassWithOneToManyRelation>();
            reader.Read(dataRecord, string.Empty);
            var instance = reader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldReadInstanceWithTwoManyToOneRelationsOfSameType()
        {
            var dataRecord = new { Id = 1, FirstProperty_Id = 2, SecondProperty_Id = 3 }.ToDataRecord();
            var reader = GetReader<ClassWithTwoProperties<ClassWithId, ClassWithId>>();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(1);
            instance.FirstProperty.Id.ShouldBe(2);
            instance.SecondProperty.Id.ShouldBe(3);
        }

        [Fact]
        public void ShouldHandleManyToOneWithoutAnyMatchingColumns()
        {
            var dataRecord = new { Id = 1 }.ToDataRecord();
            var reader = GetReader<ClassWithProperty<ClassWithId>>();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBeNull();
        }

        [Fact]
        public void ShouldReadInstanceWithNestedOneToManyRelation()
        {
            var rows = new[]
            {
                new { MasterId = 1 ,Details_DetailId = 1 ,Details_SubDetails_SubdetailId = 1 },
                new { MasterId = 1, Details_DetailId = 1 ,Details_SubDetails_SubdetailId = 2 },
                new { MasterId = 1 ,Details_DetailId = 2 ,Details_SubDetails_SubdetailId = 3 },
                new { MasterId = 1 ,Details_DetailId = 2 ,Details_SubDetails_SubdetailId = 4 },
                new { MasterId = 1 ,Details_DetailId = 2 ,Details_SubDetails_SubdetailId = 5 }
            };

            var instances = rows.ToDataReader().Read<Master>().ToArray();
            instances.Length.ShouldBe(1);
            instances.Single().Details.Count().ShouldBe(2);
        }

        [Fact]
        public void ShouldThrowExceptionWhenTypesAreIncompatible()
        {
            var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
            var reader = GetReader<ClassWithProperty<int>>();
            Should.Throw<InvalidOperationException>(() => reader.Read(dataRecord, string.Empty));
        }

        [Fact]
        public void ShouldHandleNullValuesInNavigationChain()
        {
            var dataRecord = new { ParentID = "42", Children_ChildId = (string)null, Children_SubChildren_SubChildId = (string)null }.ToDataRecord();
            var reader = GetReader<Parent>();
            var result = reader.Read(dataRecord, string.Empty);
            result.Children.ShouldBeEmpty();
        }

        public class Parent
        {
            public string ParentId { get; set; }

            public ICollection<Child> Children { get; set; }
        }

        public class Child
        {
            public string ChildId { get; set; }

            public ICollection<SubChild> SubChildren { get; set; }
        }

        public class SubChild
        {
            public string SubChildId { get; set; }
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

        public ICollection<ClassWithNoRelations> OneToManyRelation { get; set; }
    }

    public class ClassWithRecursiveOneToManyRelation
    {
        public int Id { get; set; }

        public ICollection<ClassWithRecursiveOneToManyRelation> OneToManyRelation { get; set; }
    }


    public class ClassWithNestedOnToManyRelation
    {
        public int Id { get; set; }

        public ICollection<ClassWithOneToManyRelation> TopLevelOneToManyRelation { get; set; }
    }

    public class Master
    {
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public IEnumerable<Detail> Details { get; set; }
    }
    public class Detail
    {
        public int DetailId { get; set; }
        public string DetailName { get; set; }
        public IEnumerable<SubDetail> SubDetails { get; set; }

    }
    public class SubDetail
    {
        public int SubDetailId { get; set; }
        public string SubDetailName { get; set; }
    }
}
