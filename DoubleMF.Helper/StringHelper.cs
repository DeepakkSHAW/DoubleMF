using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DoubleMF.Helper
{
    public static class StringHelper
    {
        public static string GetBeforeToStart(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }
        public static string GetAfterToEnd(this string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal) + 1;

                if (charLocation > 1)
                {
                    return text.Substring(charLocation, text.Length - charLocation);
                }
            }

            return String.Empty;
        }
        public static string TruncateWithDots(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars - 3) + "...";
        }
        public static bool IsNumericType(string o)
        {
            return double.TryParse(o, out double dummy);
        }
        public static bool IsNumeric(this string text)
        {
            var isNumeric = int.TryParse(text, out int n);
            return isNumeric;
        }
        public static int ToNumeric(this string text)
        {
            var isNumeric = int.TryParse(text, out int n);
            return n;
        }
        public static int? ParseNullableInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value.ToNumeric();
        }
        public static bool IsDouble(this string text)
        {
            var isDouble = double.TryParse(text, out double n);
            return isDouble;
        }
        public static double ToDouble(this string text)
        {
            var isDouble = double.TryParse(text, out double n);
            return n;
        }
        public static bool IsDate(this string text)
        {
            var isDate = DateTime.TryParse(text, out DateTime n);
            return isDate;
        }
        public static DateTime ToDate(this string text)
        {
            var isDate = DateTime.TryParse(text, out DateTime n);
            return n;
        }
        public static bool IsBoolean(this string text)
        {
            var isDouble = bool.TryParse(text, out bool n);
            return isDouble;
        }
        public static bool ToBoolean(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("value");
            }
            string val = text.ToLower().Trim();
            switch (val)
            {
                case "false":
                    return false;
                case "f":
                    return false;
                case "true":
                    return true;
                case "t":
                    return true;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    throw new ArgumentException("Invalid boolean");
            }
        }
        public static bool IsEmailAddress(this string email)
        {
            string pattern =
                "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(email, pattern).Success;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        ///     Convert a string to its equivalent byte array
        /// </summary>
        /// <param name="val">string to convert</param>
        /// <returns>System.byte array</returns>
        public static byte[] ToBytes(this string val)
        {
            var bytes = new byte[val.Length * sizeof(char)];
            Buffer.BlockCopy(val.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     Reverse string
        /// </summary>
        /// <param name="val">string to reverse</param>
        /// <returns>System.string</returns>
        public static string Reverse(this string val)
        {
            var chars = new char[val.Length];
            for (int i = val.Length - 1, j = 0; i >= 0; --i, ++j)
            {
                chars[j] = val[i];
            }
            val = new String(chars);
            return val;
        }

        /// <summary>
        ///     Encrypt a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToEncrypt">String that must be encrypted.</param>
        /// <param name="key">Encryption key</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt(this string stringToEncrypt, string key)
        {
            var cspParameter = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameter) { PersistKeyInCsp = true };
            byte[] bytes = rsaServiceProvider.Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt), true);
            return BitConverter.ToString(bytes);
        }


        /// <summary>
        ///     Decrypt a string using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToDecrypt">String that must be decrypted.</param>
        /// <param name="key">Decryption key.</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        /// 
        public static string Decrypt(this string stringToDecrypt, string key)
        {
            var cspParamters = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParamters) { PersistKeyInCsp = true };
            string[] decryptArray = stringToDecrypt.Split(new[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll(decryptArray,
                (s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber))));
            byte[] bytes = rsaServiceProvider.Decrypt(decryptByteArray, true);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }
        
        /// <summary>
        ///     Removes the first part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no prefix or original string</returns>
        public static string RemovePrefix(this string val, string prefix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(val) && (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix)))
            {
                return val.Substring(prefix.Length, val.Length - prefix.Length);
            }
            return val;
        }

        /// <summary>
        ///     Removes the end part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no suffix or original string</returns>
        public static string RemoveSuffix(this string val, string suffix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(val) && (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix)))
            {
                return val.Substring(0, val.Length - suffix.Length);
            }
            return null;
        }

        /// <summary>
        ///     Convert url query string to IDictionary value key pair
        /// </summary>
        /// <param name="queryString">query string value</param>
        /// <returns>IDictionary value key pair</returns>
        public static IDictionary<string, string> QueryStringToDictionary(this string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }
            if (!queryString.Contains("?"))
            {
                return null;
            }
            string query = queryString.Replace("?", "");
            if (!query.Contains("="))
            {
                return null;
            }
            return query.Split('&').Select(p => p.Split('=')).ToDictionary(
                key => key[0].ToLower().Trim(), value => value[1]);
        }

        /// <summary>
        ///     Check a String ends with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true or false</returns>
        public static bool EndsWithIgnoreCase(this string val, string suffix)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix", "suffix parameter is null");
            }
            if (val.Length < suffix.Length)
            {
                return false;
            }
            return val.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Check a String starts with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true or false</returns>
        public static bool StartsWithIgnoreCase(this string val, string prefix)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix", "prefix parameter is null");
            }
            if (val.Length < prefix.Length)
            {
                return false;
            }
            return val.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Appends the suffix to the end of the string if the string does not already end in the suffix.
        /// </summary>
        /// <param name="val">string to append suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string AppendSuffixIfMissing(this string val, string suffix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val) || (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix)))
            {
                return val;
            }
            return val + suffix;
        }

        /// <summary>
        ///     Appends the prefix to the start of the string if the string does not already start with prefix.
        /// </summary>
        /// <param name="val">string to append prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string AppendPrefixIfMissing(this string val, string prefix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val) || (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix)))
            {
                return val;
            }
            return prefix + val;
        }

    }
}
