//-----------------------------------------------------------------------
// <copyright file="SlopedTestTile1.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Graphics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Testing;

namespace SmlSample
{
    /// <summary>
    /// A sloped test tile.
    /// </summary>
    public class SlopedTestTile1 : SlopedTile
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
        /// The graphics of this tile.
        /// </summary>
        private StaticGraphicsObject graphics;

        public SlopedTestTile1()
        {
            this.Size = new Vector2(16f, 16f);
            this.graphics = new StaticGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\gfx\\smw_grass_slope1.png");
            this.graphics.Load(absolute);
        }

        /// <summary>
        /// Initializes this tile.
        /// </summary>
        /// <param name="owner">The Level that owns this tile.</param>
        /// <param name="contentResourceName">The name of the content resource that is used for this tile's graphics.</param>
        public void Initialize(TestLevel owner, string contentResourceName)
        {
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

        public override void DeserializeCustomObjects(SMLimitless.Sprites.Assemblies.JsonHelper customObjects)
        {
        }

        public override object GetCustomSerializableObjects()
        {
            return null;
        }
    }
}
