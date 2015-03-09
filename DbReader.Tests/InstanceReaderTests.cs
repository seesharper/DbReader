namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;    
    using DbReader.Interfaces;
    using Should;
    using Should.Core.Assertions;
    
    public class InstanceReaderTests
    {        
        public void ShouldReadStringValue(IInstanceReader<ClassWithProperty<string>> reader)
        {
            var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual("SomeValue");
        }
        
        public void ShouldReadInt64Value(IInstanceReader<ClassWithProperty<long>> reader)
        {
            var dataRecord = new { Id = 42, Property = (long)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadNullableInt64Value(IInstanceReader<ClassWithProperty<long?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (long?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadInt32Value(IInstanceReader<ClassWithProperty<int>> reader)
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadNullableInt32Value(IInstanceReader<ClassWithProperty<int?>> reader)
        {
            var dataRecord = new { Id = 42, Property = 84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadInt16Value(IInstanceReader<ClassWithProperty<short>> reader)
        {
            var dataRecord = new { Id = 42, Property = (short)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual((short)84);
        }

        public void ShouldReadNullableInt16Value(IInstanceReader<ClassWithProperty<short?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (short?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual((short)84);
        }

        public void ShouldReadByteValue(IInstanceReader<ClassWithProperty<byte>> reader)
        {
            var dataRecord = new { Id = 42, Property = (byte)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual((byte)84);
        }

        public void ShouldReadNullableByteValue(IInstanceReader<ClassWithProperty<byte?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (byte?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual((byte)84);
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
            instance.Property.ShouldEqual(new DateTime(2014, 2, 5));
        }

        public void ShouldReadDateTimeValue(IInstanceReader<ClassWithProperty<DateTime>> reader)
        {
            var dataRecord = new { Id = 42, Property = new DateTime(2014, 2, 5) }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(new DateTime(2014, 2, 5));
        }

        public void ShouldReadDecimalValue(IInstanceReader<ClassWithProperty<decimal>> reader)
        {
            var dataRecord = new { Id = 42, Property = (decimal)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadNullableDecimalValue(IInstanceReader<ClassWithProperty<decimal?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (decimal?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }
       
        public void ShouldReadDoubleValue(IInstanceReader<ClassWithProperty<double>> reader)
        {
            var dataRecord = new { Id = 42, Property = (double)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }
        
        public void ShouldReadNullableDoubleValue(IInstanceReader<ClassWithProperty<double?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (double?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadFloatValue(IInstanceReader<ClassWithProperty<float>> reader)
        {
            var dataRecord = new { Id = 42, Property = (float)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadNullableFloatValue(IInstanceReader<ClassWithProperty<float?>> reader)
        {
            var dataRecord = new { Id = 42, Property = (float?)84 }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual(84);
        }

        public void ShouldReadCharValue(IInstanceReader<ClassWithProperty<char>> reader)
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual('A');
        }

        public void ShouldReadNullableCharValue(IInstanceReader<ClassWithProperty<char?>> reader)
        {
            var dataRecord = new { Id = 42, Property = 'A' }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.ShouldEqual('A');
        }

        public void ShouldReadCharArrayValue(IInstanceReader<ClassWithProperty<char[]>> reader)
        {
            var dataRecord = new { Id = 42, Property = new[] { 'A', 'B', 'C' } }.ToDataRecord();
            var instance = reader.Read(dataRecord, string.Empty);
            instance.Property.SequenceEqual(new[] { 'A', 'B', 'C' });
        }

        public void ShouldReadInstance(IInstanceReader<ClassWithNoRelations> instanceReader)
        {
            var dataRecord = new { Id = 42 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldEqual(42);
        }

        public void ShouldReadInstanceWithManyToOneRelation(IInstanceReader<ClassWithManyToOneRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, ManyToOneRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.Id.ShouldEqual(42);
            instance.ManyToOneRelation.Id.ShouldEqual(84);
        }

        public void ShouldReadInstanceWithOneToManyRelation(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldEqual(1);
        }

        public void ShouldAddOneToManyRelationOnlyOnce(IInstanceReader<ClassWithOneToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, OneToManyRelation_Id = 84 }.ToDataRecord();
            instanceReader.Read(dataRecord, string.Empty);
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.OneToManyRelation.Count().ShouldEqual(1);
        }

        public void ShouldReadInstanceWithNestedOneToManyRelation(IInstanceReader<ClassWithNestedOnToManyRelation> instanceReader)
        {
            var dataRecord = new { Id = 42, TopLevelOneToManyRelation_Id = 84, TopLevelOneToManyRelation_OneToManyRelation_Id = 168 }.ToDataRecord();
            var instance = instanceReader.Read(dataRecord, string.Empty);
            instance.TopLevelOneToManyRelation.Count().ShouldEqual(1);
            instance.TopLevelOneToManyRelation.First().Id.ShouldEqual(84);
            instance.TopLevelOneToManyRelation.First().OneToManyRelation.Count().ShouldEqual(1);
            instance.TopLevelOneToManyRelation.First().OneToManyRelation.First().Id.ShouldEqual(168);
        }

        //public void ShouldThrowExceptionWhenTypesAreIncompatible(IInstanceReader<ClassWithProperty<int>> reader)
        //{
        //    var dataRecord = new { Id = 42, Property = "SomeValue" }.ToDataRecord();
        //    Assert.Throws<InvalidOperationException>(() => reader.Read(dataRecord, string.Empty));
        //}
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
