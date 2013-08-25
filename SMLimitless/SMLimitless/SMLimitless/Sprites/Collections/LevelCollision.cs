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
    public sealed partial class Level
    {
        /// <summary>
        /// Checks and resolves the collisions in this level.
        /// </summary>
        public void CheckCollision()
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            stopwatch.Stop();
            this.debugText = stopwatch.Elapsed.ToString();
        }

        private bool SpriteIsIntersecting(Sprite sprite, List<Tile> collidableTiles, List<SlopedTile> collidableSlopes)
        {
            foreach (Tile tile in collidableTiles)
            {
                if (tile.Hitbox.Intersects(sprite.Hitbox))
                {
                    return true;
                }
            }

            foreach (SlopedTile slope in collidableSlopes)
            {
                if (slope.Hitbox.Intersects(sprite.Hitbox))
                {
                    return true;
                }
            }

            return false;
        }

        private void CorrectCollision(Sprite sprite, Vector2 resolution, bool isHorizontal)
        {

        }
    }
}
