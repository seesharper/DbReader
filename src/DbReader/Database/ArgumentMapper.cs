﻿namespace DbReader.Database
{
    using System;
    using System.Collections.Generic;
    using Construction;
    using Extensions;
    using Interfaces;

    public class ArgumentMapper : IArgumentMapper
    {
        private readonly IParameterParser parameterParser;
        private readonly IArgumentParser argumentParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentMapper"/> class.
        /// </summary>
        /// <param name="parameterParser">The <see cref="IParameterParser"/> that is responsible for parsing 
        /// the SQL statement and return a list of parameters.</param>
        /// <param name="argumentParser">The <see cref="IArgumentParser"/> that is responsible for parsing an arguments object and 
        /// return a list of named arguments.</param>
        public ArgumentMapper(IParameterParser parameterParser, IArgumentParser argumentParser)
        {
            this.parameterParser = parameterParser;
            this.argumentParser = argumentParser;
        }

        /// <summary>
        /// Maps the argument values represented by the <paramref name="arguments"/>
        /// to the parameters defined in the given <paramref name="sql"/>.
        /// </summary>
        /// <param name="sql">The query containing the parameters.</param>
        /// <param name="arguments">An object where each readable property represents a named argument value.</param>
        /// <returns>A </returns>
        public IReadOnlyDictionary<string, ArgumentValue> Map(string sql, object arguments)
        {
            var result = new Dictionary<string, ArgumentValue>(StringComparer.OrdinalIgnoreCase);
            if (arguments == null)
            {
                return result;
            }
            var parameterNames = parameterParser.GetParameters(sql);
            var namedArguments = argumentParser.Parse(arguments);
            if (parameterNames.Length > 0)
            {
                foreach (var parameterName in parameterNames)
                {
                    ArgumentValue argument;
                    if (!namedArguments.TryGetValue(parameterName, out argument))
                    {
                        throw new InvalidOperationException(ErrorMessages.MissingArgument.FormatWith(parameterName));
                    }
                    result.Add(parameterName, argument);
                }
            }
            else
            {
                return namedArguments;
            }
            return result;
        }
    }
}