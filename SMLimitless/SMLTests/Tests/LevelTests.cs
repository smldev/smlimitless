//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using Microsoft.Xna.Framework;
//using NUnit.Framework;
//using SMLimitless;
//using SMLimitless.Physics;
//using SMLimitless.Sprites;
//using SMLimitless.Sprites.Collections;

//namespace SMLTests.Tests
//{
//    [TestFixture]
//    public class LevelTests
//    {
//        [Test]
//        public void CollidableTilesTest()
//        {
//            Level level = new Level();

//            UnitTestingTile tile = new UnitTestingTile() { Position = Vector2.Zero };
//            UnitTestingSprite sprite = new UnitTestingSprite() { Position = new Vector2(16f, 16f) };
//            level.AddSprite(sprite);
//            level.AddTile(tile);

//            FieldInfo quadTreeField = typeof(Level).GetField("quadTree", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
//            QuadTree quadTree = (QuadTree)quadTreeField.GetValue(level);

//            var collidableTiles = quadTree.GetCollidableTiles(sprite);

//            if (collidableTiles.Count != 1) Assert.Fail();
//            Assert.IsTrue(collidableTiles[0] == tile);
//        }

//        [Test]
//        public void IntersectingCellsLargeSpriteTest()
//        {
//            Level level = new Level();

//            UnitTestingTile tile = new UnitTestingTile() { Position = Vector2.Zero };
//            UnitTestingSprite sprite = new UnitTestingSprite() { Position = new Vector2(16f, 16f)};
//            typeof(UnitTestingSprite).GetProperty("Size").SetValue(sprite, new Vector2(80f, 80f), null);
//            level.AddTile(tile);
//            level.AddSprite(sprite);

//            FieldInfo quadTreeField = typeof(Level).GetField("quadTree", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
//            QuadTree quadTree = (QuadTree)quadTreeField.GetValue(level);

//            List<Vector2> intersectingCells;
//            MethodInfo getIntersectingCellsMethod = typeof(QuadTree).GetMethod("GetIntersectingCells", BindingFlags.NonPublic | BindingFlags.Instance);
//            intersectingCells = (List<Vector2>)getIntersectingCellsMethod.Invoke(quadTree, new[] { sprite });

//            Assert.IsTrue(intersectingCells.Count != 4);
//            // phew, look at how much reflection I needed
//        }
//    }
//}
