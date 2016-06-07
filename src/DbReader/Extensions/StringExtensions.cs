namespace DbReader.Extensions
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extends the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex LowerCaseRegex = new Regex("[a-z]");
        
        /// <summary>
        /// Applies the given <paramref name="arguments"/> and returns the formatted string.
        /// </summary>
        /// <param name="value">The target <see cref="string"/> value.</param>
        /// <param name="arguments">An object array that contains zero or more arguments used to format the <paramref name="value"/> </param>
        /// <returns>A copy of the <paramref name="value"/> in which the format items have been replaced by the string representation of the <paramref name="arguments"/>.</returns>
        public static string FormatWith(this string value, params object[] arguments)
        {
            return string.Format(value, arguments);
        }

        /// <summary>
        /// Gets the upper case letters from the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value for which to get the uppercase letters.</param>
        /// <returns>The upper case letters from the given <paramref name="value"/>.</returns>
        public static string GetUpperCaseLetters(this string value)
        {
            return LowerCaseRegex.Replace(value, string.Empty);
        }
    }
}