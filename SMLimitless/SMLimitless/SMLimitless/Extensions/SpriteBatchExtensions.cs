using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Extensions
{
    public static class SpriteBatchExtensions
    {
        // Credit to fbrookie for certain methods
        private static Texture2D blank;
        private static bool initialized;

        private static void Initialize(this SpriteBatch spriteBatch)
        {
            if (!initialized)
            {
                blank = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                blank.SetData(new Color[] { Color.White });
                initialized = true;
            }
        }

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, SpriteEffects effects)
        {
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            spriteBatch.Draw(texture, destRect, null, color, 0.0f, Vector2.Zero, effects, 0f);
        }

        #region Rectangles
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle)
        {
            spriteBatch.DrawRectangle(rectangle, Color.Black);
        }

        /// <summary>
        /// Draws a colored rectangle.
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            Initialize(spriteBatch);
            spriteBatch.Draw(blank, rectangle, color);
        }

        /// <summary>
        /// Draws a colored rectangle using vectors.
        /// </summary>
        /// <param name="start">The top-left corner of the rectangle.</param>
        /// <param name="end">The size of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            spriteBatch.DrawRectangle(new Rectangle((int)start.X, (int)start.Y, (int)(end.X - start.X), (int)(end.Y - start.Y)), color);
        }

        /// <summary>
        /// Draws a colored rectangle using integers for positioning.
        /// </summary>
        public static void DrawRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            spriteBatch.DrawRectangle(new Rectangle(x, y, width, height), color);
        }

        #endregion
    }
}
