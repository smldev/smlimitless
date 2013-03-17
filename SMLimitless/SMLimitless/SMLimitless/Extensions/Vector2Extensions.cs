﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Checks if one or both of the components of a Vector2 are equal to Single.NaN.
        /// </summary>
        public static bool IsNaN(this Vector2 vector)
        {
            return (vector.X == Single.NaN || vector.Y == Single.NaN);
        }

        public static Rectangle ToRectangle(this Vector2 position, Vector2 size)
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public static Vector2 Floor(this Vector2 vector)
        {
            return new Vector2((float)Math.Floor(vector.X), (float)Math.Floor(vector.Y));
        }

        public static Vector2 FloorDivide(this Vector2 vector, float divisor)
        {
            return new Vector2((float)Math.Floor(vector.X / divisor), (float)Math.Floor(vector.Y / divisor));
        }

        public static Vector2 FloorDivide(this Vector2 vector, Vector2 divisor)
        {
            return new Vector2((float)Math.Floor(vector.X / divisor.X), (float)Math.Floor(vector.Y / divisor.Y));
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2((float)Math.Abs(vector.X), (float)Math.Abs(vector.Y));
        }

        public static bool GreaterThan(this Vector2 left, Vector2 right)
        {
            return (left.X > right.X) && (left.Y > right.Y);
        }

        public static bool LessThan(this Vector2 left, Vector2 right)
        {
            return (left.X < right.X) && (left.Y < right.Y);
        }

        public static bool GreaterThanOrEqualTo(this Vector2 left, Vector2 right)
        {
            return (left.X >= right.X) && (left.Y >= right.Y);
        }

        public static bool LessThanOrEqualTo(this Vector2 left, Vector2 right)
        {
            return (left.X <= right.X) && (left.Y <= right.Y);
        }
    }
}
