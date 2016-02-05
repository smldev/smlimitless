//-----------------------------------------------------------------------
// <copyright file="SpriteComponent.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// A base type for reusable objects that modify a sprite's behavior in a given way.
    /// </summary>
    public abstract class SpriteComponent
    {
        /// <summary>
        /// Gets the sprite that's using this component.
        /// </summary>
        public Sprite Owner { get; protected set; }

        /// <summary>
        /// Updates this sprite component.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Initializes this component.
        /// </summary>
        /// <param name="owner">The sprite that owns this component.</param>
        public virtual void Initialize(Sprite owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// A method called when the owner sprite collides with a tile.
        /// </summary>
        /// <param name="collidingTile">The tile that the owner sprite collided with.</param>
        public virtual void HandleTileCollision(Tile collidingTile, Vector2 resolutionDistance) { }

        /// <summary>
        /// A method called when the owner sprite collides with another sprite.
        /// </summary>
        /// <param name="collidingSprite">The sprite that the owner sprite collided with.</param>
        public virtual void HandleSpriteCollision(Sprite collidingSprite, Vector2 resolutionDistance) { }
    }
}
