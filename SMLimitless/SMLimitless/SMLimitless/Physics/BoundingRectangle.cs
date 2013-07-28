//-----------------------------------------------------------------------
// <copyright file="BoundingRectangle.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        /// The size.
        /// </summary>
        private Vector2 max;

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
            get { return (this.min + this.max) / 2; }
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
        /// Gets the intersection depth between this rectangle
        /// and another. Please use the <see cref="Intersection"/>
        /// structure instead of this method - that struct provides
        /// much more detail and help in resolving collisions.
        /// </summary>
        /// <param name="other">The other rectangle to check.</param>
        /// <returns>The intersection depth.</returns>
        [Obsolete]
        public Vector2 GetIntersectionDepth(BoundingRectangle other)
        {
            // Calculate half sizes.
            float halfWidthA = this.Width / 2.0f;
            float halfHeightA = this.Height / 2.0f;
            float halfWidthB = other.Width / 2.0f;
            float halfHeightB = other.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(this.Left + halfWidthA, this.Top + halfHeightA);
            Vector2 centerB = new Vector2(other.Left + halfWidthB, other.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            {
                return Vector2.Zero;
            }

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Returns a standard Rectangle that uses integral components.
        /// </summary>
        /// <returns>The standard Rectangle.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)this.min.X, (int)this.min.Y, (int)this.max.X, (int)this.max.Y);
        }

        /// <summary>
        /// Gets the minimum distance to offset a given rectangle
        /// so that it will no longer be colliding with this shape.
        /// </summary>
        /// <param name="rect">The rectangle to resolve.</param>
        /// <returns>The minimum distance to offset the given rectangle.</returns>
        [Obsolete]
        public Vector2 GetResolutionDistance(BoundingRectangle rect)
        {
            Vector2 resolution = Vector2.Zero;

            // Get the horizontal resolution
            if (rect.Right <= this.Left || rect.Left >= this.Right)
            {
                return Vector2.Zero;
            }
            else if (this.Left < rect.Right && rect.Right < this.Right)
            {
                resolution.X = -(rect.Right - this.Left);
            }
            else if (this.Left < rect.Left && rect.Left < this.Right)
            {
                resolution.X = this.Right - rect.Left;
            }

            // Get the vertical resolution
            if (rect.Bottom <= this.Top || rect.Top >= this.Bottom)
            {
                return Vector2.Zero;
            }
            else if (this.Top < rect.Bottom && rect.Bottom < this.Bottom)
            {
                resolution.Y = -(rect.Bottom - this.Top);
            }
            else if (this.Top < rect.Top && rect.Top < this.Bottom)
            {
                resolution.Y = this.Bottom - rect.Top;
            }

            return new Intersection(resolution).GetIntersectionResolution();
        }

        /// <summary>
        /// Returns the distance to move a given rectangle by to
        /// resolve a collision with this rectangle.
        /// </summary>
        /// <param name="that">The rectangle to resolve.</param>
        /// <returns>The distance to move the rectangle by.</returns>
        public Vector2 GetCollisionResolution(BoundingRectangle that)
        {
            Vector2 resolution = new Vector2(float.MaxValue, float.MaxValue); // this is so the shallowest-edge checks don't fail if the X or Y resolutions happen to be zero

            // Step 1: Get the depth and direction on the X (horizontal) axis.
            if (that.Right <= this.Left || this.Right <= that.Left)
            {
                // The other rect is to our left or right.
                // Per axis separation theorem, we're not intersecting.
                return Vector2.Zero;
            }
            else if (that.Left > this.Left && that.Right < this.Right)
            {
                // The other rect is completely contained within this one. The direction is derived by the distance to the center.
                float thisCenter = this.X + (this.Width / 2f);
                float thatCenter = that.X + (that.Width / 2f);

                if (thatCenter >= thisCenter)
                {
                    resolution.X = this.Right - that.Left;
                }
                else if (thisCenter <= thatCenter)
                {
                    resolution.X = -(that.Right - this.Left);
                }
            }
            else if (that.Right > this.Left && that.Right < this.Right)
            {
                // The right edge of the other rect is between the edges of this rect. The direction is left.
                resolution.X = -(that.Right - this.Left);
            }
            else if (that.Left > this.Left && that.Left < this.Right)
            {
                // The left edge of the other rect is between the edges of this rect. The direction is right.
                resolution.X = this.Right - that.Left;
            }

            // Step 2: Get the depth and direction on the Y (vertical) axis.
            if (that.Bottom <= this.Top || that.Top >= this.Bottom)
            {
                // The other rect is above or below us.
                // Per axis separation theorem, we're not intersecting.
                return Vector2.Zero;
            }
            else if (that.Top > this.Top && that.Bottom < this.Bottom)
            {
                // The other rect is completely contained within this one. The direction is derived by the distance to the center.
                float thisCenter = this.Y + (this.Height / 2f);
                float thatCenter = that.Y + (that.Height / 2f);

                if (thatCenter >= thisCenter)
                {
                    resolution.Y = this.Bottom - that.Top;
                }
                else if (thatCenter <= thisCenter)
                {
                    resolution.Y = -(that.Bottom - this.Top);
                }
            }
            else if (that.Bottom > this.Top && that.Bottom < this.Bottom)
            {
                // The bottom edge of the other rect is between the edges of this rect. The direction is up.
                resolution.Y = -(that.Bottom - this.Top);
            }
            else if (that.Top > this.Top && that.Top < this.Bottom)
            {
                // The top edge of the other rect is between the edges of this rect. The direction is down.
                resolution.Y = this.Bottom - that.Top;
            }

            // Step 3: Determine the shallowest edge and correct the resolution.
            if (Math.Abs(resolution.X) <= Math.Abs(resolution.Y))
            {
                // Horizontal edge of the intersection is shallower than the vertical edge.
                // Resolve horizontally.
                resolution.Y = 0;
                return resolution;
            }
            else
            {
                // Vertical edge of the intersection is shallower than the horizontal edge.
                // Resolve vertically.
                resolution.X = 0;
                return resolution;
            }
        }

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
