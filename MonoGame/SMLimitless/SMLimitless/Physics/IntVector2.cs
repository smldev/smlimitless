//-----------------------------------------------------------------------
// <copyright file="IntVector2.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a two-dimensional vector that uses
    /// 32-bit signed integers as components.
    /// </summary>
    public struct IntVector2
    {
        /// <summary>
        /// Gets an IntVector2 with all of its components set to zero.
        /// </summary>
        public static IntVector2 Zero
        {
            get
            {
                return new IntVector2(0, 0);
            }
        }

        /// <summary>
        /// Gets an IntVector2 with all of its components set to one.
        /// </summary>
        public static IntVector2 One
        {
            get
            {
                return new IntVector2(1, 1);
            }
        }

        /// <summary>
        /// Gets the unit vector for the X axis.
        /// </summary>
        public static IntVector2 UnitX
        {
            get
            {
                return new IntVector2(1, 0);
            }
        }

        /// <summary>
        /// Gets the unit vector for the X axis.
        /// </summary>
        public static IntVector2 UnitY
        {
            get
            {
                return new IntVector2(0, 1);
            }
        }

        /// <summary>
        /// Gets or sets the X component of the vector.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the vector.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntVector2"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the x-component of the vector.</param>
        /// <param name="y">Initial value for the y-component of the vector.</param>
        public IntVector2(int x, int y) : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntVector2"/> struct.
        /// </summary>
        /// <param name="value">Value to initialize both components to.</param>
        public IntVector2(int value) : this()
        {
            this.X = value;
            this.Y = value;
        }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="value1">The first vector to add.</param>
        /// <param name="value2">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static IntVector2 Add(IntVector2 value1, IntVector2 value2)
        {
            return value1 + value2;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="left">The first vector to add.</param>
        /// <param name="right">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static IntVector2 operator +(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="left">The first vector to subtract.</param>
        /// <param name="right">The second vector to subtract.</param>
        /// <returns>The difference of the two vectors.</returns>
        public static IntVector2 operator -(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Multiplies two vectors.
        /// </summary>
        /// <param name="left">The first vector to multiply.</param>
        /// <param name="right">The second vector to multiply.</param>
        /// <returns>The product of the two vectors.</returns>
        public static IntVector2 operator *(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        /// Divides two vectors.
        /// </summary>
        /// <param name="left">The first vector to divide.</param>
        /// <param name="right">The second vector to divide.</param>
        /// <returns>The quotient of the two vectors.</returns>
        public static IntVector2 operator /(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Divides two vectors and returns the remainder of the division.
        /// </summary>
        /// <param name="left">The first vector to divide.</param>
        /// <param name="right">The second vector to divide.</param>
        /// <returns>The remainder of the quotient of the two vectors.</returns>
        public static IntVector2 operator %(IntVector2 left, IntVector2 right)
        {
            return new IntVector2(left.X % right.X, left.Y % right.Y);
        }

        /// <summary>
        /// Checks if a vector is less than another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is less than the right vector, false if otherwise.</returns>
        public static bool operator <(IntVector2 left, IntVector2 right)
        {
            return (left.X < right.X) && (left.Y < right.Y);
        }

        /// <summary>
        /// Checks if a vector is greater than another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is greater than the right vector, false if otherwise.</returns>
        public static bool operator >(IntVector2 left, IntVector2 right)
        {
            return (left.X > right.X) && (left.Y > right.Y);
        }

        /// <summary>
        /// Checks if a vector is less than or equal to another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is less than or equal to the right vector, false if otherwise.</returns>
        public static bool operator <=(IntVector2 left, IntVector2 right)
        {
            return (left.X <= right.X) && (left.Y <= right.Y);
        }

        /// <summary>
        /// Checks if a vector is greater than or equal to another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is greater than or equal to the right vector, false if otherwise.</returns>
        public static bool operator >=(IntVector2 left, IntVector2 right)
        {
            return (left.X >= right.X) && (left.Y >= right.Y);
        }

        /// <summary>
        /// Checks if a vector is equal to another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is equal to the right vector, false if otherwise.</returns>
        public static bool operator ==(IntVector2 left, IntVector2 right)
        {
            return (left.X == right.X) && (left.Y == right.Y);
        }

        /// <summary>
        /// Checks if a vector is not equal to another.
        /// </summary>
        /// <param name="left">The first vector to check.</param>
        /// <param name="right">The second vector to check.</param>
        /// <returns>True if the left vector is not equal to the right vector, false if otherwise.</returns>
        public static bool operator !=(IntVector2 left, IntVector2 right)
        {
            return (left.X != right.X) && (left.Y != right.Y);
        }

        /// <summary>
        /// Performs a bitwise shift of the components of a vector to the left.
        /// </summary>
        /// <param name="value">The vector to shift.</param>
        /// <param name="positions">The number of positions to shift the vector by.</param>
        /// <returns>The shifted vector.</returns>
        public static IntVector2 operator <<(IntVector2 value, int positions)
        {
            return new IntVector2(value.X << positions, value.Y << positions);
        }

        /// <summary>
        /// Performs a bitwise shift of the components of a vector to the right.
        /// </summary>
        /// <param name="value">The vector to shift.</param>
        /// <param name="positions">The number of positions to shift the vector by.</param>
        /// <returns>The shifted value.</returns>
        public static IntVector2 operator >>(IntVector2 value, int positions)
        {
            return new IntVector2(value.X >> positions, value.Y >> positions);
        }

        /// <summary>
        /// Returns a number used to uniquely identify this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        /// <summary>
        /// Determines if an object equals this IntVector2.
        /// </summary>
        /// <param name="obj">The object to check for equality.</param>
        /// <returns>True if the objects are equal, false if otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is IntVector2))
            {
                return false;
            }
            else
            {
                return (IntVector2)obj == this;
            }
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            return string.Format("[X: {0}, Y: {1}]", this.X, this.Y);
        }
    }
}
