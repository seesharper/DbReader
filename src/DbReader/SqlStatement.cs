namespace DbReader
{
    using System.Threading;

    /// <summary>
    /// Represents the query currently executing.
    /// </summary>
    public class SqlStatement
    {
        private static readonly AsyncLocal<string> _current = new AsyncLocal<string>();

        /// <summary>
        /// The query currently executing.
        /// </summary>
        public static string Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}