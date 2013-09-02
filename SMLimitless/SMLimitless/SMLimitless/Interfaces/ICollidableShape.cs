﻿//-----------------------------------------------------------------------
// <copyright file="ICollidableShape.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Physics;

namespace SMLimitless.Interfaces
{
    /// <summary>
    /// Represents a shape that rectangles can collide with.
    /// </summary>
    public interface ICollidableShape
    {
        BoundingRectangle Bounds { get; }

        /// <summary>
        /// Gets the distance to move a given rectangle by
        /// so that it won't be colliding with this shape.
        /// </summary>
        /// <param name="that">The rectangle to resolve.</param>
        /// <returns>The distance to move the rectangle by.</returns>
        Resolution GetCollisionResolution(BoundingRectangle that);

        /// <summary>
        /// Determines if a rectangle intersects this shape.
        /// </summary>
        /// <param name="that">The rectangle to check for intersection.</param>
        /// <returns>True if the rectangle intersects this shape, false if it doesn't.</returns>
        /// <remarks>This method returns False if the rectangle is directly against this shape but not within it.</remarks>
        bool Intersects(BoundingRectangle that);
    }
}
