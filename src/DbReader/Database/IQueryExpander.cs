// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using DbReader.Construction;

// namespace DbReader.Database
// {
//     public interface IQueryExpander
//     {
//         QueryInfo Expand(string query, object arguments, Func<IDataParameter> parameterFactory, IDataParameter[] existingParameters);
//     }


//     public class QueryExpander : IQueryExpander
//     {
//         private readonly IParameterParser parameterParser;
//         private readonly IParameterParser listParameterParser;
//         private readonly IObjectConverter objectConverter;
//         private readonly IParameterValidator parameterValidator;

//         public QueryExpander(IParameterParser parameterParser, IParameterParser listParameterParser, IObjectConverter objectConverter, IParameterValidator parameterValidator)
//         {
//             this.parameterParser = parameterParser;
//             this.listParameterParser = listParameterParser;
//             this.objectConverter = objectConverter;
//             this.parameterValidator = parameterValidator;
//         }

//         public QueryInfo Expand(string query, object arguments, Func<IDataParameter> parameterFactory, IDataParameter[] existingParameters)
//         {
//             var dataParameters = new List<IDataParameter>();
//             var parameters = parameterParser.GetParameters(query);
//             var propertyValues = objectConverter.Convert(arguments);
//             parameterValidator.ValidateParameters(parameters, propertyValues.Keys.ToArray(), existingParameters);

//             foreach (var parameter in parameters)
//             {
//                 if (parameter.IsListParameter)
//                 {
//                     var listValues = ((IEnumerable<object>)propertyValues[parameter.Name]).ToArray();
//                     var expandedParameterList = new List<string>();
//                     for (int i = 0; i < listValues.Length; i++)
//                     {
//                         if (listValues[i] is IDataParameter dataParameter)
//                         {
//                             dataParameters.Add(dataParameter);
//                         }
//                         else
//                         {
//                             dataParameter = parameterFactory();
//                             dataParameter.Value = DbNullConverter.ToDbNullIfNull(listValues[i]);
//                             dataParameter.ParameterName = $"{parameter.FullName}{i}";
//                             dataParameters.Add(dataParameter);
//                             expandedParameterList.Add(dataParameter.ParameterName);
//                         }
//                     }
//                     var expandedParameterFragment = expandedParameterList.Aggregate((current, next) => $"{current}, {next}");
//                     query = query.Replace(parameter.FullName, expandedParameterFragment);
//                 }
//                 else
//                 {
//                     var parameterValue = propertyValues[parameter.Name];
//                     if (parameterValue is IDataParameter dataParameter)
//                     {
//                         dataParameters.Add(dataParameter);
//                     }
//                     else
//                     {
//                         dataParameter = parameterFactory();
//                         dataParameter.Value = DbNullConverter.ToDbNullIfNull(parameterValue);
//                         dataParameter.ParameterName = parameter.FullName;
//                         dataParameters.Add(dataParameter);
//                     }
//                 }
//             }

//             return new QueryInfo(query, dataParameters.ToArray());

//         }
//     }
// }