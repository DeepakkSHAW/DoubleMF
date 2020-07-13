using System;

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
    }
}
