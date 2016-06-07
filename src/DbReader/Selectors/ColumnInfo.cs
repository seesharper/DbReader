namespace DbReader.Selectors
{
    using System;
    using Extensions;

    /// <summary>
    /// Contains information about a field in a data record.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="ordinal">The ordinal of the column.</param>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The type of the column as represented in the target data record.</param>
        public ColumnInfo(int ordinal,string name,  Type type)
        {
            Ordinal = ordinal;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets the ordinal of the column.
        /// </summary>
        public int Ordinal { get; }

        /// <summary>
        /// Gets the type of the column as represented in the target data record.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>       
        public override string ToString()
        {
            if (Ordinal == -1)
            {
                return "Not Mapped";
            }
            return "Name: {0}, Type: {1}, Ordinal: {2}".FormatWith(Name, Type, Ordinal);
        }
    }
}