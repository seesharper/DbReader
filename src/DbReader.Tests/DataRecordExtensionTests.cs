namespace DbReader.Tests
{
    using System;
    using System.Linq;
    using Shouldly;

    public class DataRecordExtensionTests
    {
        public void ShouldGetBytesWithSpecifiedLength()
        {
            var dataRecord = new { Bytes = new byte[] { 42, 84 } }.ToDataRecord();

            var bytes = dataRecord.GetBytes(0, 2);

            bytes.SequenceEqual(new byte[] { 42, 84 }).ShouldBeTrue();
        }

        public void ShouldGetBytesWithoutSpecifiedLength()
        {
            byte[] actualBytes = new byte[512];
            Random random = new Random();
            random.NextBytes(actualBytes);            
            var dataRecord = new {Bytes = actualBytes}.ToDataRecord();

            var bytes = dataRecord.GetBytes(0);

            bytes.SequenceEqual(actualBytes).ShouldBeTrue();
        }

        public void ShouldGetChars()
        {
            var dataRecord = new { Bytes = new[] { 'a', 'b' } }.ToDataRecord();

            var chars = dataRecord.GetChars(0);

            chars.SequenceEqual(new[] { 'a', 'b' }).ShouldBeTrue();
        }
    }
}