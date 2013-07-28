//-----------------------------------------------------------------------
// <copyright file="SlopedTestTile2.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// A sloped test tile.
    /// </summary>
    public class SlopedTestTile2 : SlopedTile
    {
        /// <summary>
        /// The graphics of this tile.
        /// </summary>
        private StaticGraphicsObject graphics;

        /// <summary>
        /// Initializes this tile.
        /// </summary>
        /// <param name="owner">The Level that owns this tile.</param>
        public override void Initialize(Level owner)
        {
            this.Size = new Vector2(16f, 16f);
            this.graphics = new StaticGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\gfx\\smw_grass_slope2.png");
            this.graphics.Load(absolute);
            base.Initialize(owner);
        }

        /// <summary>
        /// Loads the content for this tile.
        /// </summary>
        public override void LoadContent()
        {
            this.graphics.LoadContent();
        }

        /// <summary>
        /// Updates this tile.
        /// </summary>
        public override void Update()
        {
        }

        /// <summary>
        /// Handles a collision between this tile and a sprite.
        /// </summary>
        /// <param name="sprite">The sprite that collided with this tile.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleCollision(Sprite sprite, Vector2 intersect)
        {
        }

        /// <summary>
        /// Draws this tile.
        /// </summary>
        public override void Draw()
        {
            this.graphics.Draw(this.Position, Color.White);
        }
    }
}
