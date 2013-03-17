using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless;

namespace SMLimitless.Physics
{
    public struct Intersection
    {
        public Vector2 Depth;

        public IntersectionDirection Direction
        {
            get
            {
                if (this.Depth == Vector2.Zero)
                {
                    return IntersectionDirection.None;
                }
                if ((Math.Abs(Depth.X) > Math.Abs(Depth.Y)) || (Math.Abs(Depth.X) == Math.Abs(Depth.Y)))
                {
                    // Resolve vertically
                    if (Depth.Y < 0) return IntersectionDirection.Up;
                    else return IntersectionDirection.Down;
                }
                else
                {
                    // Resolve horizontally
                    if (Depth.X < 0) return IntersectionDirection.Left;
                    else return IntersectionDirection.Right;
                }
            }
        }

        private Vector2 multiplier
        {
            get
            {
                switch (this.Direction)
                {
                    case IntersectionDirection.Up:
                        return new Vector2(0f, 1f);
                    case IntersectionDirection.Down:
                        return new Vector2(0f, 1f);
                    case IntersectionDirection.Left:
                        return new Vector2(1f, 0f);
                    case IntersectionDirection.Right:
                        return new Vector2(1f, 0f);
                    case IntersectionDirection.None:
                        return Vector2.Zero;
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
    }
}
