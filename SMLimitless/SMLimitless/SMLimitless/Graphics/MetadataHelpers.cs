using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// A class of helper methods that work with graphics metadata strings.
    /// </summary>
    internal static class MetadataHelpers
    {
        /// <summary>
        /// Extracts all substrings from a string between two characters (for example, parentheses or brackets).
        /// </summary>
        /// <param name="fullString">The string to extract the substring from.</param>
        /// <param name="start">The opening character (for example, a left parentheses or bracket).</param>
        /// <param name="end">The closing character (for example, a right parentheses or bracket).</param>
        /// <returns>A list of all substrings between two characters.</returns>
        [Obsolete]
        internal static List<String> ExtractStringBetweenChars(string fullString, char start, char end)
        {
            // Sanity check - see if there actually are any start/end characters inside
            if (!fullString.Contains(start) || !fullString.Contains(end)) return new List<string> { fullString };

            var startCharIndexes = new List<int>();
            var endCharIndexes = new List<int>();
            int i;
            
            // First, find the indexes of all the start characters
            for (i = 0; i < fullString.Length; i++)
            {
                if (fullString[i] == start)
                {
                    startCharIndexes.Add(i);
                }
            }

            // Now, find the indexes of all the end characters.
            // Due to possible nested substrings, we need to search from the end of the string backwards.
            for (i = fullString.Length - 1; i >= 0; i--)
            {
                if (fullString[i] == end)
                {
                    endCharIndexes.Add(i);
                }
            }

            // Now, we should have a full set of indexes that will grab the outermost nests.
            var result = new List<string>();

            for (i = 0; i < startCharIndexes.Count; i++)
            {
                result.Add(StringExtensions.Substring(fullString, startCharIndexes[i] + 1, endCharIndexes[i]));
            }

            return result;
        }
    }
}
