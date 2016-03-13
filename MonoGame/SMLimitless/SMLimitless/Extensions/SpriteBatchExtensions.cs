//-----------------------------------------------------------------------
// <copyright file="SpriteBatchExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Extends the SpriteBatch class.
    /// </summary>
    public static class SpriteBatchExtensions
    {
        // Credit to fbrookie for certain methods

        /// <summary>
        /// A blank, 1x1 texture, used in drawing the primitives.
        /// </summary>
        private static Texture2D blank;

        /// <summary>
        /// Set when the <see cref="Initialize"/> method is called.
        /// </summary>
        private static bool isInitialized;

        /// <summary>
        /// Draws a texture to the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw the texture.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="position">Where to draw the texture.</param>
        /// <param name="color">The color used to "shade" the texture. Use Color.White for no shading.</param>
        /// <param name="effects">How the texture is mirrored.</param>
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, SpriteEffects effects)
        {
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            spriteBatch.Draw(texture, destRect, null, color, 0.0f, Vector2.Zero, effects, 0f);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw the rectangle.</param>
        /// <param name="rectangle">The bounds to draw the rectangle within.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle)
        {
            spriteBatch.DrawRectangle(rectangle, Color.Black);
        }

        /// <summary>
        /// Draws a colored rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw the rectangle.</param>
        /// <param name="rectangle">The bounds to draw the rectangle within.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            Initialize(spriteBatch);
            spriteBatch.Draw(blank, rectangle, color);
        }

        /// <summary>
        /// Draws a colored rectangle using vectors.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw the rectangle.</param>
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
        /// <param name="spriteBatch">The SpriteBatch used to draw the rectangle.</param>
        /// <param name="x">The X position of the rectangle.</param>
        /// <param name="y">The Y position of the rectangle.</param>
        /// <param name="width">The width in pixels of the rectangle.</param>
        /// <param name="height">The height in pixels of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            spriteBatch.DrawRectangle(new Rectangle(x, y, width, height), color);
        }

        /// <summary>
        /// Draws an outline of a rectangle.
        /// </summary>
        /// <param name="batch">The SpriteBatch used to draw the rectangle outline.</param>
        /// <param name="position">The position of the rectangle outline.</param>
        /// <param name="size">The size of the rectangle outline.</param>
        /// <param name="color">The color of the rectangle outline.</param>
        public static void DrawRectangleEdges(this SpriteBatch batch, Vector2 position, Vector2 size, Color color)
        {
            // Draw four lines, one for each side.
            Tuple<Vector2, Vector2> topSide = new Tuple<Vector2, Vector2>(position, new Vector2(position.X + size.X, position.Y));
            Tuple<Vector2, Vector2> bottomSide = new Tuple<Vector2, Vector2>(new Vector2(position.X, position.Y + size.Y), position + size);
            Tuple<Vector2, Vector2> leftSide = new Tuple<Vector2, Vector2>(position, new Vector2(position.X, position.Y + size.Y));
            Tuple<Vector2, Vector2> rightSide = new Tuple<Vector2, Vector2>(new Vector2(position.X + size.X, position.Y), position + size);

            DrawLine(batch, 1f, color, topSide.Item1, topSide.Item2);
            DrawLine(batch, 1f, color, bottomSide.Item1, bottomSide.Item2);
            DrawLine(batch, 1f, color, leftSide.Item1, leftSide.Item2);
            DrawLine(batch, 1f, color, rightSide.Item1, rightSide.Item2);
        }

        /// <summary>
        /// Draws an outline of a rectangle.
        /// </summary>
        /// <param name="batch">The SpriteBatch used to draw this rectangle outline.</param>
        /// <param name="rect">The bounds of the rectangle outline.</param>
        /// <param name="color">The color of the rectangle outline.</param>
        public static void DrawRectangleEdges(this SpriteBatch batch, Rectangle rect, Color color)
        {
            DrawRectangleEdges(batch, new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), color);
        }

        /// <summary>
        /// Draws a line between two points with a custom width and color.
        /// </summary>
        /// <param name="batch">The SpriteBatch used to draw the line.</param>
        /// <param name="width">The width, in pixels, of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="point1">The point where the line starts.</param>
        /// <param name="point2">The point where the line ends.</param>
        public static void DrawLine(this SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2)
        {
            // from http://www.xnawiki.com/index.php/Drawing_2D_lines_without_using_primitives
            Initialize(batch);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Initializes the blank texture.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to initialize.</param>
        public static void Initialize(this SpriteBatch spriteBatch)
        {
            if (!isInitialized)
            {
                blank = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                blank.SetData(new Color[] { Color.White });
                isInitialized = true;
            }
        }
    }
}
