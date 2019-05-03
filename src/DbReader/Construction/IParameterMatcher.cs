using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DbReader.Database;
using DbReader.Extensions;
using DbReader.Selectors;

namespace DbReader.Construction
{
    public interface IParameterMatcher
    {
        MatchedParameter[] Match(string sql, Type argumentsType, IDataParameter[] existingParameters);
    }

    public class ParameterMatcher : IParameterMatcher
    {
        private readonly IParameterParser parameterParser;
        private readonly IPropertySelector readablePropertySelector;

        public ParameterMatcher(IParameterParser parameterParser, IPropertySelector readablePropertySelector)
        {
            this.parameterParser = parameterParser;
            this.readablePropertySelector = readablePropertySelector;
        }

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


    public class MatchedParameter
    {
        public MatchedParameter(DataParameterInfo dataParameterInfo, PropertyInfo propertyInfo)
        {
            DataParameter = dataParameterInfo;
            Property = propertyInfo;
        }

        public DataParameterInfo DataParameter { get; }
        public PropertyInfo Property { get; }
    }
}
