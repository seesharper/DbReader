namespace DbReader
{
    using System;

    internal static class Require
    {
        public static void IsNotNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);    
            }            
        }

        public static void IsNotNull<T>(T value, string parameterName, string message)
        {

        }
    }


    internal static class Ensure
    {
        public static T IsNotNull<T>(T value, string message) where T : class
        {
            if (value == null)
            {
                throw new InvalidOperationException(message);
            }

            return value;
        }
    }

}