using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

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
        internal static List<String> ExtractStringBetweenChars(string fullString, char start, char end)
        {
            // Sanity check - see if there actually are any start/end characters inside
            if (!fullString.Contains(start) || !fullString.Contains(end)) return new List<string> { fullString };

            var startCharIndexes = new List<int>();
            var endCharIndexes = new List<int>();
            int i;
            
            // Find the indexes of all the start and end characters
            for (i = 0; i < fullString.Length; i++)
            {
                if (fullString[i] == start)
                {
                    startCharIndexes.Add(i);
                }
                else if (fullString[i] == end)
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

        /// <summary>
        /// Returns a single rectangle from a metadata string.
        /// Format: "[x,y,width,height]".  First rectangle is returned.
        /// </summary>
        internal static Rectangle ExtractSingleRectangle(string fullString)
        {
            var substrings = ExtractStringBetweenChars(fullString, '[', ']');
            string firstRectStr = substrings[0];

            // The substrings should contain characters in the format ###:###:###:###
            if (!firstRectStr.Contains(':')) { throw new Exception("MetadataHelpers.ExtractSingleRectangle: Invalid metadata rectangle format."); }

            var firstRectSplit = firstRectStr.Split(':');
            int[] integers = new int[4];

            for (int i = 0; i < 4; i++)
            {
                integers[i] = Int32.Parse(firstRectSplit[i]);
            }

            return new Rectangle(integers[0], integers[1], integers[2], integers[3]);
        }

        /// <summary>
        /// Extracts a group of rectangles from a metadata string.
        /// </summary>
        internal static List<Rectangle> ExtractAllRectangles(string fullString)
        {
            var result = new List<Rectangle>();
            var substrings = ExtractStringBetweenChars(fullString, '[', ']');

            foreach (string substring in substrings)
            {
                if (!substring.Contains(':')) { throw new Exception("MetadataHelpers.ExtractSingleRectangle: Invalid metadata rectangle format."); }

                var split = substring.Split(':');
                Rectangle rect = new Rectangle(Int32.Parse(split[0]),
                                               Int32.Parse(split[1]),
                                               Int32.Parse(split[2]),
                                               Int32.Parse(split[3]));
                result.Add(rect);
            }

            return result;
        }

        internal static List<Rectangle> ExtractAllRectangles(string[] strArray)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in strArray) builder.Append(str);
            return ExtractAllRectangles(builder.ToString());
        }

        /// <summary>
        /// Extracts a string between quote (") characters.
        /// </summary>
        public static string TrimQuotes(string input)
        {
            if (input.StartsWith(@"""") && input.EndsWith(@""""))
            {
                return input.Substring(1, input.Length - 2);
            }
            else return input;
        }

        [Obsolete] // well, you usually have to split() it anyway
        public static string GetDatatypeHeader(string input)
        {
            if (input.Contains('>'))
            {
                return input.Split('>')[0];
            }
            else
            {
                throw new Exception("MetadataHelpers.GetDatatypeHeader: Invalid metadata or not metadata.");
            }
        }
    }
}
