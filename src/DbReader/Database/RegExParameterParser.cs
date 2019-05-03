namespace DbReader.Database
{
    using System;
    using System.Collections.Generic;
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
        // TODO Cache this.
        private readonly Regex parameterMatcher;

        private readonly Regex listParameterMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegExParameterParser"/> class.
        /// </summary>
        /// <param name="parameterPattern">The regular expression pattern to be used to identify the
        /// parameter references in a sql statement.</param>
        /// <param name="listParameterPattern">The regular expression to be used to identify the list
        /// parameter references in a sql statement.</param>
        public RegExParameterParser(string parameterPattern, string listParameterPattern)
        {
            this.parameterMatcher = new Regex(parameterPattern, RegexOptions.Compiled);
            this.listParameterMatcher = new Regex(listParameterPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Gets a list that represents the names of the parameters
        /// used in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The sql statement to be parsed.</param>
        /// <returns>A list of parameters used in the given <paramref name="sql"/>.</returns>
        public DataParameterInfo[] GetParameters(string sql)
        {
            var result = new List<DataParameterInfo>();
            var allParameters = Parse(sql, parameterMatcher);
            var listParameters = Parse(sql, listParameterMatcher);
            foreach (var parameter in allParameters)
            {
                result.Add(new DataParameterInfo(parameter.Substring(1), parameter, listParameters.Contains(parameter)));
            }
            return result.ToArray();
        }

        private string[] Parse(string sql, Regex regex)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var matches = regex.Matches(sql).Cast<Match>();
            foreach (var match in matches)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    if (!string.IsNullOrEmpty(match.Groups[i].Value))
                    {
                        result.Add(match.Groups[i].Value.Trim());
                        break;
                    }
                }
            }

            return result.ToArray();
        }
    }
}