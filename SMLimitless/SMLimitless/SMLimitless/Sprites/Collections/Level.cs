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
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// The main unit of gameplay.
    /// </summary>
    public sealed class Level
    {
        private List<LevelExit> levelExits;

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

        public Level()
        {
            this.levelExits = new List<LevelExit>();
        }

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

        public void LevelExitCleared(string exitSpriteName)
        {
            // Look in this.levelExits for an exit with the sprite name
            // Notify the owner (world/levelpack/whatever) that this exit has been cleared
            // Give the owner the LevelExit tied to the exitSpriteName
        }
    }
}
