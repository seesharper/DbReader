namespace DbClient
{
    using System;

    /// <summary>
    /// Represents the query currently executing.
    /// </summary>
    public class SqlStatement
    {
        /// <summary>
        /// The query currently executing. 
        /// </summary>
        [ThreadStatic]
        public static string Current;      
    }
}