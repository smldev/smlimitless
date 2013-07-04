//-----------------------------------------------------------------------
// <copyright file="Intersection.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents an intersection between two geometric shapes.
    /// </summary>
    public struct Intersection
    {
        /// <summary>
        /// The backing field for the Depth property.
        /// </summary>
        private Vector2 depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Intersection"/> struct.
        /// </summary>
        /// <param name="depth">The intersection depth.</param>
        public Intersection(Vector2 depth) : this()
        {
            this.depth = depth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Intersection"/> struct.
        /// </summary>
        /// <param name="x">The horizontal depth of the intersection.</param>
        /// <param name="y">The vertical depth of the intersection.</param>
        public Intersection(float x, float y) : this()
        {
            this.depth = new Vector2(x, y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Intersection"/> struct.
        /// </summary>
        /// <param name="a">A bounding rectangle.</param>
        /// <param name="b">A bounding rectangle that may be intersecting the first.</param>
        public Intersection(BoundingRectangle a, BoundingRectangle b)
            : this(a.GetResolutionDistance(b).Depth)
        {
        }

        /// <summary>
        /// Gets an Intersection with a zero resolution distance.
        /// </summary>
        public static Intersection Zero
        {
            get
            {
                return new Intersection(Vector2.Zero);
            }
        }

        /// <summary>
        /// Gets or sets the depth of the intersection.
        /// </summary>
        public Vector2 Depth
        {
            get
            {
                return this.depth;
            }

            set
            {
                this.depth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// Intersection was created by checking a BoundingRectangle
        /// and the sloped side of a RightTriangle for collision.
        /// </summary>
        public bool IsSlopedIntersection { get; set; }

        /// <summary>
        /// Gets the direction that any resolution should use
        /// to resolve this intersection. "Shallowest-edge"
        /// is used to determine on which axis to resolve.
        /// If |X| > |Y|, then the direction will be vertical;
        /// if |Y| > |X|, then the direction will be horizontal.
        /// If both X and Y are 0, there is no intersection
        /// and an exception will be thrown.
        /// </summary>
        public Direction Direction
        {
            get
            {
                if (this.IsSlopedIntersection)
                {
                    if (this.Depth.Y < 0)
                    {
                        return Direction.Up;
                    }
                    else if (this.Depth.Y > 0)
                    {
                        return Direction.Down;
                    }
                    else
                    {
                        throw new Exception("Intersection.Direction.Get: (sloped) No direction if not intersecting.");
                    }
                }
                else
                {
                    if (this.Depth == Vector2.Zero)
                    {
                        throw new Exception("Intersection.Direction.Get: No direction if not intersecting.");
                    }

                    if (Math.Abs(this.Depth.X) > Math.Abs(this.Depth.Y))
                    {
                        // Resolve vertically
                        if (this.Depth.Y < 0)
                        {
                            return Direction.Up;
                        }
                        else
                        {
                            return Direction.Down;
                        }
                    }
                    else
                    {
                        // Resolve horizontally
                        if (this.Depth.X < 0)
                        {
                            return Direction.Left;
                        }
                        else
                        {
                            return Direction.Right;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this intersection is
        /// actually an intersection.
        /// </summary>
        public bool IsIntersecting
        {
            get
            {
                return this.Depth != Vector2.Zero;
            }
        }

        /// <summary>
        /// Gets a value that, when multiplied by the intersection depth,
        /// zeroes out any axis that should NOT be resolved.
        /// </summary>
        private Vector2 Multiplier
        {
            get
            {
                switch (Direction)
                {
                    case Direction.Up:
                        return new Vector2(0f, 1f);
                    case Direction.Down:
                        return new Vector2(0f, 1f);
                    case Direction.Left:
                        return new Vector2(1f, 0f);
                    case Direction.Right:
                        return new Vector2(1f, 0f);
                    default:
                        throw new Exception("Intersection.multiplier.Get: Direction somehow unspecified");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the intersection
        /// depth is equal on both axes, which means that the
        /// intersection is on one of the corners of the
        /// intersecting rectangles.
        /// </summary>
       public bool IsCornerIntersection
        {
            get
            {
                return this.Depth.X == this.Depth.Y;
            }
        }

        /// <summary>
        /// For a collection of intersections, all intersections with equal X or Y
        /// intersection depths will be added to reduce the number of intersections
        /// and unify resolution directions.  Intersections are consolidated by X
        /// first, and then Y.
        /// </summary>
        /// <param name="intersections">The list of unconsolidated intersections.</param>
        /// <returns>A list of consolidated intersections.</returns>
        [Obsolete]
        public static List<Intersection> ConsolidateIntersections(List<Intersection> intersections)
        {
            ////var sortedByX = SortByX(intersections).ToList();
            ////var sortedByY = SortByY(intersections).ToList();

            var result = new List<Intersection>();

            ////float lastX = float.MinValue;
            ////float lastY = float.MinValue;

            ////for (int i = 0; i < sortedByX.Count; i++)
            ////{
            ////    var current = sortedByX[i];
            ////    if (current.Depth.X == lastX)
            ////    {
            ////        var last = sortedByX[i - 1];
            ////        result.Remove(current);
            ////        result.Remove(last);
            ////        result.AddUnlessDuplicate(new Intersection(new Vector2(lastX, float.MaxValue)));
            ////    }
            ////    else if (current.Depth.X > lastX)
            ////    {
            ////        lastX = current.Depth.X;
            ////        result.AddUnlessDuplicate(current);
            ////    }
            ////    else
            ////    {
            ////        result.Add(current);
            ////    }
            ////}

            ////for (int i = 0; i < sortedByY.Count; i++)
            ////{
            ////    var current = sortedByY[i];
            ////    if (current.Depth.Y == lastY)
            ////    {
            ////        var last = sortedByY[i - 1];
            ////        result.Remove(current);
            ////        result.Remove(last);
            ////        result.AddUnlessDuplicate(new Intersection(new Vector2(float.MaxValue, lastY)));
            ////    }
            ////    else if (current.Depth.Y > lastY)
            ////    {
            ////        lastY = current.Depth.Y;
            ////        result.AddUnlessDuplicate(current);
            ////    }
            ////    else
            ////    {
            ////        result.AddUnlessDuplicate(current);
            ////    }
            ////}

            return result;
        }

        /// <summary>
        /// Returns the minimum distance necessary to resolve this collision.
        /// </summary>
        /// <returns>The minimum distance necessary to resolve this collision.</returns>
        public Vector2 GetIntersectionResolution()
        {
            return this.Depth != Vector2.Zero ? this.Depth * this.Multiplier : Vector2.Zero;
        }

        /// <summary>
        /// Returns a string containing useful information.
        /// </summary>
        /// <returns>A string containing useful information.</returns>
        public override string ToString()
        {
            if (this.Depth != Vector2.Zero)
            {
                return string.Format("Depth: {0}, {1}; Direction: {2}", this.Depth.X, this.Depth.Y, Direction.ToString());
            }
            else
            {
                return string.Format("Depth: {0}, {1}", this.Depth.X, this.Depth.Y);
            }
        }
        
        /// <summary>
        /// Returns a collection of Intersections sorted by the X component of their depths.
        /// </summary>
        /// <param name="intersections">The unsorted collection.</param>
        /// <returns>The sorted collection.</returns>
        private static IOrderedEnumerable<Intersection> SortByX(List<Intersection> intersections)
        {
            return intersections.OrderBy(i => i.Depth.X);
        }

        /// <summary>
        /// Returns a collection of Intersections sorted by the Y component of their depths.
        /// </summary>
        /// <param name="intersections">The unsorted collection.</param>
        /// <returns>The sorted collection.</returns>
        private static IOrderedEnumerable<Intersection> SortByY(List<Intersection> intersections)
        {
            return intersections.OrderBy(i => i.Depth.Y);
        }
    }
}
