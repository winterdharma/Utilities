using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Unicode
{
    public static class Utf8
    {
        /// <summary>
        /// This method provides an accurate count of characters in an UTF-8 encoded string
        /// taking into account any triple-byte Unicode characters encountered.
        /// </summary>
        /// <param name="utf8"></param>
        /// <returns></returns>
        public static int Length(string utf8)
        {
            int length = 0;
            foreach (string cjk in AsCodePoints(utf8))
            {
                length++;
            }
            return length;
        }


        public static string Substring(string str, int startIndex, int length)
        {
            List<string> indexedCodePoints = new List<string>();
            foreach (string cjk in AsCodePoints(str))
            {
                indexedCodePoints.Add(cjk);
            }

            var outputStr = new StringBuilder();
            for (int i = startIndex; i < (startIndex + length); i++)
            {
                outputStr.Append(indexedCodePoints[i]);
            }

            return outputStr.ToString();
        }


        public static string Substring(string str, int startIndex)
        {
            List<string> indexedCodePoints = new List<string>();
            foreach (string cjk in AsCodePoints(str))
            {
                indexedCodePoints.Add(cjk);
            }

            var sb = new StringBuilder();
            for (int i = startIndex; i < indexedCodePoints.Count; i++)
            {
                sb.Append(indexedCodePoints[i]);
            }

            return sb.ToString();
        }


        private static IEnumerable<string> AsCodePoints(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                yield return char.ConvertFromUtf32(char.ConvertToUtf32(str, i));
                if (char.IsHighSurrogate(str, i))
                    i++;
            }
        }


        public static string RemoveLeadingChars(string str, params char[] charsToRemove)
        {
            int index = 0;
            bool charRemoved = false;
            foreach (string character in AsCodePoints(str))
            {
                foreach (char chr in charsToRemove)
                {
                    if (character.Equals(chr.ToString()))
                    {
                        index++;
                        charRemoved = true;
                        break;
                    }
                }

                if (!charRemoved)
                    break;
                charRemoved = false;
            }
            str = Substring(str, index);
            return str;
        }
    }
}
