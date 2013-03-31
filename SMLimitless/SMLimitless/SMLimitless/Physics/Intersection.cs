using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless;
using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    public struct Intersection
    {
        public Vector2 Depth;

        public Direction Direction
        {
            get
            {
                if (this.Depth == Vector2.Zero)
                {
                    throw new Exception("Intersection.Direction.Get: No direction if not intersecting.");
                }
                if ((Math.Abs(Depth.X) > Math.Abs(Depth.Y)) || (Math.Abs(Depth.X) == Math.Abs(Depth.Y)))
                {
                    // Resolve vertically
                    if (Depth.Y < 0) return Direction.Up;
                    else return Direction.Down;
                }
                else
                {
                    // Resolve horizontally
                    if (Depth.X < 0) return Direction.Left;
                    else return Direction.Right;
                }
            }
        }

        private Vector2 multiplier
        {
            get
            {
                switch (this.Direction)
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

        public bool IsIntersecting
        {
            get
            {
                return this.Depth != Vector2.Zero;
            }
        }

        public Intersection(Vector2 intersection)
        {
            this.Depth = intersection;
        }

        public Intersection(BoundingRectangle a, BoundingRectangle b) : this(a.GetIntersectionDepth(b)) { }

        /// <summary>
        /// Gets the minimum distance necessary to resolve this collision.
        /// </summary>
        public Vector2 GetIntersectionResolution()
        {
            return this.Depth * this.multiplier;
        }

        public override string ToString()
        {
            if (Depth != Vector2.Zero)
            {
                return String.Format("Depth: {0}, {1}; Direction: {2}", Depth.X, Depth.Y, Direction.ToString());
            }
            else
            {
                return String.Format("Depth: {0}, {1}", Depth.X, Depth.Y);
            }
        }

        private static IOrderedEnumerable<Intersection> SortByX(List<Intersection> intersections)
        {
            return intersections.OrderBy(i => i.Depth.X);
        }

        private static IOrderedEnumerable<Intersection> SortByY(List<Intersection> intersections)
        {
            return intersections.OrderBy(i => i.Depth.Y);
        }

        /// <summary>
        /// For a collection of intersections, all intersections with equal X or Y
        /// intersection depths will be added to reduce the number of intersections
        /// and unify resolution directions.  Intersections are consolidated by X
        /// first, and then Y.
        /// </summary>
        public static List<Intersection> ConsolidateIntersections(List<Intersection> intersections)
        {
            if (intersections.Count == 2 && intersections[0].Direction == Direction.Right && intersections[1].Direction == Direction.Up)
            {
                //System.Diagnostics.Debugger.Break();
            }
            var sortedByX = SortByX(intersections).ToList();
            var sortedByY = SortByY(intersections).ToList();

            var result = new List<Intersection>();

            float lastX = float.MinValue;
            float lastY = float.MinValue;

            for (int i = 0; i < sortedByX.Count; i++)
            {
                var current = sortedByX[i];
                if (current.Depth.X == lastX)
                {
                    var last = sortedByX[i - 1];
                    result.Remove(current);
                    result.Remove(last);
                    result.AddUnlessDuplicate(new Intersection(new Vector2(lastX, Single.MaxValue)));
                }
                else if (current.Depth.X > lastX)
                {
                    lastX = current.Depth.X;
                    result.AddUnlessDuplicate(current);
                }
                else
                {
                    result.Add(current);
                }
            }

            for (int i = 0; i < sortedByY.Count; i++)
            {
                var current = sortedByY[i];
                if (current.Depth.Y == lastY)
                {
                    var last = sortedByY[i - 1];
                    result.Remove(current);
                    result.Remove(last);
                    result.AddUnlessDuplicate(new Intersection(new Vector2(Single.MaxValue, lastY)));
                }
                else if (current.Depth.Y > lastY)
                {
                    lastY = current.Depth.Y;
                    result.AddUnlessDuplicate(current);
                }
                else
                {
                    result.AddUnlessDuplicate(current);
                }
            }

            return result;
        }
    }
}
