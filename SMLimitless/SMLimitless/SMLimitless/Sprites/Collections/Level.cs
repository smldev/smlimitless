//-----------------------------------------------------------------------
// <copyright file="Level.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A game-play area consisting of a collection
    /// of sprites, tiles, and utility objects.
    /// </summary>
    public sealed partial class Level
    {
        /// <summary>
        /// A value indicating whether this level should be running. Temporary.
        /// </summary>
        private bool run = true;

        /// <summary>
        /// A collection of all the tiles in this level.
        /// </summary>
        private List<Tile> tiles;

        /// <summary>
        /// A collection of all the sprites in this level.
        /// </summary>
        private List<Sprite> sprites;

        /// <summary>
        /// Testing sprite.
        /// </summary>
        private MouseFollowSprite mouseSprite;

        /// <summary>
        /// A lazy QuadTree that divides up the level into
        /// equally-sized cells into which tiles and sprites are
        /// divided into.
        /// </summary>
        private QuadTree quadTree;

        /// <summary>
        /// Debug text printed to screen. Temporary.
        /// </summary>
        private string debugText = "";

        /// <summary>
        /// A backing field for the GravityAcceleration property.
        /// </summary>
        private float gravityAcceleration = 128f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.tiles = new List<Tile>();
            this.sprites = new List<Sprite>();
            this.mouseSprite = new MouseFollowSprite();

            this.quadTree = new QuadTree(new Vector2(64f, 64f));
        }

        /// <summary>
        /// Gets or sets a value indicating how much acceleration is caused by gravity.
        /// Measured in pixels per second per second.
        /// </summary>
        [Description("How fast sprites in this level fall.")]
        public float GravityAcceleration
        {
            get { return this.gravityAcceleration; }
            set { this.gravityAcceleration = value; }
        }

        /// <summary>
        /// Initializes this level.
        /// </summary>
        public void Initialize()
        {
            TestSprite testSprite = new TestSprite() { Position = new Vector2(0f, 300f) };
            testSprite.Initialize(this);
            this.AddSprite(testSprite);

            for (int x = 0; x <= 800; x += 16)
            {
                TestTile tile = new TestTile() { Position = new Vector2(x, 400f) };
                tile.Initialize(this);
                this.AddTile(tile);
            }

            for (int x = 0; x <= 800; x += 16)
            {
                for (int y = 416; y <= 480; y += 16)
                {
                    TestTile2 tile = new TestTile2() { Position = new Vector2(x, y) };
                    tile.Initialize(this);
                    this.AddTile(tile);
                }
            }
        }

        /// <summary>
        /// Loads the content for this level.
        /// </summary>
        public void LoadContent()
        {
            this.tiles.ForEach(t => t.LoadContent());
            this.sprites.ForEach(s => s.LoadContent());
        }

        /// <summary>
        /// Adds a sprite to this level.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public void AddSprite(Sprite sprite)
        {
            this.sprites.Add(sprite);
            this.quadTree.Add(sprite);
        }

        /// <summary>
        /// Adds a tile to this level.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        public void AddTile(Tile tile)
        {
            this.tiles.Add(tile);
            this.quadTree.Add(tile);
        }

        /// <summary>
        /// Updates this level.
        /// </summary>
        public void Update()
        {
            if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                this.run = !this.run;
            }

            // TEMPORARY: this code adds sprites and blocks and left- and right-clicks
            // this is only to more easily test collision
            if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
            {
                // Add a sprite to the level.
                Vector2 mousePosition = new Vector2(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y);
                mousePosition = new Vector2(mousePosition.X - 8f, mousePosition.Y - 8f);
                TestSprite testSprite = new TestSprite() { Position = mousePosition };
                testSprite.Initialize(this);
                testSprite.LoadContent();
                this.AddSprite(testSprite);
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.X))
            {
                foreach (Sprite sprite in this.sprites)
                {
                    sprite.Velocity = new Vector2(sprite.Velocity.X, sprite.Velocity.Y - 50f);
                }
            }

            if (this.run)
            {
                this.tiles.ForEach(t => t.Update());
                this.sprites.ForEach(s => s.Update());
                this.quadTree.Update();
                this.CheckCollision();
            }
        }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw()
        {
            this.tiles.ForEach(t => t.Draw());
            this.sprites.ForEach(s => s.Draw());
            this.sprites[0].Hitbox.DrawOutline(Color.White);
            this.debugText.DrawString(new Vector2(16f, 16f), Color.White);
            this.debugText = "";
            //this.quadTree.Draw();
        }
    }
}
