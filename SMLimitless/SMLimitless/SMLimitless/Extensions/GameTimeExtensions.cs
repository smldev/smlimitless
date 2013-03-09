using System;

using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
    // Credit to fbrookie
    public static class GameTimeExtensions
    {
        /// <summary>
        /// Returns the number of elapsed seconds.
        /// </summary>
        public static float GetElapsedSeconds(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Returns the number of elapsed milliseconds.
        /// </summary>
        public static float GetElapsedMilliseconds(this GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
