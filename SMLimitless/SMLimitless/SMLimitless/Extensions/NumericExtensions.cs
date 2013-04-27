using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
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
        public static decimal RoundUp(Decimal input)
        {
            if (input % 1 != 0) // if the input isn't an integer
            {
                input += 1m;
                return Math.Truncate(input);
            }
            else return input;
        }

        /// <summary>
        /// Rounds a decimal down to the nearest integral value.
        /// For example, 5.1 will be rounded to 5, and 7.9 will
        /// be rounded to 7.
        /// </summary>
        /// <param name="input">The decimal to round down.</param>
        /// <returns>The decimal rounded down to the nearest integer.</returns>
        public static decimal RoundDown(Decimal input)
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
        public static decimal RoundNearest(Decimal input)
        {
            if (input % 1 != 0) // if the input isn't an integer
            {
                Decimal fraction = input - Math.Truncate(input); // remove the integral part
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
    }
}
