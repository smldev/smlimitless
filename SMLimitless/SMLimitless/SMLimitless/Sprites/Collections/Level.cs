//-----------------------------------------------------------------------
// <copyright file="Level.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// The main unit of gameplay.
    /// </summary>
    public sealed class Level
    {
        /// <summary>
        /// Gets the name of the level, which is presented on menu screens.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name of the author who created this level.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// The acceleration caused by gravity, measured in pixels per second per second.
        /// </summary>
        public const float GravityAcceleration = 250f;

        /// <summary>
        /// Gets the main layer of this level.
        /// </summary>
        internal Layer MainLayer { get; private set; } // TODO: it's sections you silly

        /// <summary>
        /// Initializes this level.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Loads the content of this level.
        /// </summary>
        public void LoadContent() { }

        /// <summary>
        /// Updates this level.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw() { }
    }
}
