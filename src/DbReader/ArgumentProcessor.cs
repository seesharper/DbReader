using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader
{
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;

    internal static class ArgumentProcessor
    {
        private static readonly ConcurrentDictionary<Type, Action<IDataParameter, object>> ProcessDelegates =
            new ConcurrentDictionary<Type, Action<IDataParameter, object>>();

        internal static readonly MethodInfo ConvertMethod;
        

        static ArgumentProcessor()
        {
            ConvertMethod = typeof (ArgumentProcessor).GetTypeInfo().DeclaredMethods
                .Single(m => m.Name == "Process");
          
        }

        public static void RegisterProcessDelegate<TArgument>(Action<IDataParameter, TArgument> convertFunction)
        {
            Action<IDataParameter, object> processDelegate =
                (parameter, argument) => convertFunction(parameter, (TArgument) argument);


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
            ProcessDelegates[argumentType](dataParameter, argument);
        }

        public static Action<IDataParameter, object> GetProcessDelegate(Type propertyType)
        {
            Action<IDataParameter, object> processDelegate;
            ProcessDelegates.TryGetValue(propertyType, out processDelegate);
            return processDelegate;            
        }
    }
}
