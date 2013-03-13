using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Sprites;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a collection of sprites and tiles divided into cells.
    /// </summary>
    /// <remarks>
    /// This is a lazy quadtree - it does not separate cells based on the number of objects inside them.
    /// </remarks>
    public class QuadTree
    {
        private Dictionary<Vector2, QuadTreeCell> cells;

        private List<Sprite> sprites;
        private List<Tile> tiles;

        /// <summary>
        /// The size in pixels of all cells.
        /// </summary>
        public Vector2 CellSize { get; private set; }

        public QuadTree(Vector2 cellSize)
        {
            cells = new Dictionary<Vector2, QuadTreeCell>();
            sprites = new List<Sprite>();
            tiles = new List<Tile>();
            CellSize = cellSize;
        }

        private List<Vector2> GetIntersectingCells(IPositionable item)
        {
            var result = new List<Vector2>();
            result.AddUnlessDuplicate(item.Position.FloorDivide(CellSize));
            result.AddUnlessDuplicate(new Vector2(item.Position.X + item.Size.X, item.Position.Y).FloorDivide(CellSize));
            result.AddUnlessDuplicate(new Vector2(item.Position.X, item.Position.Y + item.Size.Y).FloorDivide(CellSize));
            result.AddUnlessDuplicate(new Vector2(item.Position.X + item.Size.X, item.Position.Y + item.Size.Y).FloorDivide(CellSize));
            return result;
        }

        private void PlaceSprite(Sprite sprite)
        {
            var intersectingCells = GetIntersectingCells(sprite);

            foreach (var cell in intersectingCells)
            {
                if (!cells.ContainsKey(cell)) cells.Add(cell, new QuadTreeCell());
                if (!cells[cell].Sprites.Contains(sprite)) cells[cell].Add(sprite);
            }
        }

        private void PlaceTile(Tile tile)
        {
            var intersectingCells = GetIntersectingCells(tile);

            foreach (var cell in intersectingCells)
            {
                if (!cells.ContainsKey(cell)) cells.Add(cell, new QuadTreeCell());
                if (!cells[cell].Tiles.Contains(tile)) cells[cell].Add(tile);
            }
        }

        public void Add(Sprite sprite)
        {
            sprites.Add(sprite);
            PlaceSprite(sprite);
        }

        public void Add(Tile tile)
        {
            tiles.Add(tile);
            PlaceTile(tile);
        }

        public void Remove(Sprite sprite)
        {
            sprites.Remove(sprite);
            foreach (var cell in cells.Values)
            {
                cell.Sprites.Remove(sprite);
            }
        }

        public void Remove(Tile tile)
        {
            tiles.Remove(tile);
            foreach (var cell in cells.Values)
            {
                cell.Tiles.Remove(tile);
            }
        }

        /// <summary>
        /// Clears all the sprites and tiles from all cells.
        /// </summary>
        public void Clear()
        {
            foreach (var cell in cells.Values)
            {
                cell.Sprites.Clear();
                cell.Tiles.Clear();
            }
        }

        /// <summary>
        /// Clears all the sprites and tiles from all cells, and then removes all cells entirely.
        /// </summary>
        public void ClearAll()
        {
            cells.Clear();
        }

        public List<Sprite> GetCollidableSprites(Sprite sprite)
        {
            var result = new List<Sprite>();
            var intersectingCells = GetIntersectingCells(sprite);
            intersectingCells.ForEach(cell => result.AddRange(cells[cell].Sprites));
            return result;
        }

        public List<Tile> GetCollidableTiles(Sprite sprite)
        {
            var result = new List<Tile>();
            var intersectingCells = GetIntersectingCells(sprite);
            intersectingCells.ForEach(cell => result.AddRange(cells[cell].Tiles));
            return result;
        }

        public void Update()
        {
            ClearAll();
            sprites.ForEach(s => PlaceSprite(s));
            tiles.ForEach(t => PlaceTile(t));
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

    internal class QuadTreeCell
    {
        internal List<Sprite> Sprites;
        internal List<Tile> Tiles;

        internal QuadTreeCell()
        {
            Sprites = new List<Sprite>();
            Tiles = new List<Tile>();
        }

        internal void Add(Sprite sprite)
        {
            Sprites.Add(sprite);
        }

        internal void Add(Tile tile)
        {
            Tiles.Add(tile);
        }

        internal void Remove(Sprite sprite)
        {
            Sprites.Remove(sprite);
        }

        internal void Remove(Tile tile)
        {
            Tiles.Remove(tile);
        }
    }
}
