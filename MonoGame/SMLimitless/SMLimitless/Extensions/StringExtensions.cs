//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Contains extension methods for the String class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a substring between two indexes.
        /// </summary>
        /// <param name="str">The string from which to extract the substring.</param>
        /// <param name="startIndex">The starting index of the substring in the string.</param>
        /// <param name="endIndex">The ending index of the substring in the string.</param>
        /// <returns>The extracted substring.</returns>
        public static string Substring(this string str, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// Removes INI comments from INI-styled string arrays.
        /// A comment begins with a semicolon (;), and can start
        /// a line or be inline.
        /// </summary>
        /// <param name="input">A list of strings (e.g. a file) from which to extract comments.</param>
        /// <returns>The list of strings, minus the comments.</returns>
        public static List<string> RemoveComments(this List<string> input)
        {
            int i = 0;
            while (i < input.Count)
            {
                if (input[i].StartsWith(";"))
                {
                    // comment at beginning of line
                    input.RemoveAt(i);
                }
                else if (input[i].Contains(";"))
                {
                    // comment in middle of line
                    int pos = input[i].IndexOf(";");
                    input[i] = input[i].Substring(0, pos);
                    i++;
                }
                else
                {
                    i++;
                }
            }

            return input;
        }

        /// <summary>
        /// Removes INI comments from INI-styled string arrays.
        /// A comment begins with a semicolon (;), and can start
        /// a line or be inline.
        /// </summary>
        /// <param name="input">A string array (e.g. a file) to remove comments from.</param>
        /// <returns>The string array, minus the comments.</returns>
        public static string[] RemoveComments(this string[] input)
        {
            return RemoveComments(input.ToList()).ToArray();
        }

        /// <summary>
        /// Trims evert string in a string array.
        /// </summary>
        /// <param name="input">The string array containing the strings to trim.</param>
        /// <returns>A string array containing the trimmed strings.</returns>
        public static string[] TrimStringArray(this string[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim();
            }

            return input;
        }

        /// <summary>
        /// Draws a string to the screen.
        /// </summary>
        /// <param name="value">The string to draw.</param>
        /// <param name="position">The position on the screen to draw it at.</param>
        /// <param name="color">The color of the text.</param>
        public static void DrawString(this string value, Microsoft.Xna.Framework.Vector2 position, float scale = 1f)
        {
            GameServices.DebugFont.DrawString(value, position, scale);
        }

        /// <summary>
        /// Draws a string in the color white to the screen at the position of (16, 16) using the default font.
        /// </summary>
        /// <param name="value">The string to draw.</param>
        public static void DrawStringDefault(this string value)
        {
            GameServices.DrawStringDefault(value);
        }
    }
}
