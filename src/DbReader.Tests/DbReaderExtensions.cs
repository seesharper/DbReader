namespace DbReader.Tests
{
    using System.Data;
    using System.Text;

    public static class DbReaderExtensions
    {
        public static string ToMarkdown(this IDataReader dataReader)
        {
            StringBuilder sb = new StringBuilder();            
            var fieldCount = dataReader.FieldCount;
            string[] columnNames = new string[fieldCount];
             
            for (int i = 0; i < fieldCount; i++)
            {
                columnNames[i] = dataReader.GetName(i);
            }

            for (int i = 0; i < fieldCount; i++)
            {
                sb.Append($"| {columnNames[i]} ");
            }
            sb.Append("|");
            sb.AppendLine();

            for (int i = 0; i < fieldCount; i++)
            {
                sb.Append($"| {new string('-', columnNames[i].Length)} ");
            }

            sb.Append("|");
            sb.AppendLine();


            while (dataReader.Read())
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    object value;
                    if (dataReader.IsDBNull(i))
                    {
                        value = "NULL";
                    }
                    else
                    {
                        value = dataReader.GetValue(i);
                    }
                    sb.Append($"| {value} ");
                }
                sb.Append("|");
                sb.AppendLine();
            }


            return sb.ToString();
        }
    }
}