//-----------------------------------------------------------------------
// <copyright file="SizedGrid.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Collections
{
    /// <summary>
    /// Represents a grid made of cells of a specified size.
    /// </summary>
    /// <typeparam name="T">A type that derives from the <see cref="IPositionable"/> interface.</typeparam>
    public sealed class SizedGrid<T> : IEnumerable<T> where T : IPositionable2
    {
        /*
         * The sized grid is a generic collection composed of cells.
         * Each cell is of a certain size (usually pixels), and each
         * cell holds a reference to an object. Objects are required
         * to implement the IPositionable interface. If an object is
         * larger than a grid cell, the object is placed within multiple
         * grid cells, with each cell holding a reference to the object.
         * If the object is smaller, the cell can still only hold one reference
         * to it.
         */

        /// <summary>
        /// The internal grid.
        /// </summary>
        private Grid<T> grid;

		public Vector2 Position { get; internal set; }

        /// <summary>
        /// Gets the width of a grid cell, usually in pixels.
        /// </summary>
        public int CellWidth { get; private set; }

        /// <summary>
        /// Gets the height of a grid cell, usually in pixels.
        /// </summary>
        public int CellHeight { get; private set; }

        /// <summary>
        /// Gets the width of the grid, represented as (width in cells * width of cell).
        /// </summary>
        public int Width
        {
            get
            {
                return grid.Width;
            }
        }

        /// <summary>
        /// Gets the height of the grid, represented as (height in cells * height of cell).
        /// </summary>
        public int Height
        {
            get
            {
                return grid.Height;
            }
        }

		public BoundingRectangle Bounds
		{
			get
			{
				return new BoundingRectangle(Position.X, Position.Y, (CellWidth * Width), (CellHeight * Height));
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SizedGrid{T}"/> class.
        /// </summary>
		/// <param name="position">The position of the top-left corner of the grid.</param>
        /// <param name="cellWidth">The width of the grid cells.</param>
        /// <param name="cellHeight">The height of the grid cells.</param>
        /// <param name="gridWidth">The width of the grid in cells.</param>
        /// <param name="gridHeight">The height of the grid in cells.</param>
        public SizedGrid(Vector2 position, int cellWidth, int cellHeight, int gridWidth, int gridHeight)
        {
            if (cellWidth <= 0 || cellHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.ctor(int, int, int, int): Cell width and height must be greater than zero. Cell Width: {0}, Cell Height: {1}, Grid Width: {2}, Grid Height: {3}", cellWidth, cellHeight, gridWidth, gridHeight));
            }
            
			if (gridWidth <= 0)
            {
				gridWidth = -gridWidth;
            }
			if (gridHeight <= 0) { gridHeight = -gridHeight; }

			grid = new Grid<T>(gridWidth, gridHeight);
			Position = position;
			CellWidth = cellWidth;
			CellHeight = cellHeight;
        }

        /// <summary>
        /// Gets an object on the grid from the specified cell.
        /// To add objects to the grid, use the Add method.
        /// </summary>
        /// <param name="x">The X-coordinate of the cell.</param>
        /// <param name="y">The Y-coordinate of the cell.</param>
        /// <returns>The object at the specified grid cell, or null if there is no object in the cell.</returns>
        public T this[int x, int y]
        {
            get
            {
                if (!IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.this[int, int].get: The provided cell number fell outside the range of the grid. The grid has the size of {0}x{1} cells and the requested cell was {2}, {3}.", grid.Width, grid.Height, x, y));
                }

                return grid[x, y];
            }

            private set
            {
                if (!IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.this[int, int].set: The provided cell number fell outside the range of the grid. The grid has the size of {0}x{1} cells and the requested cell was {2}, {3}.", grid.Width, grid.Height, x, y));
                }

				grid[x, y] = value;
            }
        }

		/// <summary>
		/// Returns a value indicating whether all items in an enumerable align to the grid.
		/// </summary>
		/// <param name="items">The items to check for alignment.</param>
		/// <returns>True if all items align to the grid, false if they don't.</returns>
		public bool DoesRangeAlignToGrid(IEnumerable<T> items)
		{
			return items.All(i =>
			{
				var offset = OffsetPosition(i);
				return offset.X % CellWidth == 0 && offset.Y % CellHeight == 0;
			});
		}

        /// <summary>
        /// Adds an item to the grid.
        /// WARNING: This will overwrite any items that are
        /// already present where the item will be placed.
        /// </summary>
        /// <param name="item">The item to add to the grid.</param>
        public void Add(IPositionable2 item)
        {
            if (item.Position.X % CellWidth != 0 || item.Position.Y % CellHeight != 0)
            {
                throw new ArgumentException(string.Format("SizedGrid<T>.Add(IPositionable): The item's position does not align to the grid. Items must be aligned to ({0}, {1})-pixel boundaries. Item's position is {2}, {3}.", CellWidth, CellHeight, item.Position.X, item.Position.Y));
            }
            else if (item.Size.X % CellWidth != 0 || item.Size.Y % CellHeight != 0)
            {
                throw new ArgumentException(string.Format("SizedGrid<T>.Add(IPositionable): The item's size is not a multiple of the grid's cell size. Each grid cell has a size of {0}, {1}, and the item has a size of {2}, {3}.", CellWidth, CellHeight, item.Position.X, item.Position.Y));
            }
			else if (!Bounds.IntersectsIncludingEdges(item.Position))
			{
				throw new ArgumentException("Attempted to add an item that was outside the bounds of the grid. To add such an item, call AddWithResize instead.");
			}

			var startingCell = GetCellAtPosition(item.Position);
            int widthInCells = (int)item.Size.X / CellWidth;
            int heightInCells = (int)item.Size.Y / CellHeight;

            for (int y = startingCell.Y; y < startingCell.Y + heightInCells; y++)
            {
                for (int x = startingCell.X; x < startingCell.X + widthInCells; x++)
                {
                    if (this[x, y] != null)
                    {
						Remove(this[x, y]);
                    }

                    this[x, y] = (T)item;
                }
            }
        }

		public void AddWithResize(T item)
		{
			// we don't need this right now
			throw new NotImplementedException();
		}

        /// <summary>
        /// Removes an item from the grid.
        /// </summary>
        /// <param name="item">The item to be removed from the grid.</param>
        public void Remove(IPositionable2 item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "SizedGrid<T>.Remove(IPositionable): Cannot remove a null reference from the grid.");
            }

			var startingCell = GetCellAtPosition(item.Position);
            int widthInCells = (int)item.Size.X / CellWidth;
            int heightInCells = (int)item.Size.Y / CellHeight;

            for (int y = startingCell.Y; y < startingCell.Y + heightInCells; y++)
            {
                for (int x = startingCell.X; x < startingCell.X + widthInCells; x++)
                {
                    this[x, y] = default(T);
                }
            }
        }

        /// <summary>
        /// Returns a portion of this grid.
        /// </summary>
        /// <param name="x">The X-position on this grid of the top-left corner of the subgrid.</param>
        /// <param name="y">The Y-position on this grid of the top-left corner of the subgrid.</param>
        /// <param name="width">The width of the subgrid.</param>
        /// <param name="height">The height of the subgrid.</param>
        /// <returns>A portion of this grid.</returns>
        public SizedGrid<T> GetSubgrid(int x, int y, int width, int height)
        {
            if (!IndexWithinBounds(x, y))
            {
                throw new ArgumentException(string.Format("SizedGrid.GetSubgrid(int, int, int, int): The starting position {0}, {1} is not within the grid.", x, y));
            }
            
            if (!IndexWithinBounds(x + width, y + height))
            {
                throw new ArgumentException(string.Format("SizedGrid.GetSubgrid(int, int, int, int): The ending position {0}, {1} is not within the grid.", x + width, y + height));
            }

			Vector2 subgridPosition = Position + new Vector2(x * CellWidth, y * CellWidth);
            SizedGrid<T> result = new SizedGrid<T>(subgridPosition, CellWidth, CellWidth, width, height);
            int resultX = 0, resultY = 0;

            for (int yPos = y; yPos < y + height; yPos++)
            {
                for (int xPos = x; xPos < x + width; xPos++)
                {
                    result[resultX, resultY] = this[xPos, yPos];
                    resultX++;
                }

                resultY++;
                resultX = 0;
            }

            return result;
        }

        /// <summary>
        /// Returns the cell number for a given position.
        /// </summary>
        /// <param name="position">The position to return for.</param>
        /// <returns>The cell number for the given position.</returns>
        public Point GetCellAtPosition(Vector2 position)
        {
			position = OffsetPosition(position);

            int x = (int)(position.X / CellWidth);
            int y = (int)(position.Y / CellHeight);

            return new Point(x, y);
        }

        /// <summary>
        /// Returns the object at the given position on the grid.
        /// </summary>
        /// <param name="position">The position to return for.</param>
        /// <returns>The object at the position, or null if there is no object.</returns>
        public T GetObjectAtPosition(Vector2 position)
        {
            Point cell = GetCellAtPosition(position);
            return this[cell.X, cell.Y];
        }

        /// <summary>
        /// Draws the cell borders of this grid.
        /// </summary>
        /// <param name="position">The position to start drawing the grid at.</param>
        /// <param name="lineColor">The color of the cell lines.</param>
        public void Draw(Color lineColor)
        {
            // The total number of vertical lines to draw is (height + 1).
            // The total number of horizontal lines to draw is (width + 1).
            float gridWidth = CellWidth * grid.Width;
            float gridHeight = CellHeight * grid.Height;

            float xPosition = Position.X;
            float yPosition = Position.Y;

            for (int y = 0; y <= grid.Height; y++)
            {
                GameServices.SpriteBatch.DrawLine(1f, lineColor, new Vector2(xPosition, yPosition), new Vector2(xPosition + gridWidth, yPosition));
                yPosition += CellHeight;
            }

            yPosition = Position.Y;

            for (int x = 0; x <= grid.Width; x++)
            {
                GameServices.SpriteBatch.DrawLine(1f, lineColor, new Vector2(xPosition, yPosition), new Vector2(xPosition, yPosition + gridHeight));
                xPosition += CellWidth;
            }
        }

        /// <summary>
        /// Checks if a grid cell coordinate falls within the bounds of the grid.
        /// </summary>
        /// <param name="x">The X-coordinate of the coordinate to check.</param>
        /// <param name="y">The Y-coordinate of the coordinate to check.</param>
        /// <returns>True if the cell falls within the grid, false if otherwise.</returns>
        public bool IndexWithinBounds(int x, int y)
        {
            return (x >= 0 && x < grid.Width) && (y >= 0 && y < grid.Height);
        }

        /// <summary>
        /// Checks if a point in space falls within the bounds of the grid.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point falls within the grid, false if otherwise.</returns>
        public bool PointWithinBounds(Vector2 point)
        {
			return Bounds.IntersectsIncludingEdges(point);
        }

		private Vector2 OffsetPosition(T item)
		{
			return item.Position - Position;
		}

		private Vector2 OffsetPosition(Vector2 point)
		{
			return point - Position;
		}

		public IEnumerator<T> GetEnumerator()
		{
			var gridEnumerator = grid.GetEnumerator();

			while (gridEnumerator.MoveNext())
			{
				if (gridEnumerator.Current != null)
				{
					yield return gridEnumerator.Current;
				}
			}

			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return grid.GetEnumerator();
		}
    }
}
