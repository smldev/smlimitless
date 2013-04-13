using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a substring between two indexes.
        /// </summary>
        public static string Substring(this string str, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// Removes INI comments from INI-styled string arrays (such as world files).
        /// </summary>
        public static List<string> RemoveComments(this List<string> input)
        {
            int i = 0;
            while (i < input.Count)
            {
                // comment at beginning of line
                if (input[i].StartsWith(";"))
                {
                    input.RemoveAt(i);
                }
                // comment in middle of line
                else if (input[i].Contains(";"))
                {
                    int pos = input[i].IndexOf(";");
                    input[i] = input[i].Substring(0, pos);
                    i++;
                }
                else i++;
            }
            return input;
        }

        public static string[] RemoveComments(this string[] input)
        {
            return RemoveComments(input.ToList()).ToArray();
        }

        public static string[] TrimStringArray(this string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim();
            }
            return input;
        }
    }
}
