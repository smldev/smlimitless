//-----------------------------------------------------------------------
// <copyright file="RightTriangle.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a right triangle, used for sloped tiles.
    /// </summary>
    public class RightTriangle : ICollidableShape
    {
        /// <summary>
        /// A rectangle that completely contains the triangle.
        /// </summary>
        private BoundingRectangle bounds;

        /// <summary>
        /// Gets or sets a value indicating which sides of the triangle are sloped.
        /// </summary>
        public RtSlopedSides SlopedSides { get; set; }

        /// <summary>
        /// Gets the slope (rise over run) of the triangle.
        /// </summary>
        public float Slope
        {
            get
            {
                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.BottomRight)
                {
                    // With these two triangles (top-left and bottom-right), the slope goes up as you move from left to right, thus a positive slope.
                    return this.bounds.Height / this.bounds.Width;
                }
                else
                {
                    // With the other two triangles (top-right and bottom-left), the slope goes down as you move from left to right thus a negative slope.
                    return -this.bounds.Height / this.bounds.Width;
                }
            }
        }

        /// <summary>
        /// Gets the location of the point of the 90-degree angle.
        /// </summary>
        public Vector2 Point90
        {
            get
            {
                switch (this.SlopedSides)
                {
                    case RtSlopedSides.TopLeft:
                        // For a top-left triangle, the point is at the bottom-right corner.
                        return new Vector2(this.bounds.Right, this.bounds.Bottom);
                    case RtSlopedSides.TopRight:
                        // For a top-right triangle, the point is at the bottom-left corner.
                        return new Vector2(this.bounds.Left, this.bounds.Bottom);
                    case RtSlopedSides.BottomLeft:
                        // For a bottom-left triangle, the point is at the top-right corner.
                        return new Vector2(this.bounds.Right, this.bounds.Top);
                    case RtSlopedSides.BottomRight:
                        // For a bottom-right triangle, the point is at the top-left corner.
                        return new Vector2(this.bounds.Left, this.bounds.Top);
                    default:
                        // This can't happen, but the compiler complains.
                        return new Vector2(float.NaN, float.NaN);
                }
            }
        }

        /// <summary>
        /// Gets the location of the point at the bottom of the slope.
        /// </summary>
        public Vector2 Point1
        {
            get
            {
                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.BottomRight)
                {
                    // Bottom-left corner.
                    return new Vector2(this.bounds.Left, this.bounds.Bottom);
                }
                else
                {
                    // Bottom-right corner.
                    return new Vector2(this.bounds.Right, this.bounds.Bottom);
                }
            }
        }

        /// <summary>
        /// Gets the location of the point at the top of the slope.
        /// </summary>
        public Vector2 Point2
        {
            get
            {
                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.BottomRight)
                {
                    // Top-right corner.
                    return new Vector2(this.bounds.Right, this.bounds.Top);
                }
                else
                {
                    // Top-left corner.
                    return new Vector2(this.bounds.Left, this.bounds.Top);
                }
            }
        }

        /// <summary>
        /// Gets the point on the Y-axis where the line
        /// coinciding with the slope intersects with the Y-axis.
        /// Equivalent to the variable b in the linear equation y = mx + b.
        /// </summary>
        /// <remarks>
        /// To account for the fact that, in XNA, the Y-axis is flipped
        /// (positive Y goes down), the equation is b = mx + y.
        /// </remarks>
        public float YIntersect
        {
            get
            {
                // Ordinarily, the y-intersect is equal to (Slope * x) - y,
                // but since the Y-axis is flipped in XNA, it's equal to (Slope * x) + y.
                return (this.Slope * this.Point1.X) + this.Point1.Y;
            }
        }

        /// <summary>
        /// Gets the shape of this collidable object.
        /// </summary>
        public CollidableShape Shape
        {
            get
            {
                return CollidableShape.RightTriangle;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RightTriangle"/> class.
        /// </summary>
        /// <param name="bounds">The rectangle that forms the bounds of the triangle.</param>
        /// <param name="slopedSides">Which sides of the triangle are sloped.</param>
        public RightTriangle(BoundingRectangle bounds, RtSlopedSides slopedSides)
        {
            this.bounds = bounds;
            this.SlopedSides = slopedSides;
        }

        /// <summary>
        /// Gets a point directly on the slope of the triangle.
        /// Restricted to the slope itself. To get any point on the line
        /// coincident to this slope, call GetPointOnLine().
        /// </summary>
        /// <param name="x">The X-coordinate to solve for.</param>
        /// <returns>The point directly on the slope.</returns>
        public Vector2 GetPointOnSlope(float x)
        {
            // Y = mx + b.
            if (x <= this.bounds.Left || x >= this.bounds.Right)
            {
                return Vector2.Zero;
            }

            // Ordinarily, y = mx + b.
            // But since the Y-axis is reversed, y = mx - b.
            // But we still need a positive value, so we return negative Y.
            float y = (this.Slope * x) - this.YIntersect;
            return new Vector2(x, -y);
        }

        /// <summary>
        /// Gets a point directly on the line that is
        /// coincident to the slope of the triangle.
        /// To get points only on the slope itself,
        /// call GetPointOnSlope().
        /// </summary>
        /// <param name="x">The X-coordinate to solve for.</param>
        /// <returns>A point directly on the line.</returns>
        public Vector2 GetPointOnLine(float x)
        {
            float y = (this.Slope * x) - this.YIntersect;
            return new Vector2(x, -y);
        }

        /// <summary>
        /// Determines the minimum distance to offset a given
        /// bounding rectangle that may be colliding with this
        /// triangle.
        /// </summary>
        /// <param name="rect">The rectangle to resolve for.</param>
        /// <returns>The minimum resolution distance.</returns>
        public Intersection GetResolutionDistance(BoundingRectangle rect)
        {
            // First, check if the rectangle is intersecting our bounds.
            Intersection intersect = new Intersection(rect, this.bounds);
            if (intersect.IsIntersecting)
            {
                // There are two ways to resolve this: by the sloped edge,
                // or by the two straight edges. For TopLeft and TopRight
                // triangles, we check if the bottom center point on the rectangle
                // is between the slope and the bottom line. For the BottomLeft
                // and BottomRight triangles, we check if the top center point
                // is between the slope and the top line. If so, the resolution
                // distance is the difference between the bottom center or top center
                // point and the point on the slope.
                //
                // If the bottom center or top center point isn't between the slope
                // and the long side, or if the the point on the slope is zero (meaning
                // that the point was too far to the left or to the right of the slope,
                // or if the bottom center or top center point is below or above the long
                // side of the rectangle, the resolution is handled like a normal rectangle-
                // rectangle intersection.
                Vector2 bottomCenter = new Vector2(rect.Left + (rect.Width / 2f), rect.Bottom);
                Vector2 topCenter = new Vector2(rect.Left + (rect.Width / 2f), rect.Top);
                Vector2 pointOnSlope = this.GetPointOnSlope(rect.Left + (rect.Width / 2f));

                if (pointOnSlope == Vector2.Zero)
                {
                    goto ResolveAsRectangle;
                }

                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.TopRight)
                {
                    if (bottomCenter.Y <= pointOnSlope.Y)
                    {
                        return Intersection.Zero;
                    }
                    else if ((bottomCenter.Y > pointOnSlope.Y) && (bottomCenter.Y < this.bounds.Bottom))
                    {
                        Intersection result = new Intersection(pointOnSlope - bottomCenter);
                        result.IsSlopedIntersection = true;
                        return result;
                    }
                    else
                    {
                        goto ResolveAsRectangle;
                    }
                }
                else
                {
                    if (topCenter.Y >= pointOnSlope.Y)
                    {
                        return Intersection.Zero;
                    }
                    else if ((topCenter.Y < pointOnSlope.Y) && (topCenter.Y > this.bounds.Top))
                    {
                        Intersection result = new Intersection(pointOnSlope - topCenter);
                        result.IsSlopedIntersection = true;
                        return result;
                    }
                    else
                    {
                        goto ResolveAsRectangle;
                    }
                }

            ResolveAsRectangle:
                if ((this.SlopedSides == RtSlopedSides.TopLeft && (intersect.Direction == Direction.Right || intersect.Direction == Direction.Down)) ||
                    (this.SlopedSides == RtSlopedSides.TopRight && (intersect.Direction == Direction.Left || intersect.Direction == Direction.Down)) ||
                    (this.SlopedSides == RtSlopedSides.BottomLeft && (intersect.Direction == Direction.Right || intersect.Direction == Direction.Up)) ||
                    (this.SlopedSides == RtSlopedSides.BottomRight && (intersect.Direction == Direction.Left || intersect.Direction == Direction.Up)))
                {
                    return intersect;
                }
                else
                {
                    return Intersection.Zero;
                }
            }

            return Intersection.Zero;
        }

        /// <summary>
        /// Draws this triangle to the screen.
        /// </summary>
        /// <param name="debug">Draws some useful debug information.</param>
        public void Draw(bool debug)
        {
            GameServices.SpriteBatch.DrawLine(1f, Color.White, this.Point90, this.Point1);
            GameServices.SpriteBatch.DrawLine(1f, Color.White, this.Point90, this.Point2);
            GameServices.SpriteBatch.DrawLine(1f, Color.White, this.Point1, this.Point2);

            if (debug)
            {
                // Draw the line coincident to the slope.
                // For the inverted Y-axis, the slope-intercept formula, solved for x, is x = (-y + b) / m
                Vector2 screenSize = GameServices.ScreenSize;
                Vector2 lineStart = new Vector2((-screenSize.Y + this.YIntersect) / this.Slope, screenSize.Y);
                Vector2 lineEnd = new Vector2(this.YIntersect / this.Slope, 0f);
                GameServices.SpriteBatch.DrawLine(1f, Color.ForestGreen, lineStart, lineEnd);

                // DrawLine is a little weird with drawing the right-triangle - 
                // the bounds look visually larger than the triangle, even though they are the right sizes.
                // So we'll correct the bounds so they look right.
                Rectangle drawBounds = new Rectangle((int)this.bounds.X + 1, (int)this.bounds.Y - 1, (int)this.bounds.Width, (int)this.bounds.Height);
                drawBounds.DrawOutline(Color.Red);

                // Draw some useful debug information.
                GameServices.SpriteBatch.DrawString(GameServices.DebugFontSmall, this.ToString(), new Vector2(this.bounds.X, this.bounds.Y), Color.White);
            }
        }

        /// <summary>
        /// Returns a string representation of the important components of the triangle.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            string boundsString = string.Format("Bounds: X:{0}, Y:{1}, Width:{2}, Height:{3}{4}", this.bounds.X, this.bounds.Y, this.bounds.Width, this.bounds.Height, Environment.NewLine);
            string point90String = string.Format("Point 90: X:{0}, Y:{1}{2}", this.Point90.X, this.Point90.Y, Environment.NewLine);
            string point1String = string.Format("Point 1: X:{0}, Y:{1}{2}", this.Point1.X, this.Point1.Y, Environment.NewLine);
            string point2String = string.Format("Point 2: X:{0}, Y:{1}{2}", this.Point2.X, this.Point2.Y, Environment.NewLine);
            string slopeString = string.Format("Slope: {0}, Y-Intersect: {1}, Sloped Sides:{2}{3}", this.Slope, this.YIntersect, this.SlopedSides.ToString(), Environment.NewLine);
            return string.Concat(boundsString, point90String, point1String, point2String, slopeString);
        }
    }

    /// <summary>
    /// An enumeration defining which sides of a right triangle are sloped.
    /// </summary>
    public enum RtSlopedSides
    {
        /// <summary>
        /// The top and left sides of the right triangle are sloped.
        /// </summary>
        TopLeft,

        /// <summary>
        /// The top and right sides of the right triangle are sloped.
        /// </summary>
        TopRight,

        /// <summary>
        /// The bottom and left sides of the right triangle are sloped.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The bottom and right sides of the right triangle are sloped.
        /// </summary>
        BottomRight
    }
}
