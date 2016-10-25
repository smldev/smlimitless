//-----------------------------------------------------------------------
// <copyright file="BoundingRectangle.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
    // Credit to fbrookie.

    /// <summary>
    /// Using Rectangle for Collision bounds causes 'jiggling' as Rectangle 
    /// must round values to integers. This struct uses float for precision.
    /// </summary>
    public struct BoundingRectangle : ICollidableShape
    {
		/// <summary>
		/// Debug; keeps track of how many times intersection methods have been called.
		/// </summary>
		public static int IntersectionCallCount = 0;

        /// <summary>
        /// The position of the top-left corner.
        /// </summary>
        private Vector2 min;

        /// <summary>
        /// The size of the rectangle.
        /// </summary>
        private Vector2 max;

		/// <summary>
		/// Gets a bounding rectangle at the origin of the world with zero width and height.
		/// </summary>
		public static BoundingRectangle Zero
		{
			get
			{
				return new BoundingRectangle(0f, 0f, 0f, 0f);
			}
		}

		/// <summary>
		/// Gets a bounding rectangle with all coordinates set to NaN.
		/// </summary>
		public static BoundingRectangle NaN
		{
			get
			{
				return new BoundingRectangle(float.NaN, float.NaN, float.NaN, float.NaN);
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> struct.
        /// </summary>
        /// <param name="rectangle">The <see cref="Rectangle"/> used to create this rectangle.</param>
        public BoundingRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> struct.
        /// </summary>
        /// <param name="start">The position of the top-left corner.</param>
        /// <param name="end">The size of the rectangle.</param>
		[DebuggerStepThrough]
        public BoundingRectangle(Vector2 start, Vector2 end)
            : this(start.X, start.Y, end.X - start.X, end.Y - start.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> struct.
        /// </summary>
        /// <param name="x">The X-coordinate of the top-left corner.</param>
        /// <param name="y">The Y-coordinate of the top-right corner.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
			min = new Vector2(x, y);
			max = new Vector2(width, height);
        }

        /// <summary>
        /// Gets or sets the X-coordinate of the left line.
        /// </summary>
        public float X
        {
            get { return min.X; }
            set { min.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the top line.
        /// </summary>
        public float Y
        {
            get { return min.Y; }
            set { min.Y = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width
        {
            get { return max.X; }
            set { max.X = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public float Height
        {
            get { return max.Y; }
            set { max.Y = value; }
        }

        /// <summary>
        /// Gets the X-coordinate of the left line.
        /// </summary>
        public float Left
        {
            get { return X; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the top line.
        /// </summary>
        public float Top
        {
            get { return Y; }
        }

        /// <summary>
        /// Gets the X-coordinate of the right line.
        /// </summary>
        public float Right
        {
            get { return X + Width; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom line.
        /// </summary>
        public float Bottom
        {
            get { return Y + Height; }
        }

        /// <summary>
        /// Gets the position of the top-center point of this rectangle.
        /// </summary>
        public Vector2 TopCenter
        {
            get
            {
                return new Vector2(Center.X, Top);
            }
        }

        /// <summary>
        /// Gets the position of the bottom-center point of this rectangle.
        /// </summary>
        public Vector2 BottomCenter
        {
            get
            {
                return new Vector2(Center.X, Bottom);
            }
        }

        /// <summary>
        /// Gets the position of the center point on the left edge.
        /// </summary>
        public Vector2 LeftCenter
        {
            get
            {
                return new Vector2(Left, Center.Y);
            }
        }

        /// <summary>
        /// Gets the position of the center point on the right edge.
        /// </summary>
        public Vector2 RightCenter
        {
            get
            {
                return new Vector2(Right, Center.Y);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector2 Position
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public Vector2 Size
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// Gets the position of the point in the center.
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(X + (Width / 2f), Y + (Height / 2f)); }
        }

        /// <summary>
        /// Gets the shape of this collidable object.
        /// </summary>
        public CollidableShape Shape
        {
            get
            {
                return CollidableShape.Rectangle;
            }
        }

        /// <summary>
        /// Gets this rectangle.
        /// </summary>
        /// <remarks>This member is required for the ICollidableShape interface.</remarks>
        public BoundingRectangle Bounds
        {
            get
            {
                return this;
            }
        }

		/// <summary>
		/// Gets the position of the bottom-right point on this rectangle.
		/// </summary>
		public Vector2 BottomRight
		{
			get
			{
				return new Vector2(this.Right, this.Bottom);
			}
		}

		/// <summary>
		/// Gets the position of the top-right point on this rectangle.
		/// </summary>
		public Vector2 TopRight => new Vector2(Right, Top);

		/// <summary>
		/// Gets the position of the bottom-left point on this rectangle.
		/// </summary>
		public Vector2 BottomLeft => new Vector2(Left, Bottom);

		/// <summary>
		/// Gets the position of the top-left point on this rectangle.
		/// </summary>
		public Vector2 TopLeft => new Vector2(Left, Top);

		/// <summary>
		/// Returns a rectangle created from a simple string representation.
		/// </summary>
		/// <param name="input">A string containing four comma-delimited numbers.</param>
		/// <returns>A rectangle created from the string.</returns>
		public static BoundingRectangle FromSimpleString(string input)
        {
            string[] components = input.Split(',');

            if (components.Length != 4)
            {
                throw new ArgumentException(string.Format("BoundingRectangle.Deserialize(string): The input was not in a valid form. Input: {0}", input));
            }

            for (int i = 1; i < 4; i++)
            {
                components[i] = components[i].TrimStart();
            }

            float x, y, width, height;

            if (!float.TryParse(components[0], out x)) { throw new ArgumentException(string.Format("BoundingRectangle.Deserialize(string): Invalid value for X component. Input is {0}.", input)); }
            if (!float.TryParse(components[1], out y)) { throw new ArgumentException(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Y component. Input is {0}.", input)); }
            if (!float.TryParse(components[2], out width)) { throw new ArgumentException(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Width component. Input is {0}.", input)); }
            if (!float.TryParse(components[3], out height)) { throw new ArgumentException(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Height component. Input is {0}.", input)); }

            return new BoundingRectangle(x, y, width, height);
        }

        /// <summary>
        /// Returns a string representing the components of this rectangle.
        /// </summary>
        /// <returns>A string in the format "X, Y, Width, Height".</returns>
        public string Serialize()
        {
            return string.Format("{0}, {1}, {2}, {3}", X, Y, Width, Height);
        }

        /// <summary>
        /// Checks if any component of this rectangle is Not a Number (NaN).
        /// </summary>
        /// <returns>True if any component is equal to float.NaN, false if otherwise.</returns>
        public bool IsNaN()
        {
            return min.IsNaN() || max.IsNaN();
        }

        /// <summary>
        /// Determines if a given point is within this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is within this rectangle, false if otherwise.</returns>
        public bool Intersects(Vector2 point)
        {
            return (point.X > Left && point.X < Right) && (point.Y > Top && point.Y < Bottom);
        }

        /// <summary>
        /// Determines if a given point is within or tangent to this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is within or tangent to this rectangle, false if otherwise.</returns>
        public bool IntersectsIncludingEdges(Vector2 point)
        {
            return (point.X >= Left && point.X <= Right) && (point.Y >= Top && point.Y <= Bottom);
        }

        /// <summary>
        /// Determines if a given rectangle is intersecting this rectangle.
        /// </summary>
        /// <param name="that">The rectangle to check.</param>
        /// <returns>True if any part of the other rectangle is intersecting this rectangle, false if otherwise.</returns>
        public bool Intersects(BoundingRectangle that)
        {
            if (that.Right <= Left || that.Left >= Right)
            {
                return false;
            }
            else if (that.Bottom <= Top || that.Top >= Bottom)
            {
                return false;
            }

            return true;
        }

		/// <summary>
		/// Returns a value indicating whether another rectangle intersects or is tangent to this one.
		/// </summary>
		/// <param name="that">The other rectangle.</param>
		/// <returns>True if the other rectangle intersects or is tangent to this one, false if otherwise.</returns>
		public bool IntersectsIncludingEdges(BoundingRectangle that)
		{
			if (that.Right < Left || that.Left > Right)
			{
				return false;
			}
			else if (that.Bottom < Top || that.Top > Bottom)
			{
				return false;
			}

			return true;
		}

        /// <summary>
        /// Determines if a given right triangle is intersecting this rectangle.
        /// </summary>
        /// <param name="triangle">The right triangle to check.</param>
        /// <returns>True if any part of the right triangle is intersecting this rectangle, false if otherwise.</returns>
        public bool Intersects(RightTriangle triangle)
        {
            Vector2 pointOnSlope = triangle.GetPointOnSlope(Center.X);
            Vector2 rectCenterPoint = (triangle.SlopedSides == RtSlopedSides.TopLeft || triangle.SlopedSides == RtSlopedSides.TopRight) ? BottomCenter : TopCenter;

            if (!Intersects(triangle.Bounds))
            {
                return false;
            }

            if (pointOnSlope.IsNaN())
            {
                if (triangle.HorizontalSlopedSide == HorizontalDirection.Left)
                {
                    return rectCenterPoint.X >= triangle.Bounds.Right;
                }
                else
                {
                    return rectCenterPoint.X <= triangle.Bounds.Left;
                }
            }
            else
            {
                if (triangle.VerticalSlopedSide == VerticalDirection.Up)
                {
                    return rectCenterPoint.Y > pointOnSlope.Y;
                }
                else
                {
                    return rectCenterPoint.Y < pointOnSlope.Y;
                }
            }
        }

        /// <summary>
        /// Determines if a point is within this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="adjacentPointsAreWithin">If true, any point on the edge of the shape will be considered within.</param>
        /// <returns>True if the point is within the shape, false if otherwise.</returns>
        public bool Within(Vector2 point, bool adjacentPointsAreWithin)
        {
            if (adjacentPointsAreWithin)
            {
                return IntersectsIncludingEdges(point);
            }
            else
            {
                return Intersects(point);
            }
        }

		/// <summary>
		/// Returns a value indicating whether a bounding rectangle is entirely contained within this rectangle.
		/// </summary>
		/// <param name="that">The other rectangle.</param>
		/// <returns>True if the other rectangle is entirely contained within this one, false if otherwise.</returns>
		public bool Within(BoundingRectangle that)
		{
			return (that.Left >= Left) && (that.Right <= Right) && (that.Top >= Top) && (that.Bottom <= Bottom);
		}

		/// <summary>
		/// Returns a value indicating whether a point is above, below, to the left of, et cetera, of this rectangle.
		/// </summary>
		/// <param name="point">The point to check.</param>
		/// <returns>A <see cref="RectangularSpaceDivision"/> instance indicating where the point is with relation to this rectangle.</returns>
		public RectangularSpaceDivision GetPointRelation(Vector2 point)
		{
			if (IntersectsIncludingEdges(point)) { return RectangularSpaceDivision.Within; }
			bool above = point.Y < Top;
			bool below = point.Y > Bottom;
			bool left = point.X < Left;
			bool right = point.X > Right;

			if (above && (!left && !right)) { return RectangularSpaceDivision.Above; }
			else if (above && left && !right) { return RectangularSpaceDivision.AboveLeft; }
			else if (above && !left && right) { return RectangularSpaceDivision.AboveRight; }
			else if (left && (!above && !below)) { return RectangularSpaceDivision.Left; }
			else if (right && (!above && !below)) { return RectangularSpaceDivision.Right; }
			else if (below && (!left && !right)) { return RectangularSpaceDivision.Below; }
			else if (below && (left && !right)) { return RectangularSpaceDivision.BelowLeft; }
			else if (below && (!left && right)) { return RectangularSpaceDivision.BelowRight; }
			else { throw new InvalidOperationException(); }
		}

        /// <summary>
        /// Returns the intersection depth between a given rectangle and this one.
        /// </summary>
        /// <param name="that">The rectangle to check.</param>
        /// <returns>The intersection depth between the other rectangle and this one. If the rectangles aren't intersecting, a vector with NaN components is returned.</returns>
        /// <remarks>
        /// This method determines the sign of either component of the result
        /// by determining the "direction" of the intersection - for example,
        /// if the right edge of the other rectangle is to the left of the center
        /// of this rectangle, the "direction" is to the left, and since left is
        /// negative X, the resulting X component will be negative.
        /// </remarks>
        public Vector2 GetIntersectionDepth(BoundingRectangle that)
        {
			IntersectionCallCount++;
			Vector2 result = new Vector2(float.NaN);
			
			Vector2 thisCenter = Center;
			Vector2 thatCenter = that.Center;

			Vector2 thisHalfSize = Size / 2f;
			Vector2 thatHalfSize = that.Size / 2f;
			Vector2 combinedSizes = thisHalfSize + thatHalfSize;

			// X-axis checks
			float xDistanceBetweenCenters = thatCenter.X - thisCenter.X;
			float absXDistance = Math.Abs(xDistanceBetweenCenters);
			float signXDistance = (xDistanceBetweenCenters >= 0f) ? 1f : -1f;
			if (absXDistance < combinedSizes.X)
			{
				result.X = signXDistance * (combinedSizes.X - Math.Abs(xDistanceBetweenCenters));
			}

			// Y-axis checks
			float yDistanceBetweenCenters = thatCenter.Y - thisCenter.Y;
			float absYDistance = Math.Abs(yDistanceBetweenCenters);
			float signYDistance = (yDistanceBetweenCenters >= 0f) ? 1f : -1f;
			if (absYDistance < combinedSizes.Y)
			{
				result.Y = signYDistance * (combinedSizes.Y - Math.Abs(yDistanceBetweenCenters));
			}

			return result;

			// WYLO: We're returning wrong results here, or at least results that we didn't get before.
			// If a sprite and tile have the same X position and same width, this method returns 0 for X,
			// where the old one returned 16. Maybe we can apply the sign(a)*(b-|a|) to Right and Left instead?
			// Do some math and find out.
        }

        /// <summary>
        /// Returns the distance to move a given rectangle
        /// in order to properly resolve a collision.
        /// </summary>
        /// <param name="that">The rectangle to check.</param>
        /// <returns>A value that can be applied to the position of the rectangle
        /// to move it the minimum distance such that it won't be intersecting this one.</returns>
        /// <remarks>Internally, this method uses the GetIntersectionDepth() method to determine
        /// the intersection depth. It then determines which of the two axes has the smallest absolute
        /// value and zeroes out the other. This way of collision resolution is called the "shallowest edge"
        /// method, and it allows quick determination of which axis to resolve along.</remarks>
        public Vector2 GetCollisionResolution(BoundingRectangle that)
        {
            Vector2 intersection = GetIntersectionDepth(that);

			return GetCollisionResolution(intersection);
        }

		/// <summary>
		/// Returns the distance to move a given rectangle
		/// in order to properly resolve a collision.
		/// </summary>
		/// <param name="intersect">The intersection depth for the other rectangle.</param>
		/// <returns>A value that can be applied to the position of the rectangle
		/// to move it the minimum distance such that it won't be intersecting this one.</returns>
		public Vector2 GetCollisionResolution(Vector2 intersect)
		{
			if (intersect.IsNaN())
			{
				return new Vector2(float.NaN, float.NaN);
			}
			else if (Math.Abs(intersect.X) < Math.Abs(intersect.Y))
			{
				return new Vector2(intersect.X, 0f);
			}
			else
			{
				return new Vector2(0f, intersect.Y);
			}
		}

        /// <summary>
        /// Gets the Y-coordinate of the top of this rectangle.
        /// </summary>
        /// <param name="x">The x-coordinate to check for.</param>
        /// <returns>The Y-coordinate of the top of this rectangle.</returns>
        public Vector2 GetTopPoint(float x)
        {
            return new Vector2(x, Top);
        }

        /// <summary>
        /// Returns a standard Rectangle that uses integral components.
        /// </summary>
        /// <returns>The standard Rectangle.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)min.X, (int)min.Y, (int)max.X, (int)max.Y);
        }

		/// <summary>
		/// Gets a position to place another rectangle at such that both this
		/// and that's Center lines are equal.
		/// </summary>
		/// <param name="that">The rectangle to align.</param>
		/// <returns>A position to move the rectangle to.</returns>
		public Vector2 GetCenterAlignedInColumnPosition(BoundingRectangle that)
		{
			float halfWidth = that.Width / 2f;
			return new Vector2(Center.X - halfWidth, that.Y);
		}

        /// <summary>
        /// Draws this rectangle to the screen.
        /// </summary>
        /// <param name="color">The color of the rectangle.</param>
        public void Draw(Color color)
        {
            GameServices.SpriteBatch.DrawRectangle(ToRectangle(), color);
        }

        /// <summary>
        /// Draws the outline of this rectangle to the screen.
        /// </summary>
        /// <param name="color">The color of the outline.</param>
        public void DrawOutline(Color color)
        {
            GameServices.SpriteBatch.DrawRectangleEdges(ToRectangle(), color);
        }

        /// <summary>
        /// Returns a string representing key values of this rectangle.
        /// </summary>
        /// <returns>A string representing key values of this rectangle.</returns>
        public override string ToString()
        {
            return "{X:" + X.ToString() +
                   " Y:" + Y.ToString() +
                   " Width:" + Width.ToString() +
                   " Height:" + Height.ToString() + "}";
        }       
    }
}
