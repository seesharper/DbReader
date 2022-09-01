using System;
using System.Linq;

namespace DbReader
{
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;
    using DbReader.Extensions;

    internal static class ArgumentProcessor
    {
        private static readonly ConcurrentDictionary<Type, Action<IDataParameter, object>> ProcessDelegates =
            new ConcurrentDictionary<Type, Action<IDataParameter, object>>();

        internal static readonly MethodInfo ConvertMethod;


        static ArgumentProcessor()
        {
            ConvertMethod = typeof(ArgumentProcessor).GetTypeInfo().DeclaredMethods
                .Single(m => m.Name == "Process");

        }

        public static void RegisterProcessDelegate<TArgument>(Action<IDataParameter, TArgument> convertFunction)
        {
            Action<IDataParameter, object> processDelegate =
                (parameter, argument) => convertFunction(parameter, (TArgument)argument);


            Type argumentType = typeof(TArgument);
            if (ProcessDelegates.ContainsKey(argumentType))
            {
                ProcessDelegates.TryUpdate(argumentType, processDelegate, processDelegate);
            }
            else
            {
                ProcessDelegates.TryAdd(argumentType, processDelegate);
            }
        }

        public static void Process(Type argumentType, IDataParameter dataParameter, object argument)
        {
            ProcessDelegates[argumentType.GetUnderlyingType()](dataParameter, argument);
        }

        public static bool CanProcess(Type type)
        {
            return ProcessDelegates.ContainsKey(type.GetUnderlyingType());
        }
    }
}
