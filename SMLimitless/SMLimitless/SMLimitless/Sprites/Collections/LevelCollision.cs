//-----------------------------------------------------------------------
// <copyright file="LevelCollision.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A game-play area consisting of a collection
    /// of sprites, tiles, and utility objects.
    /// </summary>
    public sealed partial class Level
    {
        // IMPORTANT - READ THIS
        // This collision code is NOT YET FINAL.
        // It does NOT support collision resolution with slopes.
        // This is just code to make the first release moderately okay.
        // This will be gutted and replaced for subsequent releases.

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
                            sprite.IsEmbedded = true;
                        }
                    }

                    sprite.IsOnGround = true;
                }
                else // if Sprite.IsEmbedded
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
            this.debugText = string.Concat(stopwatch.Elapsed.ToString(), ", ", this.sprites.Count, " sprites, IsOnGround: ", this.sprites[0].IsOnGround.ToString());
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
        /// Resolves a sprite vertically, given a list of vertical resolution distances.
        /// </summary>
        /// <param name="sprite">The sprite to resolve.</param>
        /// <param name="verticalResolutions">A list of all the vertical resolution distances.</param>
        private void ResolveSpriteVertically(Sprite sprite, List<Resolution> verticalResolutions)
        {
            Vector2 greatestResolution = Level.LeastResolutionByY(verticalResolutions);
            sprite.Position += greatestResolution;
        }

        private void ResolveSpriteHorizontally(Sprite sprite, List<Resolution> horizontalResolutions)
        {
            Vector2 greatestResoluton = Level.LeastResolutionByX(horizontalResolutions);
            sprite.Position += greatestResoluton;
        }

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
    }
}
