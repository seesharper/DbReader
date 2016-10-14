namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;
    using Database;
    using DbReader.Interfaces;
    using Readers;
    using Shouldly;


    public class InstanceReaderTests
    {
        static InstanceReaderTests()
        {
            DbReaderOptions.WhenReading<CustomValueType>().Use((record, ordinal) => new CustomValueType(record.GetInt32(ordinal)));              
        }

        public void ShouldReadStringValue(IInstanceReader<ClassWithProperty<string>> reader)
        {
            var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe("SomeValue");
        }
        
        public void ShouldReadInt64Value(IInstanceReader<ClassWithProperty<long>> reader)
        {
            var dataRecord = new { Id = 42, Property = (long)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadNullableInt64Value(IInstanceReader<ClassWithProperty<long?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (long?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadInt32Value(IInstanceReader<ClassWithProperty<int>> reader)
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadNullableInt32Value(IInstanceReader<ClassWithProperty<int?>> reader)
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadInt16Value(IInstanceReader<ClassWithProperty<short>> reader)
        {
            var dataRecord = new { Id = 42, Property = (short)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((short)84);
        }

        public void ShouldReadNullableInt16Value(IInstanceReader<ClassWithProperty<short?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (short?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((short)84);
        }

        public void ShouldReadByteValue(IInstanceReader<ClassWithProperty<byte>> reader)
        {
            var dataRecord = new { Id = 42, Property = (byte)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((byte)84);
        }

        public void ShouldReadNullableByteValue(IInstanceReader<ClassWithProperty<byte?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (byte?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe((byte)84);
        }

        public void ShouldReadByteArrayValue(IInstanceReader<ClassWithProperty<byte[]>> reader)
        {
            var dataRecord = new { Id = 42, Property = new byte[] { 1, 2, 3 } }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.SequenceEqual(new byte[] { 1, 2, 3 });
        }

        public void ShouldNullableReadDateTimeValue(IInstanceReader<ClassWithProperty<DateTime?>> reader)
        {
            var dataRecord = new { Id = 42, Property = new DateTime(2014, 2, 5) }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(new DateTime(2014, 2, 5));
        }

        public void ShouldReadDateTimeValue(IInstanceReader<ClassWithProperty<DateTime>> reader)
        {
            var dataRecord = new { Id = 42, Property = new DateTime(2014, 2, 5) }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(new DateTime(2014, 2, 5));
        }

        public void ShouldReadDecimalValue(IInstanceReader<ClassWithProperty<decimal>> reader)
        {
            var dataRecord = new { Id = 42, Property = (decimal)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadNullableDecimalValue(IInstanceReader<ClassWithProperty<decimal?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (decimal?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }
       
        public void ShouldReadDoubleValue(IInstanceReader<ClassWithProperty<double>> reader)
        {
            var dataRecord = new { Id = 42, Property = (double)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }
        
        public void ShouldReadNullableDoubleValue(IInstanceReader<ClassWithProperty<double?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (double?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadFloatValue(IInstanceReader<ClassWithProperty<float>> reader)
        {
            var dataRecord = new { Id = 42, Property = (float)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadNullableFloatValue(IInstanceReader<ClassWithProperty<float?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (float?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe(84);
        }

        public void ShouldReadCharValue(IInstanceReader<ClassWithProperty<char>> reader)
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe('A');
        }

        public void ShouldReadNullableCharValue(IInstanceReader<ClassWithProperty<char?>> reader)
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldBe('A');
        }

        public void ShouldReadCharArrayValue(IInstanceReader<ClassWithProperty<char[]>> reader)
        {
            var dataRecord = new { Id = 42, Property = new[] { 'A', 'B', 'C' } }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.SequenceEqual(new[] { 'A', 'B', 'C' });
        }

        public void ShouldReadCustomValueType(IInstanceReader<ClassWithProperty<CustomValueType>> reader)
        {
            var dataRecord = new { Id = 42, Property = 42 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.Value.ShouldBe(42);
        }


        public void ShouldReadInstance(IInstanceReader<ClassWithNoRelations> instanceReader)
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(42);
        }

        public void ShouldReadInstanceWithManyToOneRelation(IInstanceReader<ClassWithManyToOneRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, ManyToOneRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(42);
            instance.ManyToOneRelation.Id.ShouldBe(84);
        }

        public void ShouldReadInstanceWithOneToManyRelation(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldBe(1);
        }

        public void ShouldAddOneToManyRelationOnlyOnce(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            instanceReader.Read(dataRecord, string.Empty);
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldBe(1);
        }

        public void ShouldReadInstanceWithTwoManyToOneRelationsOfSameType(
            IInstanceReader<ClassWithTwoProperties<ClassWithId, ClassWithId>> instanceReader)
        {
            var dataRecord = new { Id = 1, FirstProperty_Id = 2, SecondProperty_Id = 3 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldBe(1);
            instance.FirstProperty.Id.ShouldBe(2);
            instance.SecondProperty.Id.ShouldBe(3);
        }

        public void ShouldReadInstanceWithNestedOneToManyRelation(IInstanceReader<ClassWithNestedOnToManyRelation> instanceReader)
        {
            var rows = new[]
 {
                new { MasterId = 1 ,Details_DetailId = 1 ,SubDetails_SubdetailId = 1 },
                new { MasterId = 1, Details_DetailId = 1 ,SubDetails_SubdetailId = 2 },
                new { MasterId = 1 ,Details_DetailId = 2 ,SubDetails_SubdetailId = 3 },
                new { MasterId = 1 ,Details_DetailId = 2 ,SubDetails_SubdetailId = 4 },
                new { MasterId = 1 ,Details_DetailId = 2 ,SubDetails_SubdetailId = 5 }
            };

            var instances = rows.ToDataReader().Read<Master>().ToArray();
            instances.Length.ShouldBe(1);
            instances.Single().Details.Count().ShouldBe(2);           
        }

        public void ShouldThrowExceptionWhenTypesAreIncompatible(IInstanceReader<ClassWithProperty<int>> reader)
        {            
            var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
            Should.Throw<InvalidOperationException>(() => reader.Read(dataRecord, string.Empty));            
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

    public class ClassWithNestedOnToManyRelation
    {
        public int Id { get; set; }

        public ICollection<ClassWithOneToManyRelation> TopLevelOneToManyRelation { get; set; }
    }

    public class Master
    {
        public Master()
        {
            Details = new List<Detail>();
        }
        
        public int MasterId { get; set; }
        public string MasterName { get; set; }
        public List<Detail> Details { get; set; }
    }
    public class Detail
    {
        public Detail()
        {
            SubDetails = new List<SubDetail>();
        }
        
        public int DetailId { get; set; }
        public string DetailName { get; set; }
        public List<SubDetail> SubDetails { get; set; }

    }
    public class SubDetail
    {        
        public int SubDetailId { get; set; }
        public string SubDetailName { get; set; }
    }
}
