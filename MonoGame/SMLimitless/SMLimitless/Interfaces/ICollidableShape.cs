//-----------------------------------------------------------------------
// <copyright file="ICollidableShape.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using SMLimitless.Physics;

namespace SMLimitless.Interfaces
{
	/// <summary>
	///   Represents a shape that rectangles can collide with.
	/// </summary>
	public interface ICollidableShape
	{
		/// <summary>
		///   Gets a rectangle which fully contains this collidable shape.
		/// </summary>
		BoundingRectangle Bounds { get; }

		/// <summary>
		///   Gets the distance to move a given rectangle by so that it won't be
		///   colliding with this shape.
		/// </summary>
		/// <param name="that">The rectangle to resolve.</param>
		/// <returns>The distance to move the rectangle by.</returns>
		Vector2 GetCollisionResolution(BoundingRectangle that);

		/// <summary>
		///   Gets the depth of an intersection between this shape and a rectangle.
		/// </summary>
		/// <param name="that">The rectangle to check for.</param>
		/// <returns>A vector representing the depth.</returns>
		Vector2 GetIntersectionDepth(BoundingRectangle that);

		/// <summary>
		///   Gets the location of the top of the shape, given an X-coordinate.
		/// </summary>
		/// <param name="x">
		///   The X-coordinate to get the top of the shape for.
		/// </param>
		/// <returns>
		///   A vector with the X position set to the X parameter, and the Y
		///   position set to the position of the top of the shape.
		/// </returns>
		Vector2 GetTopPoint(float x);

		/// <summary>
		///   Determines if a rectangle intersects this shape.
		/// </summary>
		/// <param name="that">The rectangle to check for intersection.</param>
		/// <returns>
		///   True if the rectangle intersects this shape, false if it doesn't.
		/// </returns>
		/// <remarks>
		///   This method returns False if the rectangle is directly against this
		///   shape but not within it.
		/// </remarks>
		bool Intersects(BoundingRectangle that);

		/// <summary>
		///   Determines if a point is within this shape.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="adjacentPointsAreWithin">
		///   If true, any point on the edge of the shape will be considered within.
		/// </param>
		/// <returns>True if the point is within the shape, false if otherwise.</returns>
		bool Within(Vector2 point, bool adjacentPointsAreWithin);
	}
}
