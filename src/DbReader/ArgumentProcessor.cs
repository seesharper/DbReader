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
            ProcessDelegates.TryAdd(typeof(int), (parameter, value) => AssignParameterValue(parameter, (int)value));
            ProcessDelegates.TryAdd(typeof(uint), (parameter, value) => AssignParameterValue(parameter, (uint)value));
            ProcessDelegates.TryAdd(typeof(long), (parameter, value) => AssignParameterValue(parameter, (long)value));
            ProcessDelegates.TryAdd(typeof(ulong), (parameter, value) => AssignParameterValue(parameter, (ulong)value));
            ProcessDelegates.TryAdd(typeof(string), (parameter, value) => AssignParameterValue(parameter, (string)value));
            ProcessDelegates.TryAdd(typeof(DateTime), (parameter, value) => AssignParameterValue(parameter, (DateTime)value));
            ProcessDelegates.TryAdd(typeof(Guid), (parameter, value) => AssignParameterValue(parameter, (Guid)value));
            ProcessDelegates.TryAdd(typeof(decimal), (parameter, value) => AssignParameterValue(parameter, (decimal)value));
            ProcessDelegates.TryAdd(typeof(byte[]), (parameter, value) => AssignParameterValue(parameter, (byte[])value));
            ProcessDelegates.TryAdd(typeof(char[]), (parameter, value) => AssignParameterValue(parameter, (char[])value));
            ProcessDelegates.TryAdd(typeof(sbyte), (parameter, value) => AssignParameterValue(parameter, (sbyte)value));
            ProcessDelegates.TryAdd(typeof(byte), (parameter, value) => AssignParameterValue(parameter, (byte)value));
            ProcessDelegates.TryAdd(typeof(short), (parameter, value) => AssignParameterValue(parameter, (short)value));
            ProcessDelegates.TryAdd(typeof(ushort), (parameter, value) => AssignParameterValue(parameter, (ushort)value));
            ProcessDelegates.TryAdd(typeof(ushort), (parameter, value) => AssignParameterValue(parameter, (ushort)value));            
        }


        //Extension method?
        private static void AssignParameterValue<T>(IDataParameter dataParameter, T value)
        {
            try
            {
                dataParameter.Value = ToDbNullIfNull(value);
            }
            catch
            {
                throw new ArgumentOutOfRangeException(dataParameter.ParameterName, value, ErrorMessages.InvalidParameterValue.FormatWith(dataParameter.ParameterName, value, value == null ? "null" : value.GetType().Name));
            }
        }

        private static object ToDbNullIfNull(object value)
        {
            return value ?? DBNull.Value;
        }

        public static void RegisterProcessDelegate<TArgument>(Action<IDataParameter, TArgument> convertFunction)
        {
            Action<IDataParameter, object> processDelegate =
                (parameter, argument) => convertFunction(parameter, (TArgument)argument);


            Type argumentType = typeof(TArgument);
            if (ProcessDelegates.ContainsKey(argumentType))
            {
                ProcessDelegates[argumentType] = processDelegate;
            }
            else
            {
                ProcessDelegates.TryAdd(argumentType, processDelegate);
            }
        }

        public static void Process(Type argumentType, IDataParameter dataParameter, object argument)
        {
            if (argument == null)
            {
                dataParameter.Value = DBNull.Value;
                return;
            }

            if (ProcessDelegates.ContainsKey(argumentType))
            {
                ProcessDelegates[argumentType](dataParameter, argument);
            }
            else
            {
                Type underlyingType = argumentType.GetUnderlyingType();
                while (underlyingType != argumentType)
                {
                    if (ProcessDelegates.ContainsKey(underlyingType))
                    {
                        ProcessDelegates[underlyingType](dataParameter, argument);
                        break;
                    }
                    else
                    {
                        underlyingType = underlyingType.GetUnderlyingType();
                    }
                }
            }
        }

        public static bool CanProcess(Type type)
        {
            //Keep a map to that we know that Nullable<Guid> maps to ConvertFunction for Guid

            if (ProcessDelegates.ContainsKey(type))
            {
                return true;
            }
            else
            {
                Type underlyingType = type.GetUnderlyingType();
                if (underlyingType != type)
                {
                    return CanProcess(underlyingType);
                }

                return false;
            }



            // Type underlyingType = type.GetUnderlyingType();
            // while (type != underlyingType)
            // {
            //     if (ProcessDelegates.ContainsKey(type))
            //     {
            //         return true;
            //     }
            //     else
            //     {
            //         return CanProcess(underlyingType);
            //     }
            // }
            // return type.IsSimpleType();
        }
    }
}
