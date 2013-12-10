//-----------------------------------------------------------------------
// <copyright file="TestLevel.cs" company="The Limitless Development Team">
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
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.Testing
{
    /// <summary>
    /// A game-play area consisting of a collection
    /// of sprites, tiles, and utility objects.
    /// </summary>
    public sealed class TestLevel
    {
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
        private List<Sprite> sprites;

        /// <summary>
        /// Testing sprite.
        /// </summary>
        private Sprite mouseSprite;

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

#pragma warning disable 414
        /// <summary>
        /// Text to display onscreen.
        /// </summary>
        private string displayText = "";

        /// <summary>
        /// The number of frames left to draw the displayText field.
        /// </summary>
        private int displayTextFrames = 0;
#pragma warning restore 414
        /// <summary>
        /// A backing field for the GravityAcceleration property.
        /// </summary>
        private float gravityAcceleration = 256f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLevel"/> class.
        /// </summary>
        public TestLevel()
        {
            AssemblyManager.LoadAssembly(@"TestPackage\SmlSample.dll");

            this.tiles = new List<Tile>();
            this.sprites = new List<Sprite>();
            this.mouseSprite = AssemblyManager.GetSpriteByFullName("SmlSample.MouseFollowSprite");

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
            Sprite testSprite = AssemblyManager.GetSpriteByFullName("SmlSample.TestSprite");
            testSprite.Position = new Vector2(0f, 300f);
            testSprite.Initialize(null);
            this.AddSprite(testSprite);

            Sprite player = AssemblyManager.GetSpriteByFullName("SmlSample.SimplePlayer");
            player.Position = new Vector2(0f, 300f);
            player.Initialize(null);
            this.AddSprite(player);

            for (int x = 0; x <= 800; x += 16)
            {
                Tile tile = AssemblyManager.GetTileByFullName("SmlSample.TestTile");
                tile.Position = new Vector2(x, 400f);
                tile.Initialize(null, "");
                this.AddTile(tile);
            }

            for (int x = 0; x <= 800; x += 16)
            {
                for (int y = 416; y <= 480; y += 16)
                {
                    Tile tile = AssemblyManager.GetTileByFullName("SmlSample.TestTile2");
                    tile.Position = new Vector2(x, y);
                    tile.Initialize(null, "");
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
        /// Removes a sprite from this level.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void RemoveSprite(Sprite sprite)
        {
            this.sprites.Remove(sprite);
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
                    Sprite testSprite = (Sprite)AssemblyManager.GetSpriteByFullName("SmlSample.TestSprite");
                    testSprite.Position = mousePosition;
                    testSprite.Initialize(null);
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
                        //TestTile3 tile = new TestTile3() { Position = tilePosition };
                        Tile tile = AssemblyManager.GetTileByFullName("SmlSample.TestTile3");
                        tile.Position = tilePosition;
                        tile.Initialize(null, "");
                        tile.LoadContent();
                        this.AddTile(tile);
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
                    }
                }
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                // Clear all sprites in the level.
                while (this.sprites.Count != 0)
                {
                    this.RemoveSprite(this.sprites[0]);
                }

                var tilesToRemove = this.tiles.Where(t => t.GetType().FullName == "SmlSample.TestTile3").ToList();
                foreach (Tile tile in tilesToRemove)
                {
                    this.RemoveTile(tile);
                }
            }

            if (InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.R))
            {
                // Reset the level.
                while (this.sprites.Count != 0)
                {
                    this.RemoveSprite(this.sprites[0]);
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
                        this.sprites.ForEach(s => s.Velocity = new Vector2(s.Velocity.X, s.Velocity.Y - random.Next(0, 100)));

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
                            Sprite sprite = AssemblyManager.GetSpriteByFullName("SmlSample.TestSprite");
                            sprite.Position = new Vector2(random.Next(0, 785), random.Next(-400, 1));
                            sprite.Velocity = new Vector2(sprite.Velocity.X, random.Next(-20, 0));
                            sprite.Initialize(null);
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

            this.tiles.ForEach(t => t.Update());
            this.sprites.ForEach(s => s.Update());
            this.quadTree.Update();
            this.CheckCollision();
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

            ////if (this.displayTextFrames != 0)
            ////{
            ////    this.displayText.DrawString(new Vector2(16f, 40f), Color.White);
            ////    this.displayTextFrames--;
            ////}
        }

        /// <summary>
        /// Returns the resolution with the smallest Y distance, given a collection of resolutions.
        /// </summary>
        /// <param name="resolutions">A collection of resolutions.</param>
        /// <returns>The resolution with the smallest Y distance.</returns>
        private static Vector2 LeastResolutionByY(IEnumerable<Resolution> resolutions)
        {
            Vector2 smallestSoFar = new Vector2(float.MaxValue, float.MaxValue);

            foreach (Resolution resolution in resolutions)
            {
                if (Math.Abs(resolution.ResolutionDistance.Y) < Math.Abs(smallestSoFar.Y) && resolution.ResolutionDistance.Y != 0f)
                {
                    smallestSoFar = resolution.ResolutionDistance;
                }
            }

            return (smallestSoFar != new Vector2(float.MaxValue)) ? smallestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Returns the resolution with the smallest X distance, given a collection of resolutions.
        /// </summary>
        /// <param name="resolutions">A collection of resolutions.</param>
        /// <returns>The resolution with the smallest X distance.</returns>
        private static Vector2 LeastResolutionByX(IEnumerable<Resolution> resolutions)
        {
            Vector2 smallestSoFar = new Vector2(float.MaxValue, float.MaxValue);

            foreach (Resolution resolution in resolutions)
            {
                if (Math.Abs(resolution.ResolutionDistance.X) < Math.Abs(smallestSoFar.X) && resolution.ResolutionDistance.X != 0f)
                {
                    smallestSoFar = resolution.ResolutionDistance;
                }
            }

            return (smallestSoFar != new Vector2(float.MaxValue)) ? smallestSoFar : Vector2.Zero;
        }

        /// <summary>
        /// Checks and resolves the collisions in this level.
        /// </summary>
        public void CheckCollision()
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            foreach (Sprite sprite in this.sprites)
            {
                if (!sprite.IsActive || sprite.CollisionMode == SpriteCollisionMode.NoCollision)
                {
                    continue;
                }

                if (!sprite.IsEmbedded)
                {
                    List<Tile> collidableTiles = this.quadTree.GetCollidableNormalTiles(sprite);
                    List<Tile> collidingTiles = new List<Tile>();
                    List<Resolution> horizontalResolutions = new List<Resolution>();
                    List<Resolution> verticalResolutions = new List<Resolution>();

                    if (!this.SpriteIsIntersecting(sprite, collidableTiles))
                    {
                        sprite.IsOnGround = false;
                        continue;
                    }

                    foreach (Tile tile in collidableTiles)
                    {
                        Resolution resolution = new Resolution(tile.GetCollisionResolution(sprite));

                        if (resolution == Resolution.Zero)
                        {
                            continue;
                        }
                        else if (resolution.ResolutionDistance.X != 0f)
                        {
                            horizontalResolutions.Add(resolution);
                            collidingTiles.Add(tile);
                        }
                        else if (resolution.ResolutionDistance.Y != 0f)
                        {
                            verticalResolutions.Add(resolution);
                            collidingTiles.Add(tile);
                        }
                    }

                    Vector2 oldPosition = sprite.Position;
                    this.ResolveSpriteVertically(sprite, verticalResolutions);

                    if (this.SpriteIsIntersecting(sprite, collidableTiles))
                    {
                        sprite.Position = oldPosition;
                        this.ResolveSpriteHorizontally(sprite, horizontalResolutions);

                        if (this.SpriteIsIntersecting(sprite, collidableTiles))
                        {
                            sprite.Position = oldPosition;
                            this.ResolveSpriteVertically(sprite, verticalResolutions);
                            this.ResolveSpriteHorizontally(sprite, horizontalResolutions);

                            if (this.SpriteIsIntersecting(sprite, collidableTiles))
                            {
                                sprite.Position = oldPosition;
                                sprite.IsEmbedded = true;
                            }
                            else
                            {
                                sprite.IsOnGround = true;
                            }
                        }
                    }
                    else
                    {
                        sprite.IsOnGround = true;
                    }
                }
                else
                {
                    var collidableTiles = this.quadTree.GetCollidableNormalTiles(sprite);

                    if (this.SpriteIsIntersecting(sprite, collidableTiles))
                    {
                        sprite.Position = new Vector2(sprite.Position.X - 1f, sprite.Position.Y);
                    }
                    else
                    {
                        sprite.IsEmbedded = false;
                    }
                }
            }

            stopwatch.Stop();
        }

        /// <summary>
        /// Determines if a sprite is intersecting any tiles in a given list.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <param name="collidableTiles">The tiles to check against the sprite.</param>
        /// <returns>True if the sprite intersects any of the tiles, false if otherwise.</returns>
        private bool SpriteIsIntersecting(Sprite sprite, List<Tile> collidableTiles)
        {
            foreach (Tile tile in collidableTiles)
            {
                if (tile.Intersects(sprite))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a sprite is intersecting any slopes in a given list.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <param name="collidableSlopes">A collection of slopes to check.</param>
        /// <returns>True if the sprite intersects one or more of the slopes, false if otherwise.</returns>
        private bool SpriteIsIntersecting(Sprite sprite, List<SlopedTile> collidableSlopes)
        {
            foreach (SlopedTile tile in collidableSlopes)
            {
                if (tile.Intersects(sprite))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resolves a sprite vertically, given a list of vertical resolution.
        /// </summary>
        /// <param name="sprite">The sprite to resolve.</param>
        /// <param name="verticalResolutions">A list of all the vertical resolutions.</param>
        private void ResolveSpriteVertically(Sprite sprite, List<Resolution> verticalResolutions)
        {
            Vector2 greatestResolution = TestLevel.LeastResolutionByY(verticalResolutions);
            sprite.Position += greatestResolution;
        }

        /// <summary>
        /// Resolves a sprite horizontally, given a list of horizontal resolutions.
        /// </summary>
        /// <param name="sprite">The sprite to resolve.</param>
        /// <param name="horizontalResolutions">A list of all the horizontal resolutions.</param>
        private void ResolveSpriteHorizontally(Sprite sprite, List<Resolution> horizontalResolutions)
        {
            Vector2 greatestResoluton = TestLevel.LeastResolutionByX(horizontalResolutions);
            sprite.Position += greatestResoluton;
        }
    }
}