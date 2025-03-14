namespace DbClient.Tests
{
    using System;
    using System.Runtime.CompilerServices;

    public class FakeSql
    {
        public static string Create(string prefix = null, [CallerMemberName] string caller = null)
        {
            // Adding "SQL" forces the string returned not to referenceequals.
            return prefix + caller + "Sql";
        }


    }
}