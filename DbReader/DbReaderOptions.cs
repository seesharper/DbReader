namespace DbReader
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class DbReaderOptions
    {
        static DbReaderOptions()
        {
            KeySelector = p =>
                p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.InvariantCultureIgnoreCase);
        }

        public static Func<PropertyInfo, bool> KeySelector { get; set; }
    }
}