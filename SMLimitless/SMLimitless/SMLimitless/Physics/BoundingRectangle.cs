//-----------------------------------------------------------------------
// <copyright file="BoundingRectangle.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    // Credit to fbrookie.

    /// <summary>
    /// Using Rectangle for Collision bounds causes 'jiggling' as Rectangle 
    /// must round values to integers. This struct uses float for precision.
    /// </summary>
    public struct BoundingRectangle
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
            get { return this.min.X; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the top line.
        /// </summary>
        public float Top
        {
            get { return this.min.Y; }
        }

        /// <summary>
        /// Gets the X-coordinate of the right line.
        /// </summary>
        public float Right
        {
            get { return this.min.X + this.max.X; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom line.
        /// </summary>
        public float Bottom
        {
            get { return this.min.Y + this.max.Y; }
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
        /// Gets the intersection depth between this rectangle
        /// and another. Please use the <see cref="Intersection"/>
        /// structure instead of this method - that struct provides
        /// much more detail and help in resolving collisions.
        /// </summary>
        /// <param name="other">The other rectangle to check.</param>
        /// <returns>The intersection depth.</returns>
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
        /// Draws this rectangle to the screen.
        /// </summary>
        /// <param name="color">The color of the rectangle.</param>
        public void Draw(Color color)
        {
            GameServices.SpriteBatch.DrawRectangle(this.ToRectangle(), color);
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
