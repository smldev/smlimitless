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
    public class Level
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

            int j = 0;
            for (int i = 0; i < 800; i += 16)
            {
                TestTile tile = new TestTile();
                this.tiles.Add(tile);
                this.tiles[j].Initialize(this);
                this.tiles[j].Position = new Vector2(i, 432f);
                this.quadTree.Add(tile);
                j++;
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
        /// Checks and resolves the collisions in this level. Temporary.
        /// </summary>
        public void CheckCollision()
        {
            // First, check sprite-tile collisions
            foreach (Sprite sprite in this.sprites)
            {
                var collidableTiles = this.quadTree.GetCollidableTiles(sprite);
                List<Intersection> intersections = new List<Intersection>();
                Dictionary<Tile, Intersection> collidingTiles = new Dictionary<Tile, Intersection>();
                bool spriteIsOnGround = false;
                bool spriteIsEmbedded = false;

                foreach (Tile tile in collidableTiles)
                {
                    if (tile.Collision == TileCollisionType.Solid)
                    {
                        var intersection = new Intersection(sprite.Hitbox, tile.Hitbox);
                        if (intersection.Depth.Abs().GreaterThanOrEqualTo(sprite.Size))
                        {
                            sprite.IsEmbedded = true;
                            return;
                        }

                        if (intersection.IsIntersecting)
                        {
                            intersections.Add(intersection);
                            collidingTiles.Add(tile, intersection);
                        }
                    }
                    else if (tile.Collision == TileCollisionType.TopSolid && sprite.Velocity.Y > 0f)
                    {
                        var intersection = new Intersection(sprite.Hitbox, tile.Hitbox);
                        if (intersection.IsIntersecting && intersection.Direction == Direction.Up)
                        {
                            intersections.Add(intersection);
                            collidingTiles.Add(tile, intersection);
                        }
                    }
                }

                if (sprite.IsEmbedded)
                {
                    if (intersections.Count == 0)
                    {
                        sprite.IsEmbedded = false;
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }

                intersections = Intersection.ConsolidateIntersections(intersections);

                // The distance by which the sprite will have to be moved
                Vector2 resolution = Vector2.Zero;
                foreach (Intersection intersection in intersections)
                {
                    switch (intersection.Direction)
                    {
                        case Direction.Up:
                            spriteIsOnGround = true;
                            if (resolution.Y > 0) 
                            {
                                // If we've already moved down
                                spriteIsEmbedded = true;
                            }
                            else if (resolution.Y == 0)
                            {
                                resolution.Y += intersection.GetIntersectionResolution().Y;
                                debugText += "Up ";
                            }

                            break;
                        case Direction.Down:
                            if (resolution.Y < 0) 
                            {
                                // If we've already moved up
                                spriteIsEmbedded = true;
                            }
                            else if (resolution.Y == 0)
                            {
                                resolution.Y += intersection.GetIntersectionResolution().Y;
                                debugText += "Down ";
                            }

                            break;
                        case Direction.Left:
                            if (resolution.X > 0) 
                            {
                                // If we've already moved right
                                spriteIsEmbedded = true;
                            }
                            else if (resolution.X == 0)
                            {
                                resolution.X += intersection.GetIntersectionResolution().X;
                                debugText += "Left ";
                            }

                            break;
                        case Direction.Right:
                            if (resolution.X < 0) 
                            {
                                // If we've already moved left
                                spriteIsEmbedded = true;
                            }
                            else if (resolution.X == 0)
                            {
                                resolution.X += intersection.GetIntersectionResolution().X;
                                debugText += "Right ";
                            }

                            break;
                    }
                }

                sprite.IsOnGround = spriteIsOnGround;
                sprite.IsEmbedded = spriteIsEmbedded;
                if (!sprite.IsEmbedded)
                {
                    sprite.Position += resolution;
                }

                foreach (var collision in collidingTiles)
                {
                    collision.Key.HandleCollision(sprite, collision.Value);
                    sprite.HandleTileCollision(collision.Key, collision.Value);
                }
            }
        }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw()
        {
            this.tiles.ForEach(t => t.Draw());
            this.sprites.ForEach(s => s.Draw());
            //// GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, debugText, new Vector2(16, 36), Color.White);
            this.debugText = "";
            this.quadTree.Draw();
        }
    }
}
