using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DbReader.Extensions;

namespace DbReader.Database
{
    public interface IParameterValidator
    {
        void ValidateParameters(string[] parameterNames, string[] properties, IDataParameter[] existingParameters);
    }

    public class ParameterValidator : IParameterValidator
    {
        public void ValidateParameters(string[] parameterNames, string[] properties, IDataParameter[] existingParameters)
        {
            var propertyNames = new HashSet<string>(properties, StringComparer.OrdinalIgnoreCase);
            var existingParameterNames = new HashSet<string>(existingParameters.Select(p => p.ParameterName), StringComparer.OrdinalIgnoreCase);

            var firstDuplicateParameterName = propertyNames.Intersect(existingParameterNames, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
            if (firstDuplicateParameterName != null)
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateParameter.FormatWith(firstDuplicateParameterName));
            }

            foreach (var parameterName in parameterNames)
            {
                if (!propertyNames.Contains(parameterName) && !existingParameterNames.Contains(parameterName))
                {
                    throw new InvalidOperationException(ErrorMessages.MissingArgument.FormatWith(parameterName));
                }
            }
        }
    }
}