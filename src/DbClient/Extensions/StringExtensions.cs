namespace DbClient.Extensions
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
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

        /// <summary>
        /// Creates a cache key based upon the given <paramref name="keys"/>.
        /// </summary>
        /// <param name="keys">The strings for which to create a cache key.</param>
        /// <returns>A cache key based upon the given <paramref name="keys"/>.</returns>
        public static string CreateCacheKey(this string[] keys)
        {
            // Calculate the total length of the combined key
            int totalLength = 0;
            foreach (var key in keys)
            {
                totalLength += key.Length;
            }
            totalLength += keys.Length - 1; // Account for delimiters

            // Use stackalloc to avoid heap allocations for small keys
            Span<char> buffer = totalLength <= 256 ? stackalloc char[totalLength] : new char[totalLength];

            // Combine the keys into the buffer
            int position = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].AsSpan().CopyTo(buffer.Slice(position));
                position += keys[i].Length;

                if (i < keys.Length - 1)
                {
                    buffer[position] = '|'; // Delimiter
                    position++;
                }
            }

            // Convert the Span<char> to a byte array using UTF8 encoding
            int byteCount = Encoding.UTF8.GetByteCount(buffer);
            Span<byte> byteBuffer = byteCount <= 256 ? stackalloc byte[byteCount] : new byte[byteCount];
            Encoding.UTF8.GetBytes(buffer, byteBuffer);

            // Generate a hash of the combined key
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(byteBuffer.ToArray());
                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }
    }
}