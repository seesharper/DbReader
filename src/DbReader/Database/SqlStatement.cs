namespace DbReader.Database
{
    using System;

    /// <summary>
    /// Represents the current SQL statement.
    /// </summary>
    public static class SqlStatement
    {
        [ThreadStatic]
        private static string current;

        /// <summary>
        /// Gets or sets a string representing the current SQL statement.
        /// </summary>
        public static string Current
        {
            get { return current; }
            set { current = value; }
        }
    }
}