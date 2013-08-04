using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SMLimitless.Collections;

namespace SMLTests.Tests
{
    [TestFixture]
    public class GridTests
    {
        [Test]
        public void SubgridConstructionTest()
        {
            Grid<bool> grid = new Grid<bool>(3, 3);
            /* 
             * Grid map:
             * [ T T F ]
             * [ T T F ]
             * [ F F F ]
             */

            grid[0, 0] = true;
            grid[0, 1] = true;
            grid[0, 2] = false;
            grid[1, 0] = true;
            grid[1, 1] = true;
            grid[1, 2] = false;
            grid[2, 0] = false;
            grid[2, 1] = false;
            grid[2, 2] = false;

            Grid<bool> subgrid = grid.GetSubgrid(0, 0, 2, 2);
            Assert.IsTrue(subgrid[0, 0] && subgrid[0, 1] && subgrid[1, 0] && subgrid[1, 1]);
        }

        [Test]
        public void SizedGridAddItemTest()
        {
            SizedGrid<Positionable> sizedGrid = new SizedGrid<Positionable>(16, 16, 2, 2);
            Positionable item = new Positionable();
            item.Position = new Vector2(16f, 16f);

            sizedGrid.Add(item);
            Assert.NotNull(sizedGrid[1, 1]);
        }

        [Test]
        public void SizedGridRemoveItemTest()
        {
            SizedGrid<Positionable> sizedGrid = new SizedGrid<Positionable>(16, 16, 2, 2);
            Positionable item = new Positionable();
            item.Position = new Vector2(16f, 16f);

            sizedGrid.Add(item);
            sizedGrid.Remove(item);
            Assert.Null(sizedGrid[1, 1]);
        }

        [Test]
        public void SizedGridAddLargeItemTest()
        {
            SizedGrid<Positionable> sizedGrid = new SizedGrid<Positionable>(16, 16, 2, 2);
            Positionable item = new Positionable();
            item.Position = Vector2.Zero;
            item.Size = new Vector2(32f, 32f);

            sizedGrid.Add(item);
            bool areGridCellsNotNull = (sizedGrid[0, 0] != null) && (sizedGrid[1, 0] != null) && (sizedGrid[0, 1] != null) && (sizedGrid[1, 1] != null);
            Assert.True(areGridCellsNotNull);
        }

        [Test]
        public void SizedGridOverwriteAddItem()
        {
            SizedGrid<Positionable> sizedGrid = new SizedGrid<Positionable>(16, 16, 2, 2);
            Positionable smallItem = new Positionable();
            Positionable bigItem = new Positionable() { Size = new Vector2(16f, 32f) };

            sizedGrid.Add(smallItem);
            sizedGrid.Add(bigItem);
            Assert.AreEqual(bigItem, sizedGrid[0, 0]);
        }
    }

    internal class Positionable : SMLimitless.Interfaces.IPositionable
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Positionable()
        {
            this.Size = new Vector2(16f, 16f);
        }
    }
}
