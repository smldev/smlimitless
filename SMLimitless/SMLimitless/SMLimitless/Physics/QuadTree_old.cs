using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using SMLimitless;
using SMLimitless.Interfaces;
using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    [Obsolete]
    public class QuadTree<T> where T : IPositionable
    {
        private Dictionary<Vector2, List<IPositionable>> cells;
        private List<IPositionable> items;

        /// <summary>
        /// The size of each cell in pixels.
        /// </summary>
        public Vector2 CellSize { get; private set; }

        public QuadTree(Vector2 cellSize)
        {
            CellSize = cellSize;
            cells = new Dictionary<Vector2, List<IPositionable>>();
            items = new List<IPositionable>();
        }

        public List<IPositionable> this[Vector2 cell]
        {
            get
            {
                if (!cells.ContainsKey(cell)) return null;
                return cells[cell];
            }
        }

        /// <summary>
        /// Adds an item to the QuadTree.
        /// </summary>
        public void Add(IPositionable item)
        {
            InternalAdd(item);

            items.Add(item);
        }

        private void InternalAdd(IPositionable item)
        {
            // Items may intersect more than one cell, up to four.
            // We need to add the item to every cell it intersects.

            Vector2 cellTopLeft = item.Position.FloorDivide(CellSize);
            Vector2 cellTopRight = new Vector2(item.Position.X + item.Size.X, item.Position.Y).FloorDivide(CellSize);
            Vector2 cellBottomLeft = new Vector2(item.Position.X, item.Position.Y + item.Size.Y).FloorDivide(CellSize);
            Vector2 cellBottomRight = new Vector2(item.Position.X + item.Size.X, item.Position.Y + item.Size.Y).FloorDivide(CellSize);

            // Now, check if the cells exist, and add them if they don't.
            if (!cells.ContainsKey(cellTopLeft)) cells.Add(cellTopLeft, new List<IPositionable>());
            if (!cells.ContainsKey(cellTopRight)) cells.Add(cellTopRight, new List<IPositionable>());
            if (!cells.ContainsKey(cellBottomLeft)) cells.Add(cellBottomLeft, new List<IPositionable>());
            if (!cells.ContainsKey(cellBottomRight)) cells.Add(cellBottomRight, new List<IPositionable>());

            // Now, add the object to the cells, checking for duplicates.
            if (!cells[cellTopLeft].Contains(item)) cells[cellTopLeft].Add(item);
            if (!cells[cellTopRight].Contains(item)) cells[cellTopRight].Add(item);
            if (!cells[cellBottomLeft].Contains(item)) cells[cellBottomLeft].Add(item);
            if (!cells[cellBottomRight].Contains(item)) cells[cellTopRight].Add(item);
        }

        /// <summary>
        /// Removes an item from the QuadTree.
        /// </summary>
        public void Remove(IPositionable item)
        {
            foreach (var cell in cells)
            {
                if (cell.Value.Contains(item))
                {
                    cell.Value.Remove(item);
                }
            }

            items.Remove(item);
        }

        public void Update()
        {
            cells.Clear();

            foreach (var item in items)
            {
                InternalAdd(item);
            }
        }

        public void Draw()
        {
            SpriteBatch batch = GameServices.SpriteBatch;

            foreach (var cell in cells)
            {
                Rectangle drawBounds = new Rectangle(
                 (int)(cell.Key.X * CellSize.X),
                 (int)(cell.Key.Y * CellSize.Y),
                 (int)(CellSize.X),
                 (int)(CellSize.Y));

                batch.DrawRectangleEdges(drawBounds, Color.White);
            }
        }
    }
}
