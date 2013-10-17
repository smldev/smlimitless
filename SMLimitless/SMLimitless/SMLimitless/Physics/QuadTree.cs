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
        /// Given a point, this returns the coordinates of the cell the point is in.
        /// </summary>
        /// <param name="position">The point to get the cell coordinates for.</param>
        /// <returns>The coordinates of the cell the point is in.</returns>
        public Vector2 GetCellNumberAtPosition(Vector2 position)
        {
            float x = (float)Math.Floor(position.X / this.CellSize.X);
            float y = (float)Math.Floor(position.Y / this.CellSize.Y);

            return new Vector2(x, y);
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
        /// Returns a list of all the normal (square) tiles that a given sprite could collide with.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>A list of tiles.</returns>
        public List<Tile> GetCollidableNormalTiles(Sprite sprite)
        {
            var intersectingCells = this.GetIntersectingCells(sprite);
            List<Tile> result = new List<Tile>();

            foreach (var cell in intersectingCells)
            {
                result.AddRange(this.cells[cell].Tiles.Where(t => !(t is SMLimitless.Sprites.Testing.SlopedTestTile1))); // this is hackish and will be replaced.
            }

            return result;
        }

        /// <summary>
        /// Returns a list of all the sloped tiles that a given sprite could collide with.
        /// </summary>
        /// <param name="sprite">The sprite whose cells will be used.</param>
        /// <returns>A list of sloped tiles that the sprite could collide with.</returns>
        public List<SlopedTile> GetCollidableSlopedTiles(Sprite sprite)
        {
            List<Tile> collidableTiles = this.GetCollidableTiles(sprite);
            List<SlopedTile> result = new List<SlopedTile>(collidableTiles.Count);

            foreach (Tile tile in collidableTiles)
            {
                if (tile is SlopedTile)
                {
                    result.Add((SlopedTile)tile);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets two lists of tiles that a given sprite could collide with.
        /// </summary>
        /// <param name="sprite">The sprite to retrieve the collidable tiles for.</param>
        /// <param name="tiles">A list of collidable tiles.</param>
        /// <param name="slopedTiles">A list of collidable sloped tiles.</param>
        public void GetCollidableTiles(Sprite sprite, out List<Tile> tiles, out List<SlopedTile> slopedTiles)
        {
            var resultTiles = new List<Tile>();
            var resultSloped = new List<SlopedTile>();
            var intersectingCells = this.GetIntersectingCells(sprite);

            foreach (var cell in intersectingCells)
            {
                foreach (Tile tile in this.cells[cell].Tiles)
                {
                    if (tile is SlopedTile)
                    {
                        resultSloped.AddUnlessDuplicate((SlopedTile)tile);
                    }
                    else
                    {
                        resultTiles.AddUnlessDuplicate(tile);
                    }
                }
            }

            tiles = resultTiles;
            slopedTiles = resultSloped;
        }

        /// <summary>
        /// Returns all the sprites in a given cell.
        /// </summary>
        /// <param name="cell">The coordinates of the cell.</param>
        /// <returns>A list of sprites, or null if the cell doesn't exist.</returns>
        public List<Sprite> GetSpritesInCell(Vector2 cell)
        {
            if (!this.cells.ContainsKey(cell))
            {
                return null;
            }

            return this.cells[cell].Sprites;
        }

        /// <summary>
        /// Returns all the tiles in a given cell.
        /// </summary>
        /// <param name="cell">The coordinates of the cell.</param>
        /// <returns>A list of tiles, or null if the cell doesn't exist.</returns>
        public List<Tile> GetTilesInCell(Vector2 cell)
        {
            if (!this.cells.ContainsKey(cell))
            {
                return null;
            }

            return this.cells[cell].Tiles;
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
            bool isLargeItem = item.Size.X > this.CellSize.X || item.Size.Y > this.CellSize.Y;

            int topCellY = (int)Math.Floor(rect.Top / this.CellSize.Y);
            int bottomCellY = (int)Math.Floor(rect.Bottom / this.CellSize.Y);
            int leftCellX = (int)Math.Floor(rect.Left / this.CellSize.X);
            int rightCellX = (int)Math.Floor(rect.Right / this.CellSize.X);

            if (isLargeItem)
            {
                /* Consider a sprite that is larger than a single cell on either axis or both.
                 * A sprite can collide with tiles, and can be resolved in a certain direction,
                 * but for very large sprites, this may push the sprite out of its old cells
                 * from which the collidable tiles were extracted from. Now that the sprite
                 * is in different cells, it has a different set of collidable tiles, but those
                 * aren't included in the produced set of collidable tiles. Thus, for very large
                 * sprites, we need to expand the intersecting cell space such that it will include
                 * all the tiles the sprite could collide with if it is resolved outside of its actual intersecting cells.
                 */
                topCellY--;
                leftCellX--;
                bottomCellY++;
                rightCellX++;
            }

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
