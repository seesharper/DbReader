using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbClient.Database;
using DbClient.Extensions;
using DbClient.Selectors;

namespace DbClient.Construction
{
    /// <summary>
    /// A class that matches the properties of a given arguments type to the parameters found in a SQL statement.
    /// </summary>
    public class ParameterMatcher : IParameterMatcher
    {
        private readonly IParameterParser parameterParser;
        private readonly IPropertySelector readablePropertySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMatcher"/> class.
        /// </summary>
        /// <param name="parameterParser">The <see cref="IParameterParser"/> that is responsible for parsing parameters from a SQL statement.</param>
        /// <param name="readablePropertySelector">The <see cref="IPropertySelector"/> that is responsible for providing a list of readable properties from the argument object type.</param>
        public ParameterMatcher(IParameterParser parameterParser, IPropertySelector readablePropertySelector)
        {
            this.parameterParser = parameterParser;
            this.readablePropertySelector = readablePropertySelector;
        }

        /// <summary>
        /// Matches the properties of the <paramref name="argumentsType"/> to the parameters found in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The sql statement containing the parameters.</param>
        /// <param name="argumentsType">The argument object type.</param>
        /// <param name="existingParameters">A list of existing data parameters.</param>
        /// <returns>A list of matching parameters.</returns>
        public MatchedParameter[] Match(string sql, Type argumentsType, IDataParameter[] existingParameters)
        {
            var existingParameterNames = new HashSet<string>(existingParameters.Select(p => p.ParameterName), StringComparer.OrdinalIgnoreCase);
            var dataParameters = parameterParser.GetParameters(sql);
            var propertyMap = readablePropertySelector.Execute(argumentsType).ToDictionary(p => p.Name, p => p, StringComparer.InvariantCultureIgnoreCase);

            var firstDuplicateParameterName = propertyMap.Keys.Intersect(existingParameterNames, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
            if (firstDuplicateParameterName != null)
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateParameter.FormatWith(firstDuplicateParameterName));
            }

            var result = new List<MatchedParameter>();
            foreach (var dataParameter in dataParameters)
            {
                if (existingParameterNames.Contains(dataParameter.Name))
                {
                    continue;
                }

                if (!propertyMap.TryGetValue(dataParameter.Name, out var property))
                {
                    if (existingParameterNames.Contains(dataParameter.FullName))
                    {
                        continue;
                    }
                    throw new InvalidOperationException(ErrorMessages.MissingArgument.FormatWith(dataParameter.Name));
                }

                if (dataParameter.IsListParameter && !property.PropertyType.IsEnumerableOfSimpleType())
                {
                    throw new InvalidOperationException(ErrorMessages.InvalidListArgument.FormatWith(dataParameter.FullName, property.Name));
                }

                result.Add(new MatchedParameter(dataParameter, property));
            }

            return result.ToArray();
        }
    }
}
