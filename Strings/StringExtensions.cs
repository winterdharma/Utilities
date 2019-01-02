using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.Strings
{
    public static class StringExtensions
    {
        /// <summary>
        /// Searches for substrings with specified start and ending chars,
        /// removes them, and returns the resulting string. MaxLength is the max
        /// length of substrings to remove, including the start and end characters.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="startsWith">The first character of the search string.</param>
        /// <param name="endsWith">The last character of the search string.</param>
        /// <param name="maxLength">If greater than 0, limits the search to strings this length or 
        ///     shorter. If 0, there is no limit.</param>
        public static string Scrub(this string s, char startsWith, char endsWith, int maxLength = 0)
        {
            string start = startsWith.ToString();
            string ending = endsWith.ToString();

            if (startsWith.Equals('[') || startsWith.Equals(']'))
                start = @"\" + start;

            if (endsWith.Equals('[') || endsWith.Equals(']'))
                ending = @"\" + ending;

            string searchPattern;
            if(maxLength == 0)
                searchPattern = "^" + start + "$" + ending;
            else
            {
                int innerChars = maxLength - 2;
                searchPattern = start + @".{0," + innerChars + @"}" + ending;
            }

            s = Regex.Replace(s, searchPattern, String.Empty);
            
            return s;
        }

        /// <summary>
        /// Removes specified number of characters from end of string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Truncate(this string s, int length)
        {
            return s.Substring(0, s.Length - length);
        }

        /// <summary>
        /// Removes specified number of characters from beginning of string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TruncateStart(this string s, int length)
        {
            return s.Substring(length);
        }

        /// <summary>
        /// Perform same function as Replace(), but only replaces the first match found.
        /// If no match is found, returns the original string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <returns></returns>
        public static string ReplaceFirstInstanceOf(this string s, string oldString, string newString)
        {
            if(s.Contains(oldString))
            {
                int index = s.IndexOf(oldString);
                int length = oldString.Length;
                if(index == 0)
                    s = newString + s.Substring(length);
                else if (index + length == s.Length)
                    s = s.Substring(0, s.Length - length) + newString;
                else
                    s = s.Substring(0, index) + newString + s.Substring(index + length);
            }
            return s;
        }

        public static string ReplaceAt(this string s, int index, string oldString, string newString)
        {
            if (s.Contains(oldString))
            {
                int length = oldString.Length;
                if (index == 0)
                    s = newString + s.Substring(length);
                else if (index + length == s.Length)
                    s = s.Substring(0, s.Length - length) + newString;
                else
                    s = s.Substring(0, index) + newString + s.Substring(index + length);
            }
            return s;
        }
    }
}
