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
    public class QuadTree<T> where T : IPositionable
    {
        private Dictionary<Vector2, List<IPositionable>> cells;

        /// <summary>
        /// The size of each cell in pixels.
        /// </summary>
        public Vector2 CellSize { get; private set; }

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
        }

        private void UpdateItemPosition(IPositionable item)
        {
            Remove(item);

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

        public void Update()
        {
            foreach (var cell in cells)
            {
                cell.Value.ForEach(i => UpdateItemPosition(i));
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
                 (int)((cell.Key.X * CellSize.X) + CellSize.X),
                 (int)((cell.Key.Y * CellSize.Y) + CellSize.Y));

                batch.DrawRectangle(drawBounds, Color.Gray);
            }
        }
    }
}
