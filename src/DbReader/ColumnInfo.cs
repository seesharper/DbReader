namespace DbReader
{
    using System;

    public class ColumnInfo
    {
        public ColumnInfo(int ordinal,string name,  Type type)
        {
            Ordinal = ordinal;
            Name = name;
            Type = type;
        }

        public int Ordinal { get; private set; }

        public Type Type { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            if (Ordinal == -1)
            {
                return "Not Mapped";
            }
            else
            {
                return "Name: {0}, Type: {1}, Ordinal: {2}".FormatWith(Name, Type, Ordinal);
            }
        }
    }
}