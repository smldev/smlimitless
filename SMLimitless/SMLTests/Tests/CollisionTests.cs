////-----------------------------------------------------------------------
//// <copyright file="CollisionTests.cs" company="Chris Akridge">
////     Copyrighted unter the MIT Public License.
//// </copyright>
////-----------------------------------------------------------------------
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using NUnit.Framework;
//using SMLimitless;
//using SMLimitless.Physics;
//using SMLimitless.Sprites;
//using SMLimitless.Sprites.Collections;

//namespace SMLTests
//{
//    /// <summary>
//    /// Contains tests for collision detection and resolution.
//    /// </summary>
//    [TestFixture]
//    public class CollisionTests
//    {
//        /// <summary>
//        /// Tests that a rectangle intersecting another will resolve left.
//        /// </summary>
//        [Test]
//        public void RectangleLeftwardCollisionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(8f, 0f, 10f, 10f);

//            // a.Right is between b.Left and b.Right, direction is Left, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
//        }

//        /// <summary>
//        /// Tests that a rectangle intersecting another will resolve right.
//        /// </summary>
//        [Test]
//        public void RectangleRightwardCollisionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(8f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(0f, 0f, 10f, 10f);

//            // a.Left is between b.Left and b.Right, direction is right, distance is 2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(2f, 0f), resolution);
//        }

//        /// <summary>
//        /// Tests that a rectangle intersecting another will resolve up.
//        /// </summary>
//        [Test]
//        public void RectangleUpwardCollisionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(0f, 8f, 10f, 10f);

//            // a.Bottom is betwen b.Top and b.Bottom, direction is up, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(0f, -2f), resolution);
//        }

//        /// <summary>
//        /// Tests that a rectangle intersecting another will resolve down.
//        /// </summary>
//        [Test]
//        public void RectangleDownwardCollisionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 8f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(0f, 0f, 10f, 10f);

//            // a.Top is between b.Top and b.Bottom, direction is down, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(0f, 2f), resolution);
//        }

//        /// <summary>
//        /// Tests that the shallowest edge theorem works properly in a horizontal rectangle-rectangle intersection.
//        /// </summary>
//        [Test]
//        public void RectangleShallowestEdgeHorizontalTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(8f, 6f, 10f, 10f);

//            // x < y, shallowest edge is horizontal, direction is left, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
//        }

//        /// <summary>
//        /// Tests that the shallowest edge theorem works properly in a vertical rectangle-rectangle intersection.
//        /// </summary>
//        [Test]
//        public void RectangleShallowestEdgeVerticalTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(6f, 8f, 10f, 10f);

//            // y < x, shallowest edge is vertical, direction is up, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(0f, -2f), resolution);
//        }

//        /// <summary>
//        /// Tests that the shallowest edge theorem works properly in a equal rectangle-rectangle intersection.
//        /// </summary>
//        [Test]
//        public void RectangleBothEdgesEqualTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            BoundingRectangle b = new BoundingRectangle(8f, 8f, 10f, 10f);

//            // edges equal, resolve horizontally, direction is left, distance is -2f
//            Vector2 resolution = b.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(-2f, 0f), resolution);
//        }

//        /// <summary>
//        /// Tests rectangle-triangle resolution along the sloped side.
//        /// </summary>
//        [Test]
//        public void TriangleResolutionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 100f, 100f);
//            RightTriangle r = new RightTriangle(new BoundingRectangle(0f, 48f, 100f, 100f), RtSlopedSides.TopLeft);
//            Vector2 resolution = r.GetCollisionResolution(a).ResolutionDistance;
//            a = new BoundingRectangle(resolution.X, resolution.Y, 100f, 100f);
//            Assert.AreEqual(new Vector2(0f, -2f), new Vector2(a.X, a.Y));
//        }

//        /// <summary>
//        /// Tests rectangle-triangle resolution along the straight side.
//        /// </summary>
//        [Test]
//        public void TriangleStraightEdgeResolutionTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 10f, 10f);
//            RightTriangle r = new RightTriangle(new BoundingRectangle(-8f, 0f, 10f, 10f), RtSlopedSides.TopLeft);

//            // Direction is right, distance is 2f
//            Vector2 resolution = r.GetCollisionResolution(a).ResolutionDistance;
//            Assert.AreEqual(new Vector2(2f, 0f), resolution);
//        }

//        [Test]
//        public void RectangleIntersectsTest()
//        {
//            BoundingRectangle a = new BoundingRectangle(0f, 0f, 100f, 100f);
//            BoundingRectangle b = new BoundingRectangle(10f, 10f, 100f, 100f);

//            Assert.IsTrue(a.Intersects(b));
//        }

