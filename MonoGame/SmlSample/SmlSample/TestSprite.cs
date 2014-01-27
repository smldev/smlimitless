//-----------------------------------------------------------------------
// <copyright file="TestSprite.cs" company="The Limitless Development Team">
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
using SMLimitless.Sprites;
using SMLimitless.Sprites.Testing;

namespace SmlSample
{
    /// <summary>
    /// A testing sprite.
    /// </summary>
    public class TestSprite : Sprite
    {
        /// <summary>
        /// Gets the name of the category that this sprite is
        /// categorized within in the level editor.
        /// </summary>
        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        /// <summary>
        /// The graphics for this object.
        /// </summary>
        private AnimatedGraphicsObject graphics;

        public TestSprite()
        {
            this.Size = new Vector2(16, 16);
            this.IsActive = true;
        }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        /// <param name="owner">The Level that owns this object.</param>
        public void Initialize(TestLevel owner)
        {
        }

        /// <summary>
        /// Loads the content for this object.
        /// </summary>
        public override void LoadContent()
        {
            this.graphics = (AnimatedGraphicsObject)SMLimitless.Content.ContentPackageManager.GetGraphicsResource("smb3_goomba");
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
        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
        }

        public override void DeserializeCustomObjects(SMLimitless.Sprites.Assemblies.JsonHelper customObjects)
        {
        }

        public override object GetCustomSerializableObjects()
        {
            return null;
        }
    }
}
