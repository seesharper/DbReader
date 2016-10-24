namespace DbReader
{
    using System;

    public class SqlStatement
    {
        [ThreadStatic]
        private static string current;

        public static string Current
        {
            get { return current; }
            set { current = value; }
        }
    }
}