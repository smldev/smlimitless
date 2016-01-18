//-----------------------------------------------------------------------
// <copyright file="NumericExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Contains helper methods for numeric types.
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// Rounds a decimal up to the nearest integral value
        /// if the decimal has a fractional component. For
        /// example, 5.1 will be rounded to 6, and 7.9 will
        /// be rounded to 8.
        /// </summary>
        /// <param name="input">The decimal to round up.</param>
        /// <returns>The decimal rounded up to the nearest integer.</returns>
        public static decimal RoundUp(decimal input)
        {
            if (input % 1 != 0)
            {
                // if the input isn't an integer
                input += 1m;
                return Math.Truncate(input);
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Rounds a decimal down to the nearest integral value.
        /// For example, 5.1 will be rounded to 5, and 7.9 will
        /// be rounded to 7.
        /// </summary>
        /// <param name="input">The decimal to round down.</param>
        /// <returns>The decimal rounded down to the nearest integer.</returns>
        public static decimal RoundDown(decimal input)
        {
            return Math.Truncate(input);
        }

        /// <summary>
        /// Rounds a decimal to the nearest integral value.
        /// If the fractional component is 0.4 or less, the decimal
        /// will be rounded down (5.1 will be rounded to 5). If
        /// the fractional component is 0.5 or more, the decimal will
        /// be rounded up (7.9 will be rounded to 8).
        /// </summary>
        /// <param name="input">The decimal to round.</param>
        /// <returns>The rounded decimal.</returns>
        public static decimal RoundNearest(decimal input)
        {
            if (input % 1 != 0)
            {
                // if the input isn't an integer
                decimal fraction = input - Math.Truncate(input); // remove the integral part
                if (fraction >= 0.5m)
                {
                    return RoundUp(input);
                }
                else if (fraction < 0.5m)
                {
                    return RoundDown(input);
                }
            }

            return input;
        }

        /// <summary>
        /// Returns a number representing the sign of a given number.
        /// </summary>
        /// <param name="value">The value of which to get the sign.</param>
        /// <returns>1 for positive values, -1 for negative values, and 0 for zero.</returns>
        public static float Sign(this float value)
        {
            if (value < 0f)
            {
                return -1f;
            }
            else if (value > 0f)
            {
                return 1f;
            }

            return 0f;
        }

        /// <summary>
        /// Performs a given action a certain number of times.
        /// Includes an iteration number variable.
        /// </summary>
        /// <param name="iterations">The number of times to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <remarks>Useful for replacing for loops for small actions.
        /// This method is zero-based - the first iteration is zero and the
        /// last iteration is (iterations - 1).</remarks>
        public static void Times(this int iterations, Action<int> action)
        {
            for (int i = 0; i < iterations; i++)
            {
                action(i);
            }
        }

        ////public static bool Equals(this float a, float b, float epsilon)
        ////{

        ////}

        /// <summary>
        /// Corrects the value of a single-precision float to the nearest integral value if the float is very close to that value.
        /// </summary>
        /// <param name="value">The value to correct.</param>
        /// <returns>A whole-number corrected value, or the value if it was not close enough to the nearest integers.</returns>
        public static float CorrectPrecision(this float value)
        {
            float epsilon = 0.0001f;

            int ceiling = (int)(value + 1f);
            int floor = (int)value;

            if (Math.Abs(ceiling - value) < epsilon)
            {
                return ceiling;
            }
            else if (Math.Abs(value - floor) < epsilon)
            {
                return floor;
            }
            else
            {
                return value;
            }
        }

		/// <summary>
		/// Determines if a number if between two other numbers.
		/// </summary>
		/// <param name="x">The number to check.</param>
		/// <param name="a">The first number of the range.</param>
		/// <param name="b">The second number of the range.</param>
		/// <returns></returns>
		public static bool BetweenInclusive(this float x, float a, float b)
		{
			if (a > b)
			{
				return (x >= a) && (x <= b);
			}
			else if (a < b)
			{
				return (x >= b) && (x <= a);
			}
			else if (a == b)
			{
				return (x == a) && (x == b);
			}
			return false;
		}

		public static int Clamp(this int value, int min, int max)
		{
			if (value < min) { return min; }
			else if (value > max) { return max; }
			return value;
		}
    }
}
