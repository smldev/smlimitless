﻿//-----------------------------------------------------------------------
// <copyright file="Vector2Extensions.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        /// Checks the equality of the components of a collection of Vector2s.
        /// </summary>
        /// <param name="vectors">The collection to check.</param>
        /// <returns>The equality of the components of a collection of Vector2s.</returns>
        [Obsolete]
        public static VectorCollectionEqualityTypes GetVectorEqualityTypes(IEnumerable<Vector2> vectors)
        {
            float lastX = float.NaN;
            float lastY = float.NaN;
            bool someXComponentsEqual = false, someYComponentsEqual = false;

            vectors = vectors.OrderBy(v => v.X);
            foreach (Vector2 vector in vectors)
            {
                if (vector.X == lastX)
                {
                    someXComponentsEqual = true;
                    break;
                }
                else
                {
                    lastX = vector.X;
                }
            }

            vectors = vectors.OrderBy(v => v.Y);
            foreach (Vector2 vector in vectors)
            {
                if (vector.Y == lastY)
                {
                    someYComponentsEqual = true;
                    break;
                }
                else
                {
                    lastY = vector.Y;
                }
            }

            if (someXComponentsEqual && someYComponentsEqual)
            {
                return VectorCollectionEqualityTypes.Both;
            }
            else if (someXComponentsEqual)
            {
                return VectorCollectionEqualityTypes.SomeXComponentsEqual;
            }
            else if (someYComponentsEqual)
            {
                return VectorCollectionEqualityTypes.SomeYComponentsEqual;
            }
            else
            {
                return VectorCollectionEqualityTypes.NoEquality;
            }
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
    }
}
