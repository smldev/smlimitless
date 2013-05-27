//-----------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
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
    /// This is a lazy QuadTree - it does not separate cells based on the number of objects inside them.
    /// </remarks>
    public class QuadTree
    {
        /// <summary>
        /// A dictionary of cells, with Vector2 keys
        /// representing cell number, and with QuadTreeCell
        /// values.
        /// </summary>
        private Dictionary<Vector2, QuadTreeCell> cells;

        /// <summary>
        /// A list of all sprites in this QuadTree.
        /// </summary>
        private List<Sprite> sprites;

        /// <summary>
        /// A list of all tiles in the QuadTree.
        /// </summary>
        private List<Tile> tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTree"/> class.
        /// </summary>
        /// <param name="cellSize">The size in pixels of each cell.</param>
        public QuadTree(Vector2 cellSize)
        {
            this.cells = new Dictionary<Vector2, QuadTreeCell>();
            this.sprites = new List<Sprite>();
            this.tiles = new List<Tile>();
            this.CellSize = cellSize;
        }

        /// <summary>
        /// Gets the size in pixels of all cells.
        /// </summary>
        public Vector2 CellSize { get; private set; }

        /// <summary>
        /// Adds a sprite to the QuadTree.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        public void Add(Sprite sprite)
        {
            this.sprites.Add(sprite);
            this.PlaceSprite(sprite);
        }

        /// <summary>
        /// Adds a tile to the QuadTree.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        public void Add(Tile tile)
        {
            this.tiles.Add(tile);
            this.PlaceTile(tile);
        }

        /// <summary>
        /// Removes a sprite from the QuadTree.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void Remove(Sprite sprite)
        {
            this.sprites.Remove(sprite);
            foreach (var cell in this.cells.Values)
            {
                cell.Sprites.Remove(sprite);
            }
        }

        /// <summary>
        /// Removes a tile from the QuadTree.
        /// </summary>
        /// <param name="tile">The tile to remove.</param>
        public void Remove(Tile tile)
        {
            this.tiles.Remove(tile);
            foreach (var cell in this.cells.Values)
            {
                cell.Tiles.Remove(tile);
            }
        }

        /// <summary>
        /// Clears all the sprites and tiles from all cells.
        /// The list of all sprites and tiles is not affected.
        /// </summary>
        public void Clear()
        {
            foreach (var cell in this.cells.Values)
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
            this.cells.Clear();
        }

        /// <summary>
        /// Gets a list of all the sprites that a given sprite could collide with.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>A list of sprites.</returns>
        public List<Sprite> GetCollidableSprites(Sprite sprite)
        {
            var result = new List<Sprite>();
            var intersectingCells = this.GetIntersectingCells(sprite);
            intersectingCells.ForEach(cell => result.AddRange(this.cells[cell].Sprites));
            return result;
        }

        /// <summary>
        /// Gets a list of all the tiles that a given sprite could collide with.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>A list of tiles.</returns>
        public List<Tile> GetCollidableTiles(Sprite sprite)
        {
            var result = new List<Tile>();
            var intersectingCells = this.GetIntersectingCells(sprite);
            intersectingCells.ForEach(cell => this.cells[cell].Tiles.ForEach(tile => result.AddUnlessDuplicate(tile)));
            return result;
        }

        /// <summary>
        /// Updates the QuadTree, recalculating the cells for every tile.
        /// </summary>
        public void Update()
        {
            this.ClearAll();
            this.sprites.ForEach(s => this.PlaceSprite(s));
            this.tiles.ForEach(t => this.PlaceTile(t));
        }

        /// <summary>
        /// Draws a grid of all cells in the QuadTree,
        /// as well as the cell number in small text
        /// in the top-left corner.
        /// </summary>
        public void Draw()
        {
            SpriteBatch batch = GameServices.SpriteBatch;

            foreach (var cell in this.cells)
            {
                Rectangle drawBounds = new Rectangle(
                 (int)(cell.Key.X * this.CellSize.X),
                 (int)(cell.Key.Y * this.CellSize.Y),
                 (int)this.CellSize.X,
                 (int)this.CellSize.Y);

                batch.DrawRectangleEdges(drawBounds, Color.White);
                batch.DrawString(GameServices.DebugFontSmall, cell.Key.ToString(), new Vector2((cell.Key.X * this.CellSize.X) + 2, (cell.Key.Y * this.CellSize.Y) + 2), Color.White);
            }
        }

        /// <summary>
        /// Returns a list of all the cells a given object intersects.
        /// </summary>
        /// <param name="item">The object to check.</param>
        /// <returns>A list of all intersecting cells.</returns>
        private List<Vector2> GetIntersectingCells(IPositionable item)
        {
            var result = new List<Vector2>();
            Rectangle rect = new Rectangle((int)item.Position.X, (int)item.Position.Y, (int)item.Size.X, (int)item.Size.Y);

            int topCellY = (int)Math.Floor(rect.Top / this.CellSize.Y);
            int bottomCellY = (int)Math.Floor(rect.Bottom / this.CellSize.Y);
            int leftCellX = (int)Math.Floor(rect.Left / this.CellSize.X);
            int rightCellX = (int)Math.Floor(rect.Right / this.CellSize.X);

            int x, y;

            for (y = topCellY; y <= bottomCellY; y++)
            {
                for (x = leftCellX; x <= rightCellX; x++)
                {
                    result.AddUnlessDuplicate(new Vector2(x, y));
                }
            }

            return result;
        }

        /// <summary>
        /// Places a sprite in the proper cell(s).
        /// </summary>
        /// <param name="sprite">The sprite to place.</param>
        private void PlaceSprite(Sprite sprite)
        {
            var intersectingCells = this.GetIntersectingCells(sprite);

            foreach (var cell in intersectingCells)
            {
                if (!this.cells.ContainsKey(cell))
                {
                    this.cells.Add(cell, new QuadTreeCell());
                }

                if (!this.cells[cell].Sprites.Contains(sprite))
                {
                    this.cells[cell].Add(sprite);
                }
            }
        }

        /// <summary>
        /// Places a tile in the proper cell(s).
        /// </summary>
        /// <param name="tile">The tile to place.</param>
        private void PlaceTile(Tile tile)
        {
            var intersectingCells = this.GetIntersectingCells(tile);

            foreach (var cell in intersectingCells)
            {
                if (!this.cells.ContainsKey(cell))
                {
                    this.cells.Add(cell, new QuadTreeCell());
                }

                if (!this.cells[cell].Tiles.Contains(tile))
                {
                    this.cells[cell].Add(tile);
                }
            }
        }
    }
}
