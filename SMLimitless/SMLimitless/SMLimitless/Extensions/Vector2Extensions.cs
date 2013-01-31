using System;
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
    }
}
