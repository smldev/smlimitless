//-----------------------------------------------------------------------
// <copyright file="CollisionTests.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Tests
{
    /// <summary>
    /// Contains tests for collision detection and resolution.
    /// </summary>
    [TestFixture]
    public class CollisionTests
    {
        /// <summary>
        /// Tests rectangle resolution.
        /// </summary>
        [Test]
        public void RectangleResolutionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 100f, 100f);
            BoundingRectangle b = new BoundingRectangle(98f, 96f, 100f, 100f);
            Vector2 resolution = b.GetResolutionDistance(a).GetIntersectionResolution();
            a = new BoundingRectangle(resolution.X, resolution.Y, 100f, 100f);
            Assert.AreEqual(new Vector2(-2f, 0f), new Vector2(a.X, a.Y));
        }

        /// <summary>
        /// Tests rectangle-triangle resolution along the sloped side.
        /// </summary>
        [Test]
        public void TriangleResolutionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 100f, 100f);
            RightTriangle r = new RightTriangle(new BoundingRectangle(0f, 48f, 100f, 100f), RtSlopedSides.TopLeft);
            Vector2 resolution = r.GetResolutionDistance(a).GetIntersectionResolution();
            a = new BoundingRectangle(resolution.X, resolution.Y, 100f, 100f);
            Assert.AreEqual(new Vector2(0f, -2f), new Vector2(a.X, a.Y));
        }

        /// <summary>
        /// Tests that a collision between one sprite and one tile
        /// within a level is properly resolved.
        /// </summary>
        [Test]
        public void LevelCollisionSingleTileTest()
        {
            if (GameServices.GetService<GameTime>() == null)
            {
                GameServices.AddService<GameTime>(new GameTime());
            }

            Level level = new Level();

            TestSprite sprite = new TestSprite { Position = Vector2.Zero };
            sprite.Initialize(level);
            level.AddSprite(sprite);

            TestTile tile = new TestTile { Position = new Vector2(0f, 14f) };
            tile.Initialize(level);
            level.AddTile(tile);

            level.Update();
            Assert.AreEqual(new Vector2(0f, -2f), sprite.Position);
        }
    }
}
