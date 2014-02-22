//-----------------------------------------------------------------------
// <copyright file="RightTriangle.cs" company="The Limitless Development Team">
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
        /// Gets or sets the rectangle that completely contains the triangle.
        /// </summary>
        public BoundingRectangle Bounds { get; set; }

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
                    return this.Bounds.Height / this.Bounds.Width;
                }
                else
                {
                    // With the other two triangles (top-right and bottom-left), the slope goes down as you move from left to right thus a negative slope.
                    return -this.Bounds.Height / this.Bounds.Width;
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
                        return new Vector2(this.Bounds.Right, this.Bounds.Bottom);
                    case RtSlopedSides.TopRight:
                        // For a top-right triangle, the point is at the bottom-left corner.
                        return new Vector2(this.Bounds.Left, this.Bounds.Bottom);
                    case RtSlopedSides.BottomLeft:
                        // For a bottom-left triangle, the point is at the top-right corner.
                        return new Vector2(this.Bounds.Right, this.Bounds.Top);
                    case RtSlopedSides.BottomRight:
                        // For a bottom-right triangle, the point is at the top-left corner.
                        return new Vector2(this.Bounds.Left, this.Bounds.Top);
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
                    return new Vector2(this.Bounds.Left, this.Bounds.Bottom);
                }
                else
                {
                    // Bottom-right corner.
                    return new Vector2(this.Bounds.Right, this.Bounds.Bottom);
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
                    return new Vector2(this.Bounds.Right, this.Bounds.Top);
                }
                else
                {
                    // Top-left corner.
                    return new Vector2(this.Bounds.Left, this.Bounds.Top);
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
        /// Gets the horizontal side (left or right) that has the sloped edge.
        /// </summary>
        public HorizontalDirection HorizontalSlopedSide
        {
            get
            {
                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.BottomLeft)
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
        /// Gets the vertical side (up or down) that has the sloped edge.
        /// </summary>
        public VerticalDirection VerticalSlopedSide
        {
            get
            {
                if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.TopRight)
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
        /// Initializes a new instance of the <see cref="RightTriangle"/> class.
        /// </summary>
        /// <param name="bounds">The rectangle that forms the bounds of the triangle.</param>
        /// <param name="slopedSides">Which sides of the triangle are sloped.</param>
        public RightTriangle(BoundingRectangle bounds, RtSlopedSides slopedSides)
        {
            this.Bounds = bounds;
            this.SlopedSides = slopedSides;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RightTriangle"/> class.
        /// </summary>
        /// <param name="x">The horizontal distance from the left edge of the game space.</param>
        /// <param name="y">The vertical distance from the top edge of the game space.</param>
        /// <param name="width">The width of the triangle.</param>
        /// <param name="height">The height of the triangle.</param>
        /// <param name="slopedSides">Which sides of the triangle are replaced by the slope.</param>
        public RightTriangle(float x, float y, float width, float height, RtSlopedSides slopedSides)
            : this(new BoundingRectangle(x, y, width, height), slopedSides)
        {
        }

        /// <summary>
        /// Gets a point directly on the slope of the triangle.
        /// Restricted to the slope itself. To get any point on the line
        /// coincident to this slope, call GetPointOnLine().
        /// </summary>
        /// <param name="x">The X-coordinate to solve for.</param>
        /// <returns>The point directly on the slope, or NaN if the point is not on the slope.</returns>
        public Vector2 GetPointOnSlope(float x)
        {
            // Y = mx + b.
            if (x <= this.Bounds.Left || x >= this.Bounds.Right)
            {
                return new Vector2(float.NaN);
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
        /// Determines if a given rectangle intersects this triangle.
        /// </summary>
        /// <param name="rect">The rectangle to check for intersection.</param>
        /// <returns>True if any part of the rectangle intersects this right triangle, false if otherwise.</returns>
        public bool Intersects(BoundingRectangle rect)
        {
            return rect.Intersects(this);
        }

        public bool Within(Vector2 point, bool adjacentPointsAreWithin)
        {
            if (!this.Bounds.Within(point, adjacentPointsAreWithin))
            {
                return false;
            }

            float pointOnSlopeY = this.GetPointOnSlope(point.X).Y;

            if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.TopRight)
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
            else if (this.SlopedSides == RtSlopedSides.BottomLeft || this.SlopedSides == RtSlopedSides.BottomRight)
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

        /// <summary>
        /// Gets the minimum distance to move a given rectangle such that
        /// it will no longer be intersecting this right triangle.
        /// </summary>
        /// <param name="rect">The rectangle to resolve.</param>
        /// <returns>The minimum distance to move the rectangle,
        /// which can be directly applied to the rectangle's position.</returns>
        /// <remarks>This method resolves collision by first determining if the bottom-center point
        /// (for TopLeft/TopRight triangles) or the top-center point (for BottomLeft/BottomRight
        /// triangles) is within the bounds. If it is, the method treats it as a slope collision 
        /// by checking if the point is between the slope and the top. If it isn't, the collision
        /// is treated as a collision between two rectangles.</remarks>
        public Resolution GetCollisionResolution(BoundingRectangle rect)
        {
            Vector2 rectCollisionPoint = (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.TopRight) ? rect.BottomCenter : rect.TopCenter;
            Vector2 pointOnSlope = this.GetPointOnSlope(rect.Center.X);

            if (!this.Bounds.IntersectsIncludingEdges(rectCollisionPoint))
            {
                Vector2 resolution = this.Bounds.GetCollisionResolution(rect).ResolutionDistance;
                Vector2 result = Vector2.Zero;

                if (this.HorizontalSlopedSide == HorizontalDirection.Right)
                {
                    // Straight edge is left
                    if (resolution.X < 0f)
                    {
                        result.X = resolution.X;
                    }
                }
                else if (this.HorizontalSlopedSide == HorizontalDirection.Left)
                {
                    // Straight edge is right
                    if (resolution.X > 0f)
                    {
                        result.X = resolution.X;
                    }
                }

                if (this.VerticalSlopedSide == VerticalDirection.Up)
                {
                    // Straight edge is bottom
                    if (resolution.Y > 0f)
                    {
                        result.Y = resolution.Y;
                    }
                }
                else if (this.VerticalSlopedSide == VerticalDirection.Down)
                {
                    // Straight edge is top
                    if (resolution.Y < 0f)
                    {
                        result.Y = resolution.Y;
                    }
                }

                // TODO: The above could be condensed into two conditionals, but alas, I'm too tired at the moment.
                return new Resolution(result);
            }
            else
            {
                return new Resolution(this.ResolveSlopeCollision(rect, this.Bounds.GetCollisionResolution(rect).ResolutionDistance), ResolutionType.Slope);
            }
        }

        /// <summary>
        /// Resolves a collision between a rectangle and the sloped side of this triangle.
        /// </summary>
        /// <param name="that">The rectangle to resolve.</param>
        /// <param name="intersection">The intersection between the rectangle and the bounds of this triangle.</param>
        /// <returns>The distance to move the rectangle by to resolve this collision.</returns>
        private Vector2 ResolveSlopeCollision(BoundingRectangle that, Vector2 intersection)
        {
            Vector2 topCenter = new Vector2(that.X + (that.Width / 2f), that.Y);
            Vector2 bottomCenter = new Vector2(that.X + (that.Width / 2f), that.Bottom);
            Vector2 pointOnSlope = this.GetPointOnSlope(that.X + (that.Width / 2f));

            if (pointOnSlope == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            if (this.SlopedSides == RtSlopedSides.TopLeft || this.SlopedSides == RtSlopedSides.TopRight)
            {
                if (bottomCenter.Y > pointOnSlope.Y)
                {
                    // The bottom-center point is below the slope, resolution needed
                    return new Vector2(0f, -(bottomCenter.Y - pointOnSlope.Y));
                }
            }
            else if (this.SlopedSides == RtSlopedSides.BottomLeft || this.SlopedSides == RtSlopedSides.BottomRight)
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
                Rectangle drawBounds = new Rectangle((int)this.Bounds.X + 1, (int)this.Bounds.Y - 1, (int)this.Bounds.Width, (int)this.Bounds.Height);
                drawBounds.DrawOutline(Color.Red);

                // Draw some useful debug information.
                GameServices.DebugFont.DrawString(this.ToString(), new Vector2(this.Bounds.X, this.Bounds.Y));
            }
        }

        /// <summary>
        /// Returns a string representation of the important components of the triangle.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            string boundsString = string.Format("Bounds: X:{0}, Y:{1}, Width:{2}, Height:{3}{4}", this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height, Environment.NewLine);
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
