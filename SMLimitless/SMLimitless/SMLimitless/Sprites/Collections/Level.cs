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
        /*
         * TODO: Get rid of this whole class
         * TODO: Create a new level type that uses layers as the primary way of managing tiles/sprites
         * TODO: Keep the quadtrees, keep the separate lists of tiles and sprites, and keep the update/draw code here
         * TODO: Make a way to set the background color (maybe also look into gradients)
         *      http://mattryder.wordpress.com/2011/02/02/xna-creating-a-gradient-styled-texture2d/
         *      http://stackoverflow.com/questions/16571850/making-a-gradient-between-2-colors-in-xna
         *      http://gamedev.stackexchange.com/questions/773/what-are-some-good-resources-for-learning-hlsl
         *      http://blogs.msdn.com/b/shawnhar/archive/2010/04/05/spritebatch-and-custom-shaders-in-xna-game-studio-4-0.aspx
         */

        /// <summary>
        /// Temporary. Keeps track of the number of frames
        /// until the next sprite can be dropped. This field is
        /// set to 5 when a sprite is placed in a level via left-clicking.
        /// </summary>
        private int framesUntilNextSprite = 0;

        /// <summary>
        /// A collection of all the tiles in this level.
        /// </summary>
        private List<Tile> tiles;

        /// <summary>
        /// A collection of all the sprites in this level.
        /// </summary>
        internal List<Sprite> Sprites;

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
        /// Text to display onscreen.
        /// </summary>
        private string displayText = "";

        /// <summary>
        /// The number of frames left to draw the displayText field.
        /// </summary>
        private int displayTextFrames = 0;

        /// <summary>
        /// A backing field for the GravityAcceleration property.
        /// </summary>
        private float gravityAcceleration = 256f;

        private Layer layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.tiles = new List<Tile>();
            this.Sprites = new List<Sprite>();
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

            SimplePlayer player = new SimplePlayer() { Position = new Vector2(0f, 300f) };
            player.Initialize(this);
            this.AddSprite(player);

            for (int x = 0; x <= 800; x += 16)
            {
                TestTile tile = new TestTile() { Position = new Vector2(x, 400f) };
                tile.Initialize(this, "");
                this.AddTile(tile);
            }

            for (int x = 0; x <= 800; x += 16)
            {
                for (int y = 416; y <= 480; y += 16)
                {
                    TestTile2 tile = new TestTile2() { Position = new Vector2(x, y) };
                    tile.Initialize(this, "");
                    this.AddTile(tile);
                }
            }

            this.layer = new Layer(this);
        }

        /// <summary>
        /// Loads the content for this level.
        /// </summary>
        public void LoadContent()
        {
            this.tiles.ForEach(t => t.LoadContent());
            this.Sprites.ForEach(s => s.LoadContent());
        }

        /// <summary>
        /// Adds a sprite to this level.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public void AddSprite(Sprite sprite)
        {
            this.Sprites.Add(sprite);
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
        /// Removes a sprite from this level.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void RemoveSprite(Sprite sprite)
        {
            this.Sprites.Remove(sprite);
            this.quadTree.Remove(sprite);
        }

        /// <summary>
        /// Removes a tile from this level.
        /// </summary>
        /// <param name="tile">The tile to remove.</param>
        public void RemoveTile(Tile tile)
        {
            this.tiles.Remove(tile);
            this.quadTree.Remove(tile);
        }

        /// <summary>
        /// Returns a sprite at a given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The sprite at the position, or null if there is no sprite at the position.</returns>
        public Sprite GetSpriteAtPosition(Vector2 position)
        {
            Vector2 positionCell = this.quadTree.GetCellNumberAtPosition(position);
            List<Sprite> spritesInPosCell = this.quadTree.GetSpritesInCell(positionCell);

            if (spritesInPosCell != null)
            {
                foreach (Sprite sprite in spritesInPosCell)
                {
                    if (sprite.Hitbox.Intersects(position))
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a tile at a given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The tile at the position, or null if there is no tile at the position.</returns>
        public Tile GetTileAtPosition(Vector2 position)
        {
            Vector2 positionCell = this.quadTree.GetCellNumberAtPosition(position);
            List<Tile> tilesInPosCell = this.quadTree.GetTilesInCell(positionCell);

            if (tilesInPosCell != null)
            {
                foreach (Tile tile in tilesInPosCell)
                {
                    if (tile.Hitbox.Bounds.IntersectsIncludingEdges(position))
                    {
                        return tile;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Updates this level.
        /// </summary>
        public void Update()
        {
            if (this.framesUntilNextSprite > 0)
            {
                this.framesUntilNextSprite--;
            }

            // TEMPORARY: this code adds sprites and blocks and left- and right-clicks
            // this is only to more easily test collision
            if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
            {
                if (this.framesUntilNextSprite == 0 && !InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    // Add a sprite to the level.
                    Vector2 mousePosition = new Vector2(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y);
                    mousePosition = new Vector2(mousePosition.X - 8f, mousePosition.Y - 8f);
                    TestSprite testSprite = new TestSprite() { Position = mousePosition };
                    testSprite.Initialize(this);
                    testSprite.LoadContent();
                    this.AddSprite(testSprite);
                    this.framesUntilNextSprite = 5;
                }
                else if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    Sprite spriteUnderCursor = this.GetSpriteAtPosition(new Vector2(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y));
                    if (spriteUnderCursor != null)
                    {
                        this.RemoveSprite(spriteUnderCursor);
                    }
                }
            }
            else if (InputManager.IsCurrentMousePress(MouseButtons.RightButton))
            {
                if (!InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    // Add a tile to the level.
                    Vector2 mousePosition = new Vector2(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y);

                    if (this.GetTileAtPosition(mousePosition) == null)
                    {
                        Vector2 tilePosition = mousePosition.FloorDivide(16f) * 16f;
                        TestTile3 tile = new TestTile3() { Position = tilePosition };
                        tile.Initialize(this, "");
                        tile.LoadContent();
                        this.AddTile(tile);
                        this.layer.AddTile(tile);
                    }
                }
                else
                {
                    // Remove a tile from the level.
                    Vector2 mousePosition = new Vector2(InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y);
                    Tile tile = this.GetTileAtPosition(mousePosition);

                    if (tile != null)
                    {
                        this.RemoveTile(tile);
                        this.layer.RemoveTile(tile);
                    }
                }
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                // Clear all sprites in the level.
                while (this.Sprites.Count != 0)
                {
                    this.RemoveSprite(this.Sprites[0]);
                }

                var tilesToRemove = this.tiles.Where(t => t is TestTile3).ToList();
                foreach (Tile tile in tilesToRemove)
                {
                    this.RemoveTile(tile);
                }
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.R))
            {
                // Reset the level.
                while (this.Sprites.Count != 0)
                {
                    this.RemoveSprite(this.Sprites[0]);
                }

                while (this.tiles.Count != 0)
                {
                    this.RemoveTile(this.tiles[0]);
                }

                this.Initialize();
                this.LoadContent();
            }

            if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.T))
            {
                Random random = new Random();

                switch (random.Next(0, 3))
                {
                    case 0:
                        // Give all sprites a boost.
                        this.Sprites.ForEach(s => s.Velocity = new Vector2(s.Velocity.X, s.Velocity.Y - random.Next(0, 100)));

                        this.displayText = "Jump!";
                        this.displayTextFrames = 120;
                        break;
                    case 1:
                        // Randomly remove tiles.
                        List<Tile> tilesToRemove = new List<Tile>();
                        foreach (Tile tile in this.tiles)
                        {
                            if (random.Next(0, 50) == 0)
                            {
                                // 1-in-50 chance
                                tilesToRemove.Add(tile);
                            }
                        }

                        tilesToRemove.ForEach(t => this.RemoveTile(t));

                        this.displayText = "Remove!";
                        this.displayTextFrames = 120;
                        break;
                    case 2:
                        // Add a few sprites with random vertical velocities.
                        int spriteCount = random.Next(1, 11);
                        for (int i = 0; i < spriteCount; i++)
                        {
                            TestSprite sprite = new TestSprite() { Position = new Vector2(random.Next(0, 785), random.Next(-400, 1)) };
                            sprite.Velocity = new Vector2(sprite.Velocity.X, random.Next(-20, 0));
                            sprite.Initialize(this);
                            sprite.LoadContent();

                            this.AddSprite(sprite);
                        }

                        this.displayText = "Drop!";
                        this.displayTextFrames = 120;
                        break;
                    default:
                        break;
                }
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.C))
            {
                this.layer.Translate(InputManager.MousePosition);
            }

            this.tiles.ForEach(t => t.Update());
            this.Sprites.ForEach(s => s.Update());
            this.quadTree.Update();
            this.CheckCollision();
        }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw()
        {
            this.tiles.ForEach(t => t.Draw());
            this.Sprites.ForEach(s => s.Draw());
            ////this.debugText.DrawString(new Vector2(16f, 16f), Color.White);
            this.debugText = "";
            this.layer.Draw(Color.Yellow);

            if (this.displayTextFrames != 0)
            {
                this.displayText.DrawString(new Vector2(16f, 40f), Color.White);
                this.displayTextFrames--;
            }
        }
    }
}
