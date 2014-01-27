//-----------------------------------------------------------------------
// <copyright file="BoundingRectangle.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
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
    public class BoundingRectangle : ICollidableShape
    {
        /// <summary>
        /// The position of the top-left corner.
        /// </summary>
        private Vector2 min;

        /// <summary>
        /// The size of the rectangle.
        /// </summary>
        private Vector2 max;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> class.
        /// </summary>
        public BoundingRectangle()
            : this(0, 0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> class.
        /// </summary>
        /// <param name="rectangle">The <see cref="Rectangle"/> used to create this rectangle.</param>
        public BoundingRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> class.
        /// </summary>
        /// <param name="start">The position of the top-left corner.</param>
        /// <param name="end">The size of the rectangle.</param>
        public BoundingRectangle(Vector2 start, Vector2 end)
            : this(start.X, start.Y, end.X - start.X, end.Y - start.Y)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle"/> class.
        /// </summary>
        /// <param name="x">The X-coordinate of the top-left corner.</param>
        /// <param name="y">The Y-coordinate of the top-right corner.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            this.min = new Vector2(x, y);
            this.max = new Vector2(width, height);
        }

        /// <summary>
        /// Gets or sets the X-coordinate of the left line.
        /// </summary>
        public float X
        {
            get { return this.min.X; }
            set { this.min.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y-coordinate of the top line.
        /// </summary>
        public float Y
        {
            get { return this.min.Y; }
            set { this.min.Y = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width
        {
            get { return this.max.X; }
            set { this.max.X = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public float Height
        {
            get { return this.max.Y; }
            set { this.max.Y = value; }
        }

        /// <summary>
        /// Gets the X-coordinate of the left line.
        /// </summary>
        public float Left
        {
            get { return this.X; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the top line.
        /// </summary>
        public float Top
        {
            get { return this.Y; }
        }

        /// <summary>
        /// Gets the X-coordinate of the right line.
        /// </summary>
        public float Right
        {
            get { return this.X + this.Width; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom line.
        /// </summary>
        public float Bottom
        {
            get { return this.Y + this.Height; }
        }

        /// <summary>
        /// Gets the position of the top-center point of this rectangle.
        /// </summary>
        public Vector2 TopCenter
        {
            get
            {
                return new Vector2(this.Center.X, this.Top);
            }
        }

        /// <summary>
        /// Gets the position of the bottom-center point of this rectangle.
        /// </summary>
        public Vector2 BottomCenter
        {
            get
            {
                return new Vector2(this.Center.X, this.Bottom);
            }
        }

        /// <summary>
        /// Gets the position of the center point on the left edge.
        /// </summary>
        public Vector2 LeftCenter
        {
            get
            {
                return new Vector2(this.Left, this.Center.Y);
            }
        }

        /// <summary>
        /// Gets the position of the center point on the right edge.
        /// </summary>
        public Vector2 RightCenter
        {
            get
            {
                return new Vector2(this.Right, this.Center.Y);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector2 Position
        {
            get { return this.min; }
            set { this.min = value; }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public Vector2 Size
        {
            get { return this.max; }
            set { this.max = value; }
        }

        /// <summary>
        /// Gets the position of the point in the center.
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(this.X + (this.Width / 2f), this.Y + (this.Height / 2f)); }
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
        /// Returns a rectangle created from a simple string representation.
        /// </summary>
        /// <param name="input">A string containing four comma-delimited numbers.</param>
        /// <returns>A rectangle created from the string.</returns>
        public static BoundingRectangle FromSimpleString(string input)
        {
            string[] components = input.Split(',');

            if (components.Length != 4)
            {
                throw new Exception("BoundingRectangle.Deserialize(string): Invalid input.");
            }

            for (int i = 1; i < 4; i++)
            {
                components[i] = components[i].TrimStart();
            }

            float x, y, width, height;

            if (!float.TryParse(components[0], out x)) { throw new Exception(string.Format("BoundingRectangle.Deserialize(string): Invalid value for X component. Input is {0}.", input)); }
            if (!float.TryParse(components[1], out y)) { throw new Exception(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Y component. Input is {0}.", input)); }
            if (!float.TryParse(components[2], out width)) { throw new Exception(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Width component. Input is {0}.", input)); }
            if (!float.TryParse(components[3], out height)) { throw new Exception(string.Format("BoundingRectangle.Deserialize(string): Invalid value for Height component. Input is {0}.", input)); }

            return new BoundingRectangle(x, y, width, height);
        }

        /// <summary>
        /// Returns a string representing the components of this rectangle.
        /// </summary>
        /// <returns>A string in the format "X, Y, Width, Height".</returns>
        public string ToSimpleString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.X, this.Y, this.Width, this.Height);
        }

        /// <summary>
        /// Checks if any component of this rectangle is Not a Number (NaN).
        /// </summary>
        /// <returns>True if any component is equal to float.NaN, false if otherwise.</returns>
        public bool IsNaN()
        {
            return this.min.IsNaN() || this.max.IsNaN();
        }

        /// <summary>
        /// Determines if a given point is within this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is within this rectangle, false if otherwise.</returns>
        public bool Intersects(Vector2 point)
        {
            return (point.X > this.Left && point.X < this.Right) && (point.Y > this.Top && point.Y < this.Bottom);
        }

        /// <summary>
        /// Determines if a given point is within or tangent to this rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is within or tangent to this rectangle, false if otherwise.</returns>
        public bool IntersectsIncludingEdges(Vector2 point)
        {
            return (point.X >= this.Left && point.X <= this.Right) && (point.Y >= this.Top && point.Y <= this.Bottom);
        }

        /// <summary>
        /// Determines if a given rectangle is intersecting this rectangle.
        /// </summary>
        /// <param name="that">The rectangle to check.</param>
        /// <returns>True if any part of the other rectangle is intersecting this rectangle, false if otherwise.</returns>
        public bool Intersects(BoundingRectangle that)
        {
            if (that.Right <= this.Left || that.Left >= this.Right)
            {
                return false;
            }
            else if (that.Bottom <= this.Top || that.Top >= this.Bottom)
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
            Vector2 pointOnSlope = triangle.GetPointOnSlope(this.Center.X);
            Vector2 rectCenterPoint = (triangle.SlopedSides == RtSlopedSides.TopLeft || triangle.SlopedSides == RtSlopedSides.TopRight) ? this.BottomCenter : this.TopCenter;

            if (!this.Intersects(triangle.Bounds))
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
            Vector2 result = Vector2.Zero;

            // X-axis checks
            if (that.Right <= this.Left || that.Left >= this.Right)
            {
                result.X = float.NaN;
            }
            else
            {
                if (that.Center.X <= this.Center.X)
                {
                    result.X = -(that.Right - this.Left);
                }
                else if (that.Center.X > this.Center.X)
                {
                    result.X = this.Right - that.Left;
                }
            }
            
            // Y-axis checks
            if (that.Bottom <= this.Top || that.Top >= this.Bottom)
            {
                result.Y = float.NaN;
            }
            else
            {
                if (that.Center.Y <= this.Center.Y)
                {
                    result.Y = -(that.Bottom - this.Top);
                }
                else if (that.Center.Y > this.Center.Y)
                {
                    result.Y = this.Bottom - that.Top;
                }
            }

            return result.IsNaN() ? new Vector2(float.NaN, float.NaN) : result;
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
        public Resolution GetCollisionResolution(BoundingRectangle that)
        {
            Vector2 intersection = this.GetIntersectionDepth(that);

            if (intersection.IsNaN())
            {
                return Resolution.Zero;
            }
            else if (Math.Abs(intersection.X) < Math.Abs(intersection.Y))
            {
                return new Resolution(new Vector2(intersection.X, 0f));
            }
            else
            {
                return new Resolution(new Vector2(0f, intersection.Y));
            }
        }

        /// <summary>
        /// Returns a standard Rectangle that uses integral components.
        /// </summary>
        /// <returns>The standard Rectangle.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)this.min.X, (int)this.min.Y, (int)this.max.X, (int)this.max.Y);
        }

        /////// <summary>
        /////// Returns the distance to move a given rectangle by to
        /////// resolve a collision with this rectangle.
        /////// </summary>
        /////// <param name="that">The rectangle to resolve.</param>
        /////// <returns>The distance to move the rectangle by.</returns>
        ////public Vector2 GetCollisionResolution(BoundingRectangle that)
        ////{
        ////    Vector2 resolution = new Vector2(float.MaxValue, float.MaxValue); // this is so the shallowest-edge checks don't fail if the X or Y resolutions happen to be zero

        ////    // Step 1: Get the depth and direction on the X (horizontal) axis.
        ////    if (that.Right <= this.Left || this.Right <= that.Left)
        ////    {
        ////        // The other rect is to our left or right.
        ////        // Per axis separation theorem, we're not intersecting.
        ////        return Vector2.Zero;
        ////    }
        ////    else if (that.Left > this.Left && that.Right < this.Right)
        ////    {
        ////        // The other rect is completely contained within this one. The direction is derived by the distance to the center.
        ////        float thisCenter = this.X + (this.Width / 2f);
        ////        float thatCenter = that.X + (that.Width / 2f);

        ////        if (thatCenter >= thisCenter)
        ////        {
        ////            resolution.X = this.Right - that.Left;
        ////        }
        ////        else if (thisCenter <= thatCenter)
        ////        {
        ////            resolution.X = -(that.Right - this.Left);
        ////        }
        ////    }
        ////    else if (that.Right > this.Left && that.Right < this.Right)
        ////    {
        ////        // The right edge of the other rect is between the edges of this rect. The direction is left.
        ////        resolution.X = -(that.Right - this.Left);
        ////    }
        ////    else if (that.Left > this.Left && that.Left < this.Right)
        ////    {
        ////        // The left edge of the other rect is between the edges of this rect. The direction is right.
        ////        resolution.X = this.Right - that.Left;
        ////    }

        ////    // Step 2: Get the depth and direction on the Y (vertical) axis.
        ////    if (that.Bottom <= this.Top || that.Top >= this.Bottom)
        ////    {
        ////        // The other rect is above or below us.
        ////        // Per axis separation theorem, we're not intersecting.
        ////        return Vector2.Zero;
        ////    }
        ////    else if (that.Top > this.Top && that.Bottom < this.Bottom)
        ////    {
        ////        // The other rect is completely contained within this one. The direction is derived by the distance to the center.
        ////        float thisCenter = this.Y + (this.Height / 2f);
        ////        float thatCenter = that.Y + (that.Height / 2f);

        ////        if (thatCenter >= thisCenter)
        ////        {
        ////            resolution.Y = this.Bottom - that.Top;
        ////        }
        ////        else if (thatCenter <= thisCenter)
        ////        {
        ////            resolution.Y = -(that.Bottom - this.Top);
        ////        }
        ////    }
        ////    else if (that.Bottom > this.Top && that.Bottom < this.Bottom)
        ////    {
        ////        // The bottom edge of the other rect is between the edges of this rect. The direction is up.
        ////        resolution.Y = -(that.Bottom - this.Top);
        ////    }
        ////    else if (that.Top > this.Top && that.Top < this.Bottom)
        ////    {
        ////        // The top edge of the other rect is between the edges of this rect. The direction is down.
        ////        resolution.Y = this.Bottom - that.Top;
        ////    }

        ////    // Step 3: Determine the shallowest edge and correct the resolution.
        ////    if (Math.Abs(resolution.X) <= Math.Abs(resolution.Y))
        ////    {
        ////        // Horizontal edge of the intersection is shallower than the vertical edge.
        ////        // Resolve horizontally.
        ////        resolution.Y = 0;
        ////        return resolution;
        ////    }
        ////    else
        ////    {
        ////        // Vertical edge of the intersection is shallower than the horizontal edge.
        ////        // Resolve vertically.
        ////        resolution.X = 0;
        ////        return resolution;
        ////    }
        ////}

        /// <summary>
        /// Draws this rectangle to the screen.
        /// </summary>
        /// <param name="color">The color of the rectangle.</param>
        public void Draw(Color color)
        {
            GameServices.SpriteBatch.DrawRectangle(this.ToRectangle(), color);
        }

        /// <summary>
        /// Draws the outline of this rectangle to the screen.
        /// </summary>
        /// <param name="color">The color of the outline.</param>
        public void DrawOutline(Color color)
        {
            GameServices.SpriteBatch.DrawRectangleEdges(this.ToRectangle(), color);
        }

        /// <summary>
        /// Returns a string representing key values of this rectangle.
        /// </summary>
        /// <returns>A string representing key values of this rectangle.</returns>
        public override string ToString()
        {
            return "{X:" + this.X.ToString() +
                   " Y:" + this.Y.ToString() +
                   " Width:" + this.Width.ToString() +
                   " Height:" + this.Height.ToString() + "}";
        }       
    }
}
