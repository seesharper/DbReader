namespace DbReader.Tests
{
    using System.Data.SQLite;
    using System.IO;

    using Xunit;

    public class IntegrationTests
    {
        public IntegrationTests()
        {
            string dbFile = @"..\..\db\northwind.db";

            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
                using (var connection = new SQLiteConnection("Data Source = " + dbFile))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = ReadScript();
                    command.ExecuteNonQuery();    
                }                
            }
        }

        private string ReadScript()
        {
            using (StreamReader reader = new StreamReader(@"..\..\db\northwind.sql"))
            {
                return reader.ReadToEnd();
            }
        }

        [Fact]
        public void SomeTest()
        {
            
        }
    }
}