//        [Test]
//        public void TriangleIntersectsTest()
//        {
//            RightTriangle topLeft = new RightTriangle(new BoundingRectangle(0f, 0f, 100f, 100f), RtSlopedSides.TopLeft);
//            RightTriangle topRight = new RightTriangle(new BoundingRectangle(100f, 0f, 100f, 100f), RtSlopedSides.TopRight);
//            RightTriangle bottomLeft = new RightTriangle(new BoundingRectangle(0f, 100f, 100f, 100f), RtSlopedSides.BottomLeft);
//            RightTriangle bottomRight = new RightTriangle(new BoundingRectangle(100f, 100f, 100f, 100f), RtSlopedSides.BottomRight);

//            BoundingRectangle tlA = new BoundingRectangle(0f, 55f, 100f, 100f);
//            BoundingRectangle tlB = new BoundingRectangle(95f, 0f, 100f, 100f);
//            BoundingRectangle tlC = new BoundingRectangle(0f, 95f, 100f, 100f);

//            BoundingRectangle trA = new BoundingRectangle(100f, 55f, 100f, 100f);
//            BoundingRectangle trB = new BoundingRectangle(5f, 0f, 100f, 100f);
//            BoundingRectangle trC = new BoundingRectangle(100f, 95f, 100f, 100f);

//            BoundingRectangle blA = new BoundingRectangle(0f, 155f, 100f, 100f);
//            BoundingRectangle blB = new BoundingRectangle(95f, 100f, 100f, 100f);
//            BoundingRectangle blC = new BoundingRectangle(0f, 95f, 100f, 100f);

//            BoundingRectangle brA = new BoundingRectangle(100f, 155f, 100f, 100f);
//            BoundingRectangle brB = new BoundingRectangle(5f, 100f, 100f, 100f);
//            BoundingRectangle brC = new BoundingRectangle(100f, 95f, 100f, 100f);

//            bool tlIntersects = (topLeft.Intersects(tlA)) && (topLeft.Intersects(tlB)) && (topLeft.Intersects(tlC));
//            bool trIntersects = (topRight.Intersects(trA)) && (topRight.Intersects(trB)) && (topRight.Intersects(trC));
//            bool blIntersects = (bottomLeft.Intersects(blA)) && (bottomLeft.Intersects(blB)) && (bottomLeft.Intersects(blC));
//            bool brIntersects = (bottomRight.Intersects(brA)) && (bottomRight.Intersects(brB)) && (bottomRight.Intersects(brC));

//            Assert.IsTrue(tlIntersects && trIntersects && blIntersects && brIntersects);
//        }

//        private void AddSpriteToLevel(Level level, Sprite sprite, Vector2 position)
//        {
//            sprite.Initialize(level);
//            sprite.Position = position;
//            level.AddSprite(sprite);
//        }

//        private void AddSpritesToLevel(Level level, Sprite[] sprites, Vector2[] positions)
//        {
//            if (sprites.Length != positions.Length)
//            {
//                throw new ArgumentException("AddSpritesToLevel: Number of sprites doesn't equal number of positions.");
//            }

//            for (int i = 0; i < sprites.Length; i++)
//            {
//                sprites[i].Initialize(level);
//                sprites[i].Position = positions[i];
//                level.AddSprite(sprites[i]);
//            }
//        }

//        private void AddTileToLevel(Level level, Tile tile, Vector2 position)
//        {
//            tile.Initialize(level, "");
//            tile.Position = position;
//            level.AddTile(tile);
//        }

//        private void AddTilesToLevel(Level level, Tile[] tiles, Vector2[] positions)
//        {
//            if (tiles.Length != positions.Length)
//            {
//                throw new ArgumentException("AddTilesToLevel: Number of tiles doesn't equal number of positions.");
//            }

//            for (int i = 0; i < tiles.Length; i++)
//            {
//                tiles[i].Initialize(level, "");
//                tiles[i].Position = positions[i];
//                level.AddTile(tiles[i]);
//            }
//        }

//        [Test]
//        public void LevelCollisionNoTileTest()
//        {
//            /* Scenario 0:
//             * A test sprite collides with no tiles and
//             * is not moved by the collision handler.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            level.CheckCollision();

//            Assert.AreEqual(Vector2.Zero, sprite.Position);
//        }

//        /// <summary>
//        /// Tests that a collision between one sprite and one tile
//        /// within a level is properly resolved.
//        /// </summary>
//        [Test]
//        public void LevelCollisionSingleTileVerticalTest()
//        {
//            /* Scenario 1:
//             * A sprite collides with one tile in such a way
//             * that it will be resolved vertically.
//             */
//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            UnitTestingTile tile = new UnitTestingTile();
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTileToLevel(level, tile, new Vector2(0f, 14f));

