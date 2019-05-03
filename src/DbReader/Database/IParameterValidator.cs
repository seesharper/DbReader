using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DbReader.Extensions;

namespace DbReader.Database
{
    public interface IParameterValidator
    {
        void ValidateParameters(DataParameterInfo[] parameterNames, PropertyInfo[] properties, IDataParameter[] existingParameters);
    }

    public class ParameterValidator : IParameterValidator
    {
        public void ValidateParameters(DataParameterInfo[] parameterNames, PropertyInfo[] properties, IDataParameter[] existingParameters)
        {
            var propertyNames = new HashSet<string>(properties.Select(p => p.Name), StringComparer.OrdinalIgnoreCase);
            var existingParameterNames = new HashSet<string>(existingParameters.Select(p => p.ParameterName), StringComparer.OrdinalIgnoreCase);

            var firstDuplicateParameterName = propertyNames.Intersect(existingParameterNames, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
            if (firstDuplicateParameterName != null)
            {
                throw new InvalidOperationException(ErrorMessages.DuplicateParameter.FormatWith(firstDuplicateParameterName));
            }

            foreach (var parameterName in parameterNames.Select(p => p.Name))
            {
                if (!propertyNames.Contains(parameterName) && !existingParameterNames.Contains(parameterName))
                {
                    throw new InvalidOperationException(ErrorMessages.MissingArgument.FormatWith(parameterName));
                }
            }
        }
    }
}