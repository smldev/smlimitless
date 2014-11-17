//-----------------------------------------------------------------------
// <copyright file="QuadTree.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
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
            foreach (Vector2 cell in intersectingCells)
            {
                if (this.cells.ContainsKey(cell))
                {
                    result.AddRange(this.cells[cell].Sprites);
                }
            }
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
            
			foreach (var cell in intersectingCells)
			{
				foreach (Tile tile in this.cells[cell].Tiles)
				{
					if (!tile.IsExcluded)
					{
						result.Add(tile);
					}
				}
			}

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
				foreach (Tile tile in this.cells[cell].Tiles)
				{
					if (!tile.IsExcluded && tile.Hitbox is BoundingRectangle)
					{
						result.Add(tile);
					}
				}
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
                if (!tile.IsExcluded && tile is SlopedTile)
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
					if (tile.IsExcluded) continue;

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
        /// Returns a collection of all sprites within a given rectangle,
        /// plus all sprites in all cells tangent to the cells intersecting
        /// the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to retrieve sprites from.</param>
        /// <returns>A collection of sprites.</returns>
        public List<Sprite> GetNearbySprites(BoundingRectangle rect)
        {
            // Get all the cells the rect intersects plus a "ring" around these cells
            Vector2 topLeftCell = this.GetCellNumberAtPosition(new Vector2(rect.Left, rect.Top)) + new Vector2(-1f, -1f);
            Vector2 topRightCell = this.GetCellNumberAtPosition(new Vector2(rect.Right, rect.Top)) + new Vector2(1f, -1f);
            Vector2 bottomLeftCell = this.GetCellNumberAtPosition(new Vector2(rect.Left, rect.Bottom)) + new Vector2(-1f, 1f);
            Vector2 bottomRightCell = this.GetCellNumberAtPosition(new Vector2(rect.Right, rect.Bottom)) + new Vector2(1f, 1f);

            List<Sprite> result = new List<Sprite>();
            for (int y = (int)topLeftCell.Y; y < (int)bottomLeftCell.Y; y++)
            {
                for (int x = (int)topLeftCell.X; x < (int)topRightCell.X; x++)
                {
                    Vector2 cell = new Vector2(x, y);
                    if (this.cells.ContainsKey(cell))
                    {
                        result.AddRange(this.cells[cell].Sprites);
                    }
                }
            }

            return result;
        }

		/// <summary>
		/// Returns all the tiles in a given rectangular area.
		/// </summary>
		/// <param name="area">The area for which to return tiles.</param>
		/// <returns>A list of tiles within the area. The list will be empty if the area is zero or has no tiles.</returns>
		public List<Tile> GetTilesInArea(BoundingRectangle area)
		{
			List<Tile> result = new List<Tile>();

			Vector2 topLeftCell = this.GetCellNumberAtPosition(area.Position);
			Vector2 topRightCell = this.GetCellNumberAtPosition(new Vector2(area.Right, area.Top));
			Vector2 bottomLeftCell = this.GetCellNumberAtPosition(new Vector2(area.Left, area.Bottom));
			Vector2 bottomRightCell = this.GetCellNumberAtPosition(new Vector2(area.Right, area.Bottom));

			for (float x = topLeftCell.X; x <= topRightCell.X; x++)
			{
				for (float y = topLeftCell.Y; y <= bottomLeftCell.Y; y++)
				{
					foreach (Tile tile in this.cells[new Vector2(x, y)].Tiles)
					{
						if (tile.Hitbox.Bounds.Intersects(area))
						{
							result.Add(tile);
						}
					}
				}
			}

			return result;
		}

        /// <summary>
        /// Updates the QuadTree, recalculating the cells for every tile.
        /// </summary>
        public void Update()
        {
            this.ClearAll();
            this.sprites.ForEach(s => this.PlaceSprite(s));
            this.tiles.ForEach(t => this.PlaceTile(t)); // PERF: welp, found my bottleneck. Perhaps replacing 1000-odd mostly static tiles every frame isn't a good idea
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
                GameServices.DebugFont.DrawString(cell.Key.ToString(), new Vector2((cell.Key.X * this.CellSize.X) + 2, (cell.Key.Y * this.CellSize.Y) + 2));
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
        public void PlaceSprite(Sprite sprite)
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
			// PERF: For some odd reason, this method takes about 33.72% of ALL CPU time. External code (???) accounts
			// for about half that time, and the rest is in GetIntersectingCells.
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

		/// <summary>
		/// Creates an enumerator that travels through cells in a certain direction, returning each one.
		/// </summary>
		/// <param name="startCell">The initial cell to start from.</param>
		/// <param name="direction">The direction the enumerator travels.</param>
		/// <param name="searchDistance">The distance in cells the enumerator should travel.</param>
		/// <returns>An enumerator that yields a number of cells in a certain direction.</returns>
		private IEnumerable<QuadTreeCell> GetCellEnumerator(Vector2 startCell, Direction direction, int searchDistance)
		{
			if (direction == Direction.None)
			{
				throw new ArgumentException("QuadTree.GetCellEnumerator(Vector2, Direction): The direction \"None\" is not valid.");
			}

			if (searchDistance <= 0)
			{
				throw new ArgumentException(String.Format("QuadTree.GetCellEnumerator(Vector2, Direction): A search distance of {0} is not valid. Please use a positive number.", searchDistance));
			}

			int distanceRemaining = searchDistance;
			Vector2 currentCell = startCell;
			Vector2 addend;
			switch (direction)
			{
				case Direction.Up:
					addend = new Vector2(0f, -1f);
					break;
				case Direction.Down:
					addend = new Vector2(0f, 1f);
					break;
				case Direction.Left:
					addend = new Vector2(-1f, 0f);
					break;
				case Direction.Right:
					addend = new Vector2(1f, 0f);
					break;
				default:
					throw new ArgumentException("QuadTree.GetCellEnumerator(Vector2, Direction): Invalid direction.");
			}

			while (distanceRemaining > 0)
			{
				QuadTreeCell result = null;
				if (this.cells.ContainsKey(currentCell))
				{
					result = this.cells[currentCell];
				}

				currentCell += addend;
				distanceRemaining--;
				yield return result;
			}

			yield break;
		}

		/// <summary>
		/// Returns the first tile intersecting an axis-aligned ray cast from a given point and direction.
		/// </summary>
		/// <param name="position">The position of the beginning of the ray.</param>
		/// <param name="direction">The direction that the ray travels.</param>
		/// <param name="searchDistance">The distance, in quadtree cells, that this ray should travel in its search.</param>
		/// <returns>The first tile intersecting this ray, or null if no tile was found.</returns>
		public Tile GetTileIntersectingAARay(Vector2 position, Direction direction, int searchDistance)
		{
			if (position.IsNaN())
			{
				throw new ArgumentException("QuadTree.GetTileIntersectingAARay(Vector2, Direction, int): One or both of the components of the starting position are the invalid value NaN.");
			}

			if (direction == Direction.None)
			{
				throw new ArgumentException("QuadTree.GetTileIntersectingAARay(Vector2, Direction, int): The search direction was set to the invalid direction None.");
			}

			if (searchDistance <= 0)
			{
				throw new ArgumentException(String.Format("QuadTree.GetTileIntersectingAARay(Vector2, Direction, int): The search distance was equal to {0}; it must be greater than zero.", searchDistance));
			}

			var cellEnumerator = this.GetCellEnumerator(this.GetCellNumberAtPosition(position), direction, searchDistance);

			foreach (var cell in cellEnumerator)
			{
				if (!cell.Tiles.Any())
				{
					continue;
				}

				foreach (Tile tile in cell.Tiles)
				{
					BoundingRectangle tileBounds = tile.Hitbox.Bounds;
					if (direction == Direction.Left || direction == Direction.Right)
					{
						if (position.Y.BetweenInclusive(tileBounds.Top, tileBounds.Bottom))
						{
							return tile;
						}
					}
					else if (direction == Direction.Up || direction == Direction.Down)
					{
						if (position.X.BetweenInclusive(tileBounds.Left, tileBounds.Right))
						{
							return tile;
						}
					}
				}
			}

			return null;
		}
    }
}
