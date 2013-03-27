using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    public class Level
    {
        private List<Tile> tiles;
        private List<Sprite> sprites;

        private MouseFollowSprite mouseSprite;

        private QuadTree quadTree;

        private float gravityAcceleration = 20000f;
        [Description("How fast sprites in this level fall.")]
        public float GravityAcceleration
        {
            get { return gravityAcceleration; }
            set { gravityAcceleration = value; }
        }

        private string debugText = "";

        public Level()
        {
            tiles = new List<Tile>();
            sprites = new List<Sprite>();
            mouseSprite = new MouseFollowSprite();

            quadTree = new QuadTree(new Vector2(64f, 64f));
        }

        public void Initialize()
        {
            TestSprite sprite = new TestSprite();
            sprites.Add(sprite);
            sprites.Add(mouseSprite);
            sprites[0].Initialize(this);
            sprites[1].Initialize(this);
            quadTree.Add(sprite);
            quadTree.Add(mouseSprite);

            int j = 0;
            for (int i = 0; i < 800; i += 16)
            {
                TestTile tile = new TestTile();
                tiles.Add(tile);
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 432f);
                quadTree.Add(tile);
                j++;
            }

            for (int i = 0; i < 800; i += 16)
            {
                tiles.Add(new TestTile2());
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 448f);
                j++;
            }

            for (int i = 0; i < 800; i += 16)
            {
                tiles.Add(new TestTile2());
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 464f);
                j++;
            }
        }

        public void LoadContent()
        {
            tiles.ForEach(t => t.LoadContent());
            sprites.ForEach(s => s.LoadContent());
        }

        public void Update()
        {
            tiles.ForEach(t => t.Update());
            sprites.ForEach(s => s.Update());
            quadTree.Update();
            CheckCollision();
        }

        public void CheckCollision()
        {
            // First, check sprite-tile collisions
            foreach (Sprite sprite in sprites)
            {
                var collidableTiles = quadTree.GetCollidableTiles(sprite);
                List<Intersection> intersections = new List<Intersection>();
                List<Tile> collidingTiles = new List<Tile>();

                foreach (Tile tile in collidableTiles)
                {
                    if (tile.Collision == TileCollisionType.Solid)
                    {
                        var intersection = new Intersection(sprite.Hitbox, tile.Hitbox);
                        if (intersection.IsIntersecting)
                        {
                            intersections.Add(intersection);
                            collidingTiles.Add(tile);
                        }
                    }
                    else if (tile.Collision == TileCollisionType.TopSolid)
                    {
                        var intersection = new Intersection(sprite.Hitbox, tile.Hitbox);
                        if (intersection.IsIntersecting && intersection.Direction == Direction.Up)
                        {
                            intersections.Add(intersection);
                            collidingTiles.Add(tile);
                        }
                    }
                }

                intersections = Intersection.ConsolidateIntersections(intersections);

                // The distance by which the sprite will have to be moved
                Vector2 resolution = Vector2.Zero;
                foreach (Intersection intersection in intersections)
                {
                    bool spriteIsOnGround = false;
                    switch (intersection.Direction)
                    {
                        case Direction.Up:
                            spriteIsOnGround = true;
                            if (resolution.Y > 0) // If we've already moved down
                            {
                                sprite.IsEmbedded = true;
                            }
                            else if (resolution.Y == 0)
                            {
                                resolution.Y += intersection.GetIntersectionResolution().Y;
                                debugText += "Up ";
                            }
                            break;
                        case Direction.Down:
                            if (resolution.Y < 0) // If we've already moved up
                            {
                                sprite.IsEmbedded = true;
                            }
                            else if (resolution.Y == 0)
                            {
                                resolution.Y += intersection.GetIntersectionResolution().Y;
                                debugText += "Down ";
                            }
                            break;
                        case Direction.Left:
                            if (resolution.X > 0) // If we've already moved right
                            {
                                sprite.IsEmbedded = true;
                            }
                            else if (resolution.X == 0)
                            {
                                resolution.X += intersection.GetIntersectionResolution().X;
                                debugText += "Left ";
                            }
                            break;
                        case Direction.Right:
                            if (resolution.X < 0) // If we've already moved left
                            {
                                sprite.IsEmbedded = true;
                            }
                            else if (resolution.X == 0)
                            {
                                resolution.X += intersection.GetIntersectionResolution().X;
                                debugText += "Right ";
                            }
                            break;
                    }

                    sprite.IsOnGround = spriteIsOnGround;
                    if (sprite.IsEmbedded) break; // We're not resolving anything since we're embedded, but...
                }
                sprite.Position += resolution;

                collidingTiles.ForEach(t => t.HandleCollision(sprite)); // ... we do want to call the collision handler.
            }
        }

        public void Draw()
        {
            tiles.ForEach(t => t.Draw());
            sprites.ForEach(s => s.Draw());
            debugText = "";
            quadTree.Draw();
        }
    }
}
