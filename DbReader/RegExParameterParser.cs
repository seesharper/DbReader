namespace DbReader
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// An <see cref="IParameterParser"/> that uses regular 
    /// expressions to parse out references to parameters from 
    /// a given sql statement.
    /// </summary>
    public class RegExParameterParser : IParameterParser
    {
        private readonly string pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegExParameterParser"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to be used to identify the 
        /// parameter references in a sql statement.</param>
        public RegExParameterParser(string pattern)
        {
            this.pattern = pattern;
        }

        /// <summary>
        /// Gets a list that represents the names of the parameters 
        /// used in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The sql statement to be parsed.</param>
        /// <returns>A list of parameters used in the given <paramref name="sql"/>.</returns>
        public string[] GetParameters(string sql)
        {
            var result = new Collection<string>();
            var matches = Regex.Matches(sql, pattern).Cast<Match>();
            foreach (var match in matches)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    if (!string.IsNullOrEmpty(match.Groups[i].Value))
                    {
                        result.Add(match.Groups[i].Value);
                        break;                        
                    }
                }
            }

            return result.ToArray();
        }
    }
}