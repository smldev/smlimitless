//-----------------------------------------------------------------------
// <copyright file="RectangleExtensions.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
	/// <summary>
	///   Extends the Rectangle struct.
	/// </summary>
	public static class RectangleExtensions
	{
		/// <summary>
		///   Draws the outline of a rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to draw.</param>
		/// <param name="color">The color of the outline.</param>
		public static void DrawOutline(this Rectangle rect, Color color)
		{
			SpriteBatchExtensions.DrawRectangleEdges(GameServices.SpriteBatch, rect, color);
		}
	}
}
