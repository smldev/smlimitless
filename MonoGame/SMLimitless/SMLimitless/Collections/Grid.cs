//-----------------------------------------------------------------------
// <copyright file="Grid.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Collections
{
    /// <summary>
    /// Represents a two-dimensional grid of values.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the grid.</typeparam>
    public sealed class Grid<T>
    {
        /*
         * A grid is a generic collection type that is basically a 2D array.
         * Objects are stored in a 2D array, accessed via two integer indices.
         * The grid's width and height are accessible via properties, and an
         * indexer allows access to individual items. Additionally, the grid
         * can return a "subgrid" - a portion of itself, given a starting pair
         * of indicies and a width and height of the new subgrid.
         */

        /// <summary>
        /// A two-dimensional array containing the values.
        /// </summary>
        private T[,] values;

        /// <summary>
        /// Gets the width of this grid, measured in grid cells.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of this grid, measured in grid cells.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid{T}"/> class.
        /// </summary>
        /// <param name="width">The width of the grid, measured in grid cells.</param>
        /// <param name="height">The height of the grid, measured in grid cells.</param>
        public Grid(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Grid<T>.ctor(int, int): Grid width and height must be greater than zero. Width: {0}, Height: {1}", width, height));
            }

            this.values = new T[width, height];
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets a value in the grid.
        /// </summary>
        /// <param name="x">The X-position of the grid cell.</param>
        /// <param name="y">The Y-position of the grid cell.</param>
        /// <returns>The value in the grid cell.</returns>
        public T this[int x, int y]
        {
            get
            {
                if (!this.IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Grid<T>.this[int, int].get: Index of value to retrieve must be within the bounds of the grid. X: {0}, Y: {1}", x, y));
                }

                return this.values[x, y];
            }

            set
            {
                if (!this.IndexWithinBounds(x, y))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Grid<T>.this[int, int].set: Index of value to set must be within the bounds of the grid. X: {0}, Y: {1}", x, y));
                }

                this.values[x, y] = value;
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
        public Grid<T> GetSubgrid(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Grid<T>.GetSubgrid(int, int, int, int): The subgrid must have a width and a height greater than zero. X: {0}, Y: {1}, Width: {2}, Height: {3}", x, y, width, height));
            }
            else if (!this.IndexWithinBounds(x, y))
            {
                throw new ArgumentOutOfRangeException(string.Format("Grid<T>.GetSubgrid(int, int, int, int): Origin of the subgrid must be within the original grid. X: {0}, Y: {1}, Width: {2}, Height: {3}", x, y, width, height));
            }
            else if (!this.IndexWithinBounds(x + width, y + height))
            {
                throw new ArgumentOutOfRangeException(string.Format("Grid<T>.GetSubgrid(int, int, int, int): The subgrid must be entirely contained within the original grid. X: {0}, Y: {1}, Width: {2}, Height: {3}", x, y, width, height));
            }

            Grid<T> result = new Grid<T>(width, height);
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
        /// Checks whether a given grid cell coordinate is within the bounds of the grid.
        /// </summary>
        /// <param name="x">The X-position of the grid cell.</param>
        /// <param name="y">The Y-position of the grid cell.</param>
        /// <returns>True if the cell is within the bounds of the grid, false if otherwise.</returns>
        private bool IndexWithinBounds(int x, int y)
        {
            return (x >= 0 && x < this.Width) && (y >= 0 && y < this.Height);
        }
    }
}