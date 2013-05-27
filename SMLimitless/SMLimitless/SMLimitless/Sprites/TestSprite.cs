//-----------------------------------------------------------------------
// <copyright file="TestSprite.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// A testing sprite.
    /// </summary>
    public class TestSprite : Sprite
    {
        /// <summary>
        /// The graphics for this object.
        /// </summary>
        private AnimatedGraphicsObject graphics;

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="owner">The Level that owns this object.</param>
        public override void Initialize(Level owner)
        {
            this.graphics = new AnimatedGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\gfx\\smb3_goomba.png");
            this.graphics = (AnimatedGraphicsObject)GraphicsManager.LoadGraphicsObject(absolute);
            this.Size = new Vector2(16, 16);
            base.Initialize(owner);
        }

        /// <summary>
        /// Loads the content for this object.
        /// </summary>
        public override void LoadContent()
        {
            this.graphics.LoadContent();
        }

        /// <summary>
        /// Updates this object.
        /// </summary>
        public override void Update()
        {
            this.graphics.Update();
            this.Velocity = new Vector2(30f, Velocity.Y);
            base.Update();
        }

        /// <summary>
        /// Draws this object.
        /// </summary>
        public override void Draw()
        {
            this.graphics.Draw(this.Position, Color.White, false);
        }

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleSpriteCollision(Sprite sprite, Intersection intersect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleTileCollision(Tile tile, Intersection intersect)
        {
        }
    }
}
