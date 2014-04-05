//-----------------------------------------------------------------------
// <copyright file="Vector2Extensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Contains extension methods for the Vector2 structure.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Checks if one or both of the components of a Vector2 are equal to Single.NaN.
        /// </summary>
        /// <param name="vector">The Vector2 to check.</param>
        /// <returns>True if one or both of the components equal Single.NaN, false if neither do.</returns>
        public static bool IsNaN(this Vector2 vector)
        {
            return float.IsNaN(vector.X) || float.IsNaN(vector.Y);
        }

        /// <summary>
        /// Converts this Vector2 and another Vector2 into a rectangle.
        /// </summary>
        /// <param name="position">The Vector2 that will become the X and Y components of the rectangle.</param>
        /// <param name="size">The Vector2 that will become the Width and Height components of the rectangle.</param>
        /// <returns>A rectangle constructed from the two vectors.</returns>
        /// <remarks>As the Rectangle type uses integers for components, any fractional component will be lost.</remarks>
        public static Rectangle ToRectangle(this Vector2 position, Vector2 size)
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        /// <summary>
        /// Floors both components of a Vector2.
        /// </summary>
        /// <param name="vector">The vector to floor.</param>
        /// <returns>The floored vector.</returns>
        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2((float)Math.Floor(vector.X), (float)Math.Floor(vector.Y));
        }

        /// <summary>
        /// Floors and Vector2 and divides the result.
        /// </summary>
        /// <param name="vector">The original vector to floor and divide.</param>
        /// <param name="divisor">The number by which to divide the floored vector by.</param>
        /// <returns>A vector with both components floored and divided by the divisor.</returns>
        public static Vector2 FloorDivide(this Vector2 vector, float divisor)
        {
            return new Vector2((float)Math.Floor(vector.X / divisor), (float)Math.Floor(vector.Y / divisor));
        }

        /// <summary>
        /// Floors and Vector2 and divides the result by another vector.
        /// </summary>
        /// <param name="vector">The original vector to floor and divide.</param>
        /// <param name="divisor">The vector by which to divide the floored vector by.</param>
        /// <returns>A vector with both components floored and divided by the components in the divisor.</returns>
        public static Vector2 FloorDivide(this Vector2 vector, Vector2 divisor)
        {
            return new Vector2((float)Math.Floor(vector.X / divisor.X), (float)Math.Floor(vector.Y / divisor.Y));
        }

        /// <summary>
        /// Returns a vector with the absolute values of the components.
        /// </summary>
        /// <param name="vector">The vector to get the absolute value of.</param>
        /// <returns>A vector with the absolute values of the components.</returns>
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2((float)Math.Abs(vector.X), (float)Math.Abs(vector.Y));
        }

        /// <summary>
        /// Divides the components of a vector by another, and returns the remainder.
        /// </summary>
        /// <param name="a">The divisor vector.</param>
        /// <param name="b">The dividend vector.</param>
        /// <returns>The remainder of the quotient.</returns>
        public static Vector2 Mod(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.X % b.X, a.Y % b.Y);
        }

        /// <summary>
        /// Compares the components of one vector to another.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>True if left is greater than right, false if otherwise.</returns>
        public static bool GreaterThan(this Vector2 left, Vector2 right)
        {
            return (left.X > right.X) && (left.Y > right.Y);
        }

        /// <summary>
        /// Compares the components of one vector to another.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>True if left is less than right, false if otherwise.</returns>
        public static bool LessThan(this Vector2 left, Vector2 right)
        {
            return (left.X < right.X) && (left.Y < right.Y);
        }

        /// <summary>
        /// Compares the components of one vector to another.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>True if left is greater than or equal to right, false if otherwise.</returns>
        public static bool GreaterThanOrEqualTo(this Vector2 left, Vector2 right)
        {
            return (left.X >= right.X) && (left.Y >= right.Y);
        }

        /// <summary>
        /// Compares the components of one vector to another.
        /// </summary>
        /// <param name="left">The first vector.</param>
        /// <param name="right">The second vector.</param>
        /// <returns>True if left is less than or equal to right, false if otherwise.</returns>
        public static bool LessThanOrEqualTo(this Vector2 left, Vector2 right)
        {
            return (left.X <= right.X) && (left.Y <= right.Y);
        }

        /// <summary>
        /// Parses a string containing a vector value formatted "x,y".
        /// </summary>
        /// <param name="input">The string, containing only a vector formatted "x,y", to parse.</param>
        /// <returns>The parsed vector.</returns>
        public static Vector2 Parse(string input)
        {
            var values = input.Split(',');
            if (values.Length != 2)
            {
                throw new Exception(string.Format("Vector2Extensions.Parse(string): Attempted to parse invalid string {0}", input));
            }

            float x, y;
            if (!float.TryParse(values[0], out x))
            {
                throw new Exception(string.Format("Vector2Extensions.Parse(string): X Component of vector is not a valid number. Tried to parse {0}", values[0]));
            }

            if (!float.TryParse(values[1], out y))
            {
                throw new Exception(string.Format("Vector2Extensions.Parse(string): Y Component of vector is not a valid number. Tried to parse {0}", values[1]));
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns a vector with the largest Y component, given a collection of vectors.
        /// </summary>
        /// <param name="vectors">A collection of vectors.</param>
        /// <returns>The vector with the largest Y component.</returns>
        public static Vector2 GreatestVectorByY(IEnumerable<Vector2> vectors)
        {
            Vector2 largestSoFar = new Vector2(0f, 0f);
            foreach (Vector2 vector in vectors)
            {
                if (Math.Abs(vector.Y) > Math.Abs(largestSoFar.Y))
                {
                    largestSoFar = vector;
                }
            }

            return (largestSoFar.Y != float.MinValue) ? largestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Returns a vector with the largest X component, given a collection of vectors.
        /// </summary>
        /// <param name="vectors">A collection of vectors.</param>
        /// <returns>The vector with the largest X component.</returns>
        public static Vector2 GreatestVectorByX(IEnumerable<Vector2> vectors)
        {
            Vector2 largestSoFar = new Vector2(0f, 0f);
            foreach (Vector2 vector in vectors)
            {
                if (Math.Abs(vector.X) > Math.Abs(largestSoFar.X))
                {
                    largestSoFar = vector;
                }
            }

            return (largestSoFar.X != float.MinValue) ? largestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Returns the vector with the lowest Y component, given a collection of vectors.
        /// </summary>
        /// <param name="vectors">The collection of vectors.</param>
        /// <returns>The vector with the lowest Y component.</returns>
        public static Vector2 LeastVectorByY(IEnumerable<Vector2> vectors)
        {
            Vector2 smallestSoFar = new Vector2(float.MaxValue, float.MaxValue);

            foreach (Vector2 vector in vectors)
            {
                if (Math.Abs(vector.Y) < Math.Abs(smallestSoFar.Y) && vector.Y != 0f)
                {
                    smallestSoFar = vector;
                }
            }

            return (smallestSoFar != new Vector2(float.MaxValue)) ? smallestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Returns the vector with the lowest X component, given a collection of vectors.
        /// </summary>
        /// <param name="vectors">A collection of vectors.</param>
        /// <returns>The vector with the lowest X component.</returns>
        public static Vector2 LeastVectorByX(IEnumerable<Vector2> vectors)
        {
            Vector2 smallestSoFar = new Vector2(float.MaxValue, float.MaxValue);

            foreach (Vector2 vector in vectors)
            {
                if (Math.Abs(vector.X) < Math.Abs(smallestSoFar.X) && vector.X != 0f)
                {
                    smallestSoFar = vector;
                }
            }

            return (smallestSoFar != new Vector2(float.MaxValue)) ? smallestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Converts a JToken value into a Vector2.
        /// </summary>
        /// <param name="jsonEntry">A JSON token containing two comma-delimited numbers.</param>
        /// <returns>A Vector2 converted from the token.</returns>
        public static Vector2 ToVector2(this JToken jsonEntry)
        {
            string json = jsonEntry.ToString();
            string[] values = json.Split(',');

            if (values.Length != 2)
            {
                throw new ArgumentException("Vector2Extensions.ToVector2(JToken): Tried to turn a non-vector object into a vector.");
            }

            if (values[0].Contains("NaN") || values[1].Contains("NaN"))
            {
                return new Vector2(float.NaN, float.NaN);
            }

            values[1] = values[1].TrimStart(); // there's a space on the front
            float x, y;
            if (!float.TryParse(values[0], out x)) { throw new ArgumentException("Vector2Extensions.ToVector2(JToken): Invalid value for X component."); }
            if (!float.TryParse(values[1], out y)) { throw new ArgumentException("Vector2Extensions.ToVector2(JToken): Invalid value for Y component."); }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts a string, formatted as "x,y", into a Vector2.
        /// </summary>
        /// <param name="value">The string from which to convert.</param>
        /// <returns>A Vector2 containing the values of the string.</returns>
        public static Vector2 FromString(this string value)
        {
            string[] values = value.Split(',');
            values[1].Trim();

            float x, y;

            if (!float.TryParse(values[0], out x)) { throw new Exception(string.Format("Vector2Extensions.FromString(this string): Invalid value for X component. Got {0}.", values[0])); }
            if (!float.TryParse(values[1], out y)) { throw new Exception(string.Format("Vector2Extensions.FromString(this string): Invalid value for Y component. Got {0}.", values[1])); }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Produces a string representation of this vector usable for serializers and deserializers.
        /// </summary>
        /// <param name="value">The vector to serialize.</param>
        /// <returns>A string in the format of "X, Y".</returns>
        public static string Serialize(this Vector2 value)
        {
            return string.Format("{0}, {1}", value.X, value.Y);
        }

        /// <summary>
        /// Produces a list of strings containing serialized forms of a list of vectors.
        /// </summary>
        /// <param name="value">The vectors to serialize.</param>
        /// <returns>A list of strings in the format of "X, Y".</returns>
        public static List<string> Serialize(this List<Vector2> value)
        {
            return value.Select(v => v.Serialize()).ToList(); // yay LINQ
        }

        /// <summary>
        /// Gets the angle between two given vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle between the vectors.</returns>
        public static float GetAngleBetweenVectors(this Vector2 a, Vector2 b)
        {
            // Credit to http://stackoverflow.com/a/13459068/2709212
            return MathHelper.ToDegrees((float)Math.Atan2(b.Y - a.Y, b.X - a.X));
        }

        ////public static bool EqualityWithinEpsilon(this Vector2 a, Vector2 b, float epsilon)
        ////{
        // we have a lot of learning to do before this can work
        ////}

        public static string Serialize(this Point value)
        {
            return string.Format("{0}, {1}", value.X, value.Y);
        }
    }
}
