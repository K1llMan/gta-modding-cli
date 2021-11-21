using System.Linq;
using System.Text.RegularExpressions;

namespace GtaModdingCli.Extensions
{
    /// <summary>
    /// Regex options
    /// </summary>
    public enum RegExOptions
    {
        None = 0,
        IgnoreCase = 1,
        Multiline = 2,
        ExplicitCapture = 4,
        Singleline = 16,
        IgnorePatternWhitespace = 32,
        RightToLeft = 64,
        ECMAScript = 256,
        CultureInvariant = 512,
    }

    /// <summary>
    /// Regex string extensions
    /// </summary>
    public static class StringCommon
    {
        #region Работа с регулярными выражениями

        /// <summary>
        /// Regex replace
        /// </summary>
        /// <param name="regExpr">Regex</param>
        /// <param name="str">String</param>
        /// <param name="replStr">Replacement</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string str, string regExpr, string replStr, RegExOptions options)
        {
            return str == null
                ? string.Empty
                : Regex.Replace(str, regExpr, replStr, (RegexOptions)options);
        }

        /// <summary>
        /// Regex replace
        /// </summary>
        /// <param name="regExpr">Regex</param>
        /// <param name="str">String</param>
        /// <param name="replStr">Replacement</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string str, string regExpr, string replStr)
        {
            return ReplaceRegex(str, regExpr, replStr, RegExOptions.None);
        }

        /// <summary>
        /// Remove letters
        /// </summary>
        /// <param name="str">String</param>
        public static string RemoveLetters(this string str)
        {
            return ReplaceRegex(str, @"[^\d]", "");
        }

        /// <summary>
        /// Remove substring
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="subStr">Substring</param>
        public static string RemoveSubStr(this string str, string subStr)
        {
            return ReplaceRegex(str, subStr, "");
        }

        /// <summary>
        /// Replace substring
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="subStr">Substring</param>
        /// <param name="newSubStr">Replacement</param>
        public static string ReplaceSubStr(this string str, string subStr, string newSubStr)
        {
            return ReplaceRegex(str, subStr, newSubStr);
        }

        /// <summary>
        /// Trim numbers
        /// </summary>
        /// <param name="str">String</param>
        public static string TrimNumbers(this string str)
        {
            return ReplaceRegex(str, @"^[\d]*|[\d]*$", "");
        }

        /// <summary>
        /// Remove numbers
        /// </summary>
        /// <param name="str">String</param>
        public static string RemoveNumbers(this string str)
        {
            return ReplaceRegex(str, @"[\d]", "");
        }

        /// <summary>
        /// Trim letters
        /// </summary>
        /// <param name="str">String</param>
        public static string TrimLetters(this string str)
        {
            string exp = @"-?[\d](.*)(?<![^\d])";
            if (Regex.IsMatch(str, exp))
                return Regex.Match(str, exp).Captures[0].Value;
            return string.Empty;
        }

        /// <summary>
        /// Remove spaces
        /// </summary>
        /// <param name="str">String</param>
        public static string RemoveSpaces(this string str)
        {
            return ReplaceRegex(str, @"\s+", "");
        }

        /// <summary>
        /// Replace spaces
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="replacementStr">Replacement</param>
        public static string ReplaceSpaces(this string str, string replacementStr)
        {
            return ReplaceRegex(str, @"\s+", replacementStr);
        }

        /// <summary>
        /// Check by pattern
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="pattern">Regex</param>
        /// <param name="options">Options</param>
        public static bool IsMatch(this string str, string pattern, RegExOptions options)
        {
            return Regex.IsMatch(str, pattern, (RegexOptions)options);
        }

        /// <summary>
        /// Check by pattern
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="pattern">Regex</param>
        public static bool IsMatch(this string str, string pattern)
        {
            return IsMatch(str, pattern, RegExOptions.IgnoreCase);
        }

        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="pattern">Regex</param>
        /// <param name="options">Options</param>
        public static string [] GetMatches(this string str, string pattern, RegExOptions options)
        {
            if (str.IsMatch(pattern, options))
                return Regex.Matches(str, pattern, (RegexOptions)options).Cast<Match>().Select(m => m.Value).ToArray();

            return new string[] { };
        }

        /// <summary>
        /// Get all matches
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="pattern">Regex</param>
        public static string[] GetMatches(this string str, string pattern)
        {
            return str.GetMatches(pattern, RegExOptions.IgnoreCase);
        }

        #endregion Работа с регулярными выражениями
    }
}
