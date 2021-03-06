﻿//-----------------------------------------------------------------------
// <copyright file="RightTriangle.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
	/// <summary>
	///   Represents a right triangle, used for sloped tiles.
	/// </summary>
	[StructLayout(LayoutKind.Auto)]
	public struct RightTriangle : ICollidableShape
	{
		/// <summary>
		///   Gets or sets the rectangle that completely contains the triangle.
		/// </summary>
		public BoundingRectangle Bounds { get; set; }

		/// <summary>
		///   Gets or sets a value indicating which sides of the triangle are sloped.
		/// </summary>
		public RtSlopedSides SlopedSides { get; set; }

		/// <summary>
		///   Gets the slope (rise over run) of the triangle.
		/// </summary>
		public float Slope
		{
			get
			{
				if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.BottomRight)
				{
					// With these two triangles (top-left and bottom-right), the
					// slope goes up as you move from left to right, thus a
					// positive slope.
					return Bounds.Height / Bounds.Width;
				}
				else
				{
					// With the other two triangles (top-right and bottom-left),
					// the slope goes down as you move from left to right thus a
					// negative slope.
					return -Bounds.Height / Bounds.Width;
				}
			}
		}

		/// <summary>
		///   Gets the location of the point of the 90-degree angle.
		/// </summary>
		public Vector2 Point90
		{
			get
			{
				switch (SlopedSides)
				{
					case RtSlopedSides.TopLeft:
						// For a top-left triangle, the point is at the
						// bottom-right corner.
						return new Vector2(Bounds.Right, Bounds.Bottom);
					case RtSlopedSides.TopRight:
						// For a top-right triangle, the point is at the
						// bottom-left corner.
						return new Vector2(Bounds.Left, Bounds.Bottom);
					case RtSlopedSides.BottomLeft:
						// For a bottom-left triangle, the point is at the
						// top-right corner.
						return new Vector2(Bounds.Right, Bounds.Top);
					case RtSlopedSides.BottomRight:
						// For a bottom-right triangle, the point is at the
						// top-left corner.
						return new Vector2(Bounds.Left, Bounds.Top);
					default:
						// This can't happen, but the compiler complains.
						return new Vector2(float.NaN, float.NaN);
				}
			}
		}

		/// <summary>
		///   Gets the location of the point at the bottom of the slope.
		/// </summary>
		public Vector2 Point1
		{
			get
			{
				if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.BottomRight)
				{
					// Bottom-left corner.
					return new Vector2(Bounds.Left, Bounds.Bottom);
				}
				else
				{
					// Bottom-right corner.
					return new Vector2(Bounds.Right, Bounds.Bottom);
				}
			}
		}

		/// <summary>
		///   Gets the location of the point at the top of the slope.
		/// </summary>
		public Vector2 Point2
		{
			get
			{
				if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.BottomRight)
				{
					// Top-right corner.
					return new Vector2(Bounds.Right, Bounds.Top);
				}
				else
				{
					// Top-left corner.
					return new Vector2(Bounds.Left, Bounds.Top);
				}
			}
		}

		/// <summary>
		///   Gets the point on the Y-axis where the line coinciding with the
		///   slope intersects with the Y-axis. Equivalent to the variable b in
		///   the linear equation y = mx + b.
		/// </summary>
		/// <remarks>
		///   To account for the fact that, in XNA, the Y-axis is flipped
		///   (positive Y goes down), the equation is b = mx + y.
		/// </remarks>
		public float YIntersect
		{
			get
			{
				// Ordinarily, the y-intersect is equal to (Slope * x) - y, but
				// since the Y-axis is flipped in XNA, it's equal to (Slope * x)
				// + y.
				return (Slope * Point1.X) + Point1.Y;
			}
		}

		/// <summary>
		///   Gets the shape of this collidable object.
		/// </summary>
		public CollidableShape Shape
		{
			get
			{
				return CollidableShape.RightTriangle;
			}
		}

		/// <summary>
		///   Gets the horizontal side (left or right) that has the sloped edge.
		/// </summary>
		public HorizontalDirection HorizontalSlopedSide
		{
			get
			{
				if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.BottomLeft)
				{
					return HorizontalDirection.Left;
				}
				else
				{
					return HorizontalDirection.Right;
				}
			}
		}

		/// <summary>
		///   Gets the vertical side (up or down) that has the sloped edge.
		/// </summary>
		public VerticalDirection VerticalSlopedSide
		{
			get
			{
				if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
				{
					return VerticalDirection.Up;
				}
				else
				{
					return VerticalDirection.Down;
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="RightTriangle" /> class.
		/// </summary>
		/// <param name="bounds">
		///   The rectangle that forms the bounds of the triangle.
		/// </param>
		/// <param name="slopedSides">Which sides of the triangle are sloped.</param>
		public RightTriangle(BoundingRectangle bounds, RtSlopedSides slopedSides)
		{
			Bounds = bounds;
			SlopedSides = slopedSides;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="RightTriangle" /> class.
		/// </summary>
		/// <param name="x">
		///   The horizontal distance from the left edge of the game space.
		/// </param>
		/// <param name="y">
		///   The vertical distance from the top edge of the game space.
		/// </param>
		/// <param name="width">The width of the triangle.</param>
		/// <param name="height">The height of the triangle.</param>
		/// <param name="slopedSides">
		///   Which sides of the triangle are replaced by the slope.
		/// </param>
		public RightTriangle(float x, float y, float width, float height, RtSlopedSides slopedSides)
			: this(new BoundingRectangle(x, y, width, height), slopedSides)
		{
		}

		/// <summary>
		///   Gets a point directly on the slope of the triangle. Restricted to
		///   the slope itself. To get any point on the line coincident to this
		///   slope, call GetPointOnLine().
		/// </summary>
		/// <param name="x">The X-coordinate to solve for.</param>
		/// <returns>
		///   The point directly on the slope, or NaN if the point is not on the slope.
		/// </returns>
		public Vector2 GetPointOnSlope(float x)
		{
			// Y = mx + b.
			if (x <= Bounds.Left || x >= Bounds.Right)
			{
				return new Vector2(float.NaN);
			}

			// Ordinarily, y = mx + b. But since the Y-axis is reversed, y = mx -
			// b. But we still need a positive value, so we return negative Y.
			float y = (Slope * x) - YIntersect;
			return new Vector2(x, (-y));
		}

		/// <summary>
		///   For a given vertical line at an X coordinate, returns the point of
		///   intersection between this line and the slope, clamped to the left
		///   and right edges of the triangle.
		/// </summary>
		/// <param name="x">The X coordinate of the vertical line.</param>
		/// <returns>A value between the left and right edges of the triangle.</returns>
		public Vector2 GetClampedPointOnSlope(float x)
		{
			x = MathHelper.Clamp(x, Bounds.Left, Bounds.Right);
			return GetPointOnSlope(x);
		}

		/// <summary>
		///   Gets a point directly on the line that is coincident to the slope
		///   of the triangle. To get points only on the slope itself, call GetPointOnSlope().
		/// </summary>
		/// <param name="x">The X-coordinate to solve for.</param>
		/// <returns>A point directly on the line.</returns>
		public Vector2 GetPointOnLine(float x)
		{
			float y = (Slope * x) - YIntersect;
			return new Vector2(x, -y);
		}

		/// <summary>
		///   Determines if a given rectangle intersects this triangle.
		/// </summary>
		/// <param name="rect">The rectangle to check for intersection.</param>
		/// <returns>
		///   True if any part of the rectangle intersects this right triangle,
		///   false if otherwise.
		/// </returns>
		public bool Intersects(BoundingRectangle rect)
		{
			return rect.Intersects(this);
		}

		/// <summary>
		///   Determines if a point is within this triangle.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <param name="adjacentPointsAreWithin">
		///   If true, any point on the edge of the shape will be considered within.
		/// </param>
		/// <returns>True if the point is within the shape, false if otherwise.</returns>
		public bool Within(Vector2 point, bool adjacentPointsAreWithin)
		{
			if (!Bounds.Within(point, adjacentPointsAreWithin))
			{
				return false;
			}

			float pointOnSlopeY = GetClampedPointOnSlope(point.X).Y;

			if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
			{
				if (adjacentPointsAreWithin)
				{
					if (pointOnSlopeY > point.Y)
					{
						return false;
					}
				}
				else
				{
					if (pointOnSlopeY >= point.Y)
					{
						return false;
					}
				}
			}
			else if (SlopedSides == RtSlopedSides.BottomLeft || SlopedSides == RtSlopedSides.BottomRight)
			{
				if (adjacentPointsAreWithin)
				{
					if (pointOnSlopeY < point.Y)
					{
						return false;
					}
				}
				else
				{
					if (pointOnSlopeY <= point.Y)
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool IsSlopeCollision(BoundingRectangle rect, Vector2 intersect)
		{
			if (Math.Abs(intersect.X) < Math.Abs(intersect.Y))
			{
				if (VerticalSlopedSide == VerticalDirection.Up)
				{
					return intersect.Y < 0f;
				}
				else
				{
					return intersect.Y > 0f;
				}
			}
			else
			{
				if (HorizontalSlopedSide == HorizontalDirection.Left)
				{
					return intersect.X < 0f;
				}
				else if (HorizontalSlopedSide == HorizontalDirection.Right)
				{
					return intersect.X > 0f;
				}
			}
			return false;
		}

		/// <summary>
		///   Gets the minimum distance to move a given rectangle such that it
		///   will no longer be intersecting this right triangle.
		/// </summary>
		/// <param name="rect">The rectangle to resolve.</param>
		/// <returns>
		///   The minimum distance to move the rectangle, which can be directly
		///   applied to the rectangle's position.
		/// </returns>
		/// <remarks>
		///   This method resolves collision by first determining if the
		///   bottom-center point (for TopLeft/TopRight triangles) or the
		///   top-center point (for BottomLeft/BottomRight
		///   triangles) is within the bounds. If it is, the method treats it as
		///              a slope collision by checking if the point is between
		///   the slope and the top. If it isn't, the collision is treated as a
		///   collision between two rectangles.
		/// </remarks>
		public Vector2 GetCollisionResolution(BoundingRectangle rect)
		{
			// Vector2 rectCollisionPoint = (this.SlopedSides ==
			// RtSlopedSides.TopLeft || this.SlopedSides ==
			// RtSlopedSides.TopRight) ? rect.BottomCenter : rect.TopCenter;
			Vector2 rectCollisionPoint = Vector2.Zero;
			if (SlopedSides == RtSlopedSides.TopLeft) { rectCollisionPoint = rect.BottomRight; }
			else if (SlopedSides == RtSlopedSides.TopRight) { rectCollisionPoint = rect.BottomLeft; }
			else if (SlopedSides == RtSlopedSides.BottomLeft) { rectCollisionPoint = rect.TopRight; }
			else if (SlopedSides == RtSlopedSides.BottomRight) { rectCollisionPoint = rect.TopLeft; }

			if (!Bounds.IntersectsIncludingEdges(rectCollisionPoint))
			{
				return Bounds.GetCollisionResolution(rect);
			}
			else
			{
				return ResolveSlopeCollision(rect, Bounds.GetCollisionResolution(rect));
			}
		}

		/// <summary>
		///   Returns the depth of the intersection between this triangle and a
		///   given rectangle.
		/// </summary>
		/// <param name="that">The given rectangle.</param>
		/// <returns>A vector representing the collision depth.</returns>
		public Vector2 GetIntersectionDepth(BoundingRectangle that)
		{
			return GetCollisionResolution(that);
		}

		/// <summary>
		///   Gets the position of the top of this triangle, given an X-coordinate.
		/// </summary>
		/// <param name="x">The X-coordinate to check.</param>
		/// <returns>The position of the top of this triangle.</returns>
		public Vector2 GetTopPoint(float x)
		{
			if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
			{
				return GetPointOnSlope(x);
			}
			return new Vector2(x, Bounds.Top);
		}

		/// <summary>
		///   Returns a value indicating whether a given point rests above or
		///   below the slope line.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>
		///   -1 if the point is above the slope line, 1 if the point is below
		///    the slope line, 0 if otherwise.
		/// </returns>
		public int AboveOrBelowSlopeLine(Vector2 point)
		{
			float slopeIntersect = GetPointOnLine(point.X).Y;
			if (slopeIntersect < point.Y) { return -1; }
			else if (slopeIntersect > point.Y) { return 1; }
			else { return 0; }
		}

		/// <summary>
		///   Resolves a collision between a rectangle and the sloped side of
		///   this triangle.
		/// </summary>
		/// <param name="that">The rectangle to resolve.</param>
		/// <param name="intersection">
		///   The intersection between the rectangle and the bounds of this triangle.
		/// </param>
		/// <returns>
		///   The distance to move the rectangle by to resolve this collision.
		/// </returns>
		private Vector2 ResolveSlopeCollision(BoundingRectangle that, Vector2 intersection)
		{
			Vector2 topCenter = (HorizontalSlopedSide == HorizontalDirection.Left) ? that.TopRight : that.TopLeft;
			Vector2 bottomCenter = (HorizontalSlopedSide == HorizontalDirection.Left) ? that.BottomRight : that.BottomLeft;
			Vector2 pointOnSlope = GetPointOnSlope((HorizontalSlopedSide == HorizontalDirection.Left) ? that.Right : that.Left);

			if (pointOnSlope == Vector2.Zero)
			{
				return Vector2.Zero;
			}

			if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
			{
				if (bottomCenter.Y > pointOnSlope.Y)
				{
					// The bottom-center point is below the slope, resolution needed
					return new Vector2(0f, -(bottomCenter.Y - pointOnSlope.Y));
				}
			}
			else if (SlopedSides == RtSlopedSides.BottomLeft || SlopedSides == RtSlopedSides.BottomRight)
			{
				if (topCenter.Y < pointOnSlope.Y)
				{
					// The top-center point is above the slope, resolution needed
					return new Vector2(0f, pointOnSlope.Y - topCenter.Y);
				}
			}

			return Vector2.Zero;
		}

		/// <summary>
		///   Draws this triangle to the screen.
		/// </summary>
		/// <param name="debug">Draws some useful debug information.</param>
		public void Draw(bool debug)
		{
			GameServices.SpriteBatch.DrawLine(1f, Color.White, Point90, Point1);
			GameServices.SpriteBatch.DrawLine(1f, Color.White, Point90, Point2);
			GameServices.SpriteBatch.DrawLine(1f, Color.White, Point1, Point2);

			if (debug)
			{
				// Draw the line coincident to the slope. For the inverted
				// Y-axis, the slope-intercept formula, solved for x, is x = (-y
				// + b) / m
				Vector2 screenSize = GameServices.ScreenSize;
				Vector2 lineStart = new Vector2((-screenSize.Y + YIntersect) / Slope, screenSize.Y);
				Vector2 lineEnd = new Vector2(YIntersect / Slope, 0f);
				GameServices.SpriteBatch.DrawLine(1f, Color.ForestGreen, lineStart, lineEnd);

				// DrawLine is a little weird with drawing the right-triangle -
				// the bounds look visually larger than the triangle, even though
				// they are the right sizes. So we'll correct the bounds so they
				// look right.
				Rectangle drawBounds = new Rectangle((int)Bounds.X + 1, (int)Bounds.Y - 1, (int)Bounds.Width, (int)Bounds.Height);
				drawBounds.DrawOutline(Color.Red);

				// Draw some useful debug information.
				GameServices.DebugFont.DrawString(ToString(), new Vector2(Bounds.X, Bounds.Y));
			}
		}

		/// <summary>
		///   Returns a string representation of the important components of the triangle.
		/// </summary>
		/// <returns>A string representation.</returns>
		public override string ToString()
		{
			string boundsString = string.Format("Bounds: X:{0}, Y:{1}, Width:{2}, Height:{3}{4}", Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Environment.NewLine);
			string point90String = string.Format("Point 90: X:{0}, Y:{1}{2}", Point90.X, Point90.Y, Environment.NewLine);
			string point1String = string.Format("Point 1: X:{0}, Y:{1}{2}", Point1.X, Point1.Y, Environment.NewLine);
			string point2String = string.Format("Point 2: X:{0}, Y:{1}{2}", Point2.X, Point2.Y, Environment.NewLine);
			string slopeString = string.Format("Slope: {0}, Y-Intersect: {1}, Sloped Sides:{2}{3}", Slope, YIntersect, SlopedSides.ToString(), Environment.NewLine);
			return string.Concat(boundsString, point90String, point1String, point2String, slopeString);
		}
	}
}
