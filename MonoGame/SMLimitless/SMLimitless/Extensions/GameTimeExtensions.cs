//-----------------------------------------------------------------------
// <copyright file="GameTimeExtensions.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
    // Credit to fbrookie

    /// <summary>
    /// Contains extension methods for the GameTime class.
    /// </summary>
    public static class GameTimeExtensions
    {
        /// <summary>
        /// Returns the number of elapsed seconds since the last update.
        /// </summary>
        /// <param name="gameTime">The GameTime to use.</param>
        /// <returns>The number of elapsed seconds since the last update.</returns>
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Returns the number of elapsed milliseconds since the last update.
        /// </summary>
        /// <param name="gameTime">The GameTime to use.</param>
        /// <returns>The number of elapsed milliseconds since the last update.</returns>
        public static float GetElapsedMilliseconds(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
