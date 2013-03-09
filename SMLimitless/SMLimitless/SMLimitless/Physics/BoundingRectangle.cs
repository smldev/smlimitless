using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Using Rectangle for Collision bounds causes 'jiggling' as Rectangle 
    /// must round values to int. This struct uses float for precision.
    /// Credit to fbrookie.
    /// </summary>
    public struct BoundingRectangle
    {
        private Vector2 min;
        private Vector2 max;

        public float X
        {
            get { return min.X; }
            set { min.X = value; }
        }

        public float Y
        {
            get { return min.Y; }
            set { min.Y = value; }
        }

        public float Width
        {
            get { return max.X; }
            set { max.X = value; }
        }

        public float Height
        {
            get { return max.Y; }
            set { max.Y = value; }
        }

        public float Left
        {
            get { return min.X; }
        }

        public float Top
        {
            get { return min.Y; }
        }

        public float Right
        {
            get { return min.X + max.X; }
        }

        public float Bottom
        {
            get { return min.Y + max.Y; }
        }

        public Vector2 Position
        {
            get { return min; }
            set { min = value; }
        }

        public Vector2 Size
        {
            get { return max; }
            set { max = value; }
        }

        public Vector2 Centre
        {
            get { return min + max / 2; }
        }

        public BoundingRectangle(Rectangle rectangle)
            : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height) { }

        public BoundingRectangle(Vector2 start, Vector2 end)
            : this(start.X, start.Y, end.X - start.X, end.Y - start.Y) { }

        public BoundingRectangle(float x, float y, float width, float height)
        {
            min = new Vector2(x, y);
            max = new Vector2(width, height);
        }

        /// <summary>
        /// Calculates the signed depth of intersection between two BoundingRectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting BoundingRectangles. 
        /// These depth values can be negative depending on which sides the 
        /// BoundingRectangles intersect. This allows callers to determine 
        /// the correct direction to push objects in order to resolve collisions.
        /// If the BoundingRectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public Vector2 GetIntersectionDepth(BoundingRectangle other)
        {
            //return ToRectangle().GetIntersectionDepth(other.ToRectangle());

            // Calculate half sizes.
            float halfWidthA = Width / 2.0f;
            float halfHeightA = Height / 2.0f;
            float halfWidthB = other.Width / 2.0f;
            float halfHeightB = other.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(Left + halfWidthA, Top + halfHeightA);
            Vector2 centerB = new Vector2(other.Left + halfWidthB, other.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Returns a standard Rectangle.
        /// </summary>
        /// <returns>The standard Rectangle.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)min.X, (int)min.Y, (int)max.X, (int)max.Y);
        }

        public void Draw(Color color)
        {
            GameServices.SpriteBatch.DrawRectangle(this.ToRectangle(), color);
        }

        public override string ToString()
        {
            return "{X:" + X.ToString() + 
                   " Y:" + Y.ToString() + 
                   " Width:" + Width.ToString() + 
                   " Height:" + Height.ToString() + "}";
        }
    }
}
