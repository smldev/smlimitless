//-----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Screens;
using SMLimitless.Sprites;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Contains extension methods for the Type class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if one given type inherits from another.
        /// </summary>
        /// <param name="a">The presumed derived type.</param>
        /// <param name="b">The presumed base type.</param>
        /// <returns>True if the derived type inherits from the base type.</returns>
        public static bool InheritsFrom(this Type a, Type b)
        {
            // Credit to http://stackoverflow.com/a/18375526/2709212
            if (a == null)
            {
                return false;
            }

            if (b.BaseType == null)
            {
                return a.IsInterface;
            }

            if (b.IsInterface)
            {
                return a.GetInterfaces().Contains(b);
            }

            var currentType = a;
            while (currentType != null)
            {
                if (currentType.BaseType == b)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Determines if a given type is a sprite.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a sprite.</returns>
        public static bool IsSprite(this Type type)
        {
            return type.InheritsFrom(typeof(Sprite));
        }

        /// <summary>
        /// Determines if a given type is a tile.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a tile.</returns>
        public static bool IsTile(this Type type)
        {
            return type.InheritsFrom(typeof(Tile));
        }

        /// <summary>
        /// Determines if a given type is a screen.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is a screen.</returns>
        public static bool IsScreen(this Type type)
        {
            return type.InheritsFrom(typeof(Screen));
        }

		/// <summary>
		/// Determines if a given type is static.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the type is static, false if it is not.</returns>
		public static bool IsStatic(this Type type)
		{
			return type.IsClass && type.IsAbstract && type.IsSealed;
		}
    }
}
