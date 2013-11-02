//-----------------------------------------------------------------------
// <copyright file="UnitTestingSprite.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLTests
{
    /// <summary>
    /// A sprite with as few dependencies as possible,
    /// to be used in unit testing.
    /// </summary>
    public class UnitTestingSprite : Sprite
    {
        /// <summary>
        /// Gets the name of the category that this tile is
        /// categorized within in the level editor.
        /// </summary>
        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        /// <summary>
        /// Initializes this sprite.
        /// </summary>
        /// <param name="owner">The Level that owns this sprite.</param>
        public override void Initialize(Level owner)
        {
            base.Initialize(owner);
            this.Size = new Vector2(16f, 16f);
        }

        /// <summary>
        /// Loads the content for this sprite.
        /// </summary>
        public override void LoadContent()
        {
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public override void Draw()
        {
        }

        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
        }

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
        }
    }
}