//            level.CheckCollision();
//            Assert.AreEqual(new Vector2(0f, -2f), sprite.Position);
//        }

//        [Test]
//        public void LevelCollisionSingleTileHorizontalTest()
//        {
//            /* Scenario 2:
//             * A sprite collides with one tile in such a way
//             * that it will be resolved horizontally.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            UnitTestingTile tile = new UnitTestingTile();
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTileToLevel(level, tile, new Vector2(-14f, 8f));

//            level.CheckCollision();
//            Assert.AreEqual(new Vector2(2f, 0f), sprite.Position);
//        }

//        [Test]
//        public void LevelCollisionTileRowTest()
//        {
//            /* Scenario 3:
//             * A sprite collides with a horizontal row of tiles
//             * in such a way that one tile's resolution is vertical,
//             * and the other's is horizontal.
//             * The expected resolution direction is vertical.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new Tile[] { new UnitTestingTile(), new UnitTestingTile() };
//            var positions = new Vector2[] { new Vector2(-1f, 14f), new Vector2(15f, 14f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles, positions);

//            level.CheckCollision();
//            Assert.AreEqual(new Vector2(0f, -2f), sprite.Position);
//        }

//        [Test]
//        public void LevelCollisionTileColumnTest()
//        {
//            /* Scenario 4:
//             * A sprite collides with a vertical column of tiles
//             * in such a way that one tile's resolution is horizontal,
//             * and the other's is vertical.
//             * The expected resolution direction is horizontal.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new Tile[] { new UnitTestingTile(), new UnitTestingTile() };
//            var positions = new Vector2[] { new Vector2(13f, -14f), new Vector2(13f, 2f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles, positions);

//            level.CheckCollision();
//            Assert.AreEqual(new Vector2(-2f, 0f), sprite.Position);
//        }

//        [Test]
//        public void LevelCollisionCornerTest()
//        {
//            /* Scenario 5:
//             * A sprite collides with a set of tiles that looks like this:
//             *    |
//             *    |
//             * ___|, that is, a corner formed by a row and column of tiles.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new Tile[] { new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile() };
//            var positions = new Vector2[] { new Vector2(-1f, 14f), new Vector2(15f, 14f), new Vector2(29f, 14f), new Vector2(29f, -2f), new Vector2(29f, -18f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles, positions);

//            level.CheckCollision();
//            Assert.AreEqual(new Vector2(-2f, -2f), sprite.Position);
//        }

//        [Test]
//        public void LevelCollisionEmbeddedTest1()
//        {
//            /* Scenario 6:
//             * A sprite collides with two rows of tiles such that
//             * it cannot resolve itself out of them.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new Tile[] { new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile() };
//            var position = new Vector2[] { new Vector2(-2f, 14f), new Vector2(14f, 14f), new Vector2(30f, 14f), new Vector2(-2f, -18f), new Vector2(14f, -18f), new Vector2(30f, -18f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles, position);

//            level.CheckCollision();
//            Assert.IsTrue(sprite.IsEmbedded);
//        }

//        [Test]
//        public void LevelCollisionEmbeddedTest2()
//        {
//            /* Scenario 6:
//             * A sprite collides with two columns of tiles such that
//             * it cannot resolve itself out of them.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new Tile[] { new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile(), new UnitTestingTile() };
//            var position = new Vector2[] { new Vector2(-14f, -15f), new Vector2(-14f, 1f), new Vector2(-14f, 17f), new Vector2(18f, -15f), new Vector2(18f, 1f), new Vector2(18f, 17f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles, position);

//            level.CheckCollision();
//            Assert.IsTrue(sprite.IsEmbedded);
//        }

//        [Test]
//        public void LevelCollisionEmbeddedTest3()
//        {
//            /* Scenario 7:
//             * A sprite collides with a 3x3 grid of tiles such that
//             * it cannot resolve itself out of them.
//             */

//            Level level = new Level();
//            UnitTestingSprite sprite = new UnitTestingSprite();
//            var tiles = new List<Tile>();
//            for (int i = 0; i < 9; i++) { tiles.Add(new UnitTestingTile()); }
//            var positons = new Vector2[] { new Vector2(-16f, -16f), new Vector2(0f, -16f), new Vector2(16f, -16f), new Vector2(-16f, 0f), new Vector2(0f, 0f), new Vector2(16f, 0f), new Vector2(-16f, 16f), new Vector2(0f, 16f), new Vector2(16f, 16f) };
//            this.AddSpriteToLevel(level, sprite, Vector2.Zero);
//            this.AddTilesToLevel(level, tiles.ToArray(), positons);

//            level.CheckCollision();
//            Assert.IsTrue(sprite.IsEmbedded);
//        }
//    }
//}
