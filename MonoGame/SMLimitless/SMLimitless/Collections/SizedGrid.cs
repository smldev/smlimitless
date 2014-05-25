﻿//-----------------------------------------------------------------------
// <copyright file="SizedGrid.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
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
    public sealed class SizedGrid<T> where T : IPositionable
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
                return this.grid.Width;
            }
        }

        /// <summary>
        /// Gets the height of the grid, represented as (height in cells * height of cell).
        /// </summary>
        public int Height
        {
            get
            {
                return this.grid.Height;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizedGrid{T}"/> class.
        /// </summary>
        /// <param name="cellWidth">The width of the grid cells.</param>
        /// <param name="cellHeight">The height of the grid cells.</param>
        /// <param name="gridWidth">The width of the grid in cells.</param>
        /// <param name="gridHeight">The height of the grid in cells.</param>
        public SizedGrid(int cellWidth, int cellHeight, int gridWidth, int gridHeight)
        {
            if (cellWidth <= 0 || cellHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.ctor(int, int, int, int): Cell width and height must be greater than zero. Cell Width: {0}, Cell Height: {1}, Grid Width: {2}, Grid Height: {3}", cellWidth, cellHeight, gridWidth, gridHeight));
            }
            else if (gridWidth <= 0 || gridHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.ctor(int, int, int, int): Grid width and height must be greater than zero. Cell Width: {0}, Cell Height: {1}, Grid Width: {2}, Grid Height: {3}", cellWidth, cellHeight, gridWidth, gridHeight));
            }

            this.grid = new Grid<T>(gridWidth, gridHeight);
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
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
                if (!this.IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.this[int, int].get: The provided cell number fell outside the range of the grid. The grid has the size of {0}x{1} cells and the requested cell was {2}, {3}.", this.grid.Width, this.grid.Height, x, y));
                }
                return this.grid[x, y];
            }

            private set
            {
                if (!this.IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.this[int, int].set: The provided cell number fell outside the range of the grid. The grid has the size of {0}x{1} cells and the requested cell was {2}, {3}.", this.grid.Width, this.grid.Height, x, y));
                }
                this.grid[x, y] = value;
            }
        }

        /// <summary>
        /// Adds an item to the grid.
        /// WARNING: This will overwrite any items that are
        /// already present where the item will be placed.
        /// </summary>
        /// <param name="item">The item to add to the grid.</param>
        public void Add(IPositionable item)
        {
            if (item.Position.X % this.CellWidth != 0 || item.Position.Y % this.CellHeight != 0)
            {
                throw new ArgumentException(string.Format("SizedGrid<T>.Add(IPositionable): The item's position does not align to the grid. Items must be aligned to ({0}, {1})-pixel boundaries. Item's position is {2}, {3}.", this.CellWidth, this.CellHeight, item.Position.X, item.Position.Y));
            }
            else if (item.Size.X % this.CellWidth != 0 || item.Size.Y % this.CellHeight != 0)
            {
                throw new ArgumentException(string.Format("SizedGrid<T>.Add(IPositionable): The item's size is not a multiple of the grid's cell size. Each grid cell has a size of {0}, {1}, and the item has a size of {2}, {3}.", this.CellWidth, this.CellHeight, item.Position.X, item.Position.Y));
            }

            var startingCell = new IntVector2((int)item.Position.X / this.CellWidth, (int)item.Position.Y / this.CellHeight);
            int widthInCells = (int)item.Size.X / this.CellWidth;
            int heightInCells = (int)item.Size.Y / this.CellHeight;

            for (int y = startingCell.Y; y < startingCell.Y + heightInCells; y++)
            {
                for (int x = startingCell.X; x < startingCell.X + widthInCells; x++)
                {
                    if ((IPositionable)this[x, y] != null)
                    {
                        this.Remove(this[x, y]);
                    }

                    this[x, y] = (T)item;
                }
            }
        }

        /// <summary>
        /// Removes an item from the grid.
        /// </summary>
        /// <param name="item">The item to be removed from the grid.</param>
        public void Remove(IPositionable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "SizedGrid<T>.Remove(IPositionable): Cannot remove a null reference from the grid.");
            }
            else if (!this.IndexWithinBounds((int)item.Position.X, (int)item.Position.Y))
            {
                throw new ArgumentOutOfRangeException(string.Format("SizedGrid<T>.Remove(IPositionable): Cannot remove an item that does not fall within the grid. X:{0}, Y:{1}", item.Position.X, item.Position.Y));
            }

            var startingCell = new IntVector2((int)item.Position.X / this.CellWidth, (int)item.Position.Y / this.CellHeight);
            int widthInCells = (int)item.Size.X / this.CellWidth;
            int heightInCells = (int)item.Size.Y / this.CellHeight;

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
            if (!this.IndexWithinBounds(x, y))
            {
                throw new ArgumentException(string.Format("SizedGrid.GetSubgrid(int, int, int, int): The starting position {0}, {1} is not within the grid.", x, y));
            }
            
            if (!this.IndexWithinBounds(x + width, y + height))
            {
                throw new ArgumentException(string.Format("SizedGrid.GetSubgrid(int, int, int, int): The ending position {0}, {1} is not within the grid.", x + width, y + height));
            }

            SizedGrid<T> result = new SizedGrid<T>(this.CellWidth, this.CellWidth, width, height);
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
            int x = (int)(position.X / this.CellWidth);
            int y = (int)(position.Y / this.CellHeight);

            return new Point(x, y);
        }

        /// <summary>
        /// Returns the object at the given position on the grid.
        /// </summary>
        /// <param name="position">The position to return for.</param>
        /// <returns>The object at the position, or null if there is no object.</returns>
        public T GetObjectAtPosition(Vector2 position)
        {
            Point cell = this.GetCellAtPosition(position);
            return this[cell.X, cell.Y];
        }

        /// <summary>
        /// Draws the cell borders of this grid.
        /// </summary>
        /// <param name="position">The position to start drawing the grid at.</param>
        /// <param name="lineColor">The color of the cell lines.</param>
        public void Draw(Vector2 position, Color lineColor)
        {
            // The total number of vertical lines to draw is (height + 1).
            // The total number of horizontal lines to draw is (width + 1).
            float gridWidth = this.CellWidth * this.grid.Width;
            float gridHeight = this.CellHeight * this.grid.Height;

            float xPosition = position.X;
            float yPosition = position.Y;

            for (int y = 0; y <= this.grid.Height; y++)
            {
                GameServices.SpriteBatch.DrawLine(1f, lineColor, new Vector2(xPosition, yPosition), new Vector2(xPosition + gridWidth, yPosition));
                yPosition += this.CellHeight;
            }

            yPosition = position.Y;

            for (int x = 0; x <= this.grid.Width; x++)
            {
                GameServices.SpriteBatch.DrawLine(1f, lineColor, new Vector2(xPosition, yPosition), new Vector2(xPosition, yPosition + gridHeight));
                xPosition += this.CellWidth;
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
            return (x >= 0 && x < this.grid.Width) && (y >= 0 && y < this.grid.Height);
        }

        /// <summary>
        /// Checks if a point in space falls within the bounds of the grid.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point falls within the grid, false if otherwise.</returns>
        public bool PointWithinBounds(Vector2 point)
        {
            int rightEdge = this.grid.Width * this.CellWidth;
            int bottomEdge = this.grid.Height * this.CellHeight;

            return (point.X >= 0f && point.X <= rightEdge) && (point.Y >= 0f && point.Y <= bottomEdge);
        }
    }
}