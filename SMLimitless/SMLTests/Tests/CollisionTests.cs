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
using SMLimitless;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLTests
{
    /// <summary>
    /// Contains tests for collision detection and resolution.
    /// </summary>
    [TestFixture]
    public class CollisionTests
    {
        /// <summary>
        /// Tests that a rectangle intersecting another will resolve left.
        /// </summary>
        [Test]
        public void RectangleLeftwardCollisionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(8f, 0f, 10f, 10f);

            // a.Right is between b.Left and b.Right, direction is Left, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
        }

        /// <summary>
        /// Tests that a rectangle intersecting another will resolve right.
        /// </summary>
        [Test]
        public void RectangleRightwardCollisionTest()
        {
            BoundingRectangle a = new BoundingRectangle(8f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(0f, 0f, 10f, 10f);

            // a.Left is between b.Left and b.Right, direction is right, distance is 2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(2f, 0f), resolution);
        }

        /// <summary>
        /// Tests that a rectangle intersecting another will resolve up.
        /// </summary>
        [Test]
        public void RectangleUpwardCollisionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(0f, 8f, 10f, 10f);

            // a.Bottom is betwen b.Top and b.Bottom, direction is up, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(0f, -2f), resolution);
        }

        /// <summary>
        /// Tests that a rectangle intersecting another will resolve down.
        /// </summary>
        [Test]
        public void RectangleDownwardCollisionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 8f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(0f, 0f, 10f, 10f);

            // a.Top is between b.Top and b.Bottom, direction is down, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(0f, 2f), resolution);
        }

        /// <summary>
        /// Tests that the shallowest edge theorem works properly in a horizontal rectangle-rectangle intersection.
        /// </summary>
        [Test]
        public void RectangleShallowestEdgeHorizontalTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(8f, 6f, 10f, 10f);

            // x < y, shallowest edge is horizontal, direction is left, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
        }

        /// <summary>
        /// Tests that the shallowest edge theorem works properly in a vertical rectangle-rectangle intersection.
        /// </summary>
        [Test]
        public void RectangleShallowestEdgeVerticalTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(6f, 8f, 10f, 10f);

            // y < x, shallowest edge is vertical, direction is up, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(0f, -2f), resolution);
        }

        /// <summary>
        /// Tests that the shallowest edge theorem works properly in a equal rectangle-rectangle intersection.
        /// </summary>
        [Test]
        public void RectangleBothEdgesEqualTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            BoundingRectangle b = new BoundingRectangle(8f, 8f, 10f, 10f);

            // edges equal, resolve horizontally, direction is left, distance is -2f
            Vector2 resolution = b.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
        }

        /// <summary>
        /// Tests rectangle-triangle resolution along the sloped side.
        /// </summary>
        [Test]
        public void TriangleResolutionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 100f, 100f);
            RightTriangle r = new RightTriangle(new BoundingRectangle(0f, 48f, 100f, 100f), RtSlopedSides.TopLeft);
            Vector2 resolution = r.GetCollisionResolution(a);
            a = new BoundingRectangle(resolution.X, resolution.Y, 100f, 100f);
            Assert.AreEqual(new Vector2(0f, -2f), new Vector2(a.X, a.Y));
        }

        /// <summary>
        /// Tests rectangle-triangle resolution along the straight side.
        /// </summary>
        [Test]
        public void TriangleStraightEdgeResolutionTest()
        {
            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
            RightTriangle r = new RightTriangle(new BoundingRectangle(-8f, 0f, 10f, 10f), RtSlopedSides.TopLeft);

            // Direction is right, distance is 2f
            Vector2 resolution = r.GetCollisionResolution(a);
            Assert.AreEqual(new Vector2(2f, 0f), resolution);
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

            UnitTestingSprite sprite = new UnitTestingSprite { Position = Vector2.Zero };
            sprite.Initialize(level);
            level.AddSprite(sprite);

            UnitTestingTile tile = new UnitTestingTile { Position = new Vector2(0f, 14f) };
            tile.Initialize(level);
            level.AddTile(tile);

            level.Update();
            Assert.AreEqual(new Vector2(0f, -2f), sprite.Position);
        }
    }
}
