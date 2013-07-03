//-----------------------------------------------------------------------
// <copyright file="ICollidableShape.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Physics;

namespace SMLimitless.Interfaces
{
    /// <summary>
    /// Defines an object with a shape and the ability
    /// to return collision resolution distances.
    /// </summary>
    public interface ICollidableShape
    {
        /// <summary>
        /// Gets the shape of this collidable object.
        /// </summary>
        CollidableShape Shape { get; }

        /// <summary>
        /// Gets the minimum distance to offset a given rectangle
        /// so that it will no longer be colliding with this shape.
        /// </summary>
        /// <param name="rect">The rectangle to resolve.</param>
        /// <returns>The minimum distance to offset the given rectangle.</returns>
        Intersection GetResolutionDistance(BoundingRectangle rect);
    }
}
