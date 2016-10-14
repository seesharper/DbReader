using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader
{
    using System.Collections.Concurrent;

    internal static class ArgumentConverter
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> ConvertDelegates =
            new ConcurrentDictionary<Type, Func<object, object>>();
       
        public static void RegisterConvertDelegate<TArgument, TParameter>(Func<TArgument, TParameter> convertFunction)
        {
            Func<object, object> convertDelegate = argument => convertFunction((TArgument) argument);

                    
        }

        public static bool CanConvert(Type argumentType)
        {
            return ConvertDelegates.ContainsKey(argumentType);
        }

        public static object Convert(Type argumentType, object argument)
        {
            return ((Func<object, object>)ConvertDelegates[argumentType])(argumentType);
        }

        
    }
}
