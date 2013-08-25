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
            TestSprite sprite = new TestSprite();
            sprite.Position = new Vector2(96f, 72f);
            this.sprites.Add(sprite);
            this.sprites.Add(this.mouseSprite);
            this.sprites[0].Initialize(this);
            this.sprites[1].Initialize(this);
            this.quadTree.Add(sprite);
            this.quadTree.Add(this.mouseSprite);

            Random random = new Random();
            int total = random.Next(0, 100);
            for (int i = 0; i < 0; i++)
            {
                TestSprite newSprite = new TestSprite();
                newSprite.Position = new Vector2(random.Next(0, 600), 100f);
                this.sprites.Add(newSprite);
                newSprite.Initialize(this);
                this.quadTree.Add(newSprite);
                newSprite.Velocity += new Vector2(random.Next(0, 100), random.Next(0, 100));
            }

            int j = 0;
            for (int i = 0; i < 400; i += 16)
            {
                TestTile tile = new TestTile();
                this.tiles.Add(tile);
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 432f);
                this.quadTree.Add(tile);
                j++;
            }

            for (int i = 400; i < 800; i += 32)
            {
                SlopedTestTile1 tile1 = new SlopedTestTile1();
                SlopedTestTile2 tile2 = new SlopedTestTile2();
                TestTile4 tile3 = new TestTile4();
                TestTile5 tile4 = new TestTile5();
                this.tiles.Add(tile1);
                this.tiles.Add(tile2);
                this.tiles.Add(tile3);
                this.tiles.Add(tile4);
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 416f);
                j++;
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i + 16f, 416f);
                j++;
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 432f);
                j++;
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i + 16f, 432f);
                j++;
                this.quadTree.Add(tile1);
                this.quadTree.Add(tile2);
            }

            for (int i = 0; i < 800; i += 16)
            {
                this.tiles.Add(new TestTile2());
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 448f);
                j++;
            }

            for (int i = 0; i < 800; i += 16)
            {
                this.tiles.Add(new TestTile2());
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 464f);
                j++;
            }

            for (int x = 96; x < 160; x += 16)
            {
                TestTile3 tile = new TestTile3();
                this.tiles.Add(tile);
                tile.Initialize(this);
                tile.Position = new Vector2(x, 96);
                this.quadTree.Add(tile);
                j++;
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
            this.debugText.DrawString(new Vector2(16f, 16f), Color.White);
            this.debugText = "";
            this.quadTree.Draw();
        }
    }
}
