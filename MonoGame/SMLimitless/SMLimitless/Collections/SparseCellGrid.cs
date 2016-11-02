using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Collections
{
	/// <summary>
	///   Represents a grid composed of sized cells that can contain items. The
	///   grid is "sparse" - cells only exist if they contains items.
	/// </summary>
	/// <typeparam name="T">
	///   A type implementing the <see cref="IPositionable2" /> interface.
	/// </typeparam>
	public sealed class SparseCellGrid<T> : IEnumerable<T>, IEnumerable where T : IPositionable2
	{
		/// <summary>
		///   The default size, in pixels, of a sparse cell.
		/// </summary>
		private static readonly Vector2 DefaultCellSize = new Vector2(64f);

		/// <summary>
		///   A dictionary of all the cell indices as keys and the corresponding
		///   cells as values.
		/// </summary>
		private Dictionary<Point, SparseCell<T>> cells = new Dictionary<Point, SparseCell<T>>();

		/// <summary>
		///   A collection of all items within any of the cells in the grid.
		/// </summary>
		private List<T> items = new List<T>();

		/// <summary>
		///   Gets a read-only dictionary of the cells in this grid.
		/// </summary>
		public ReadOnlyDictionary<Point, SparseCell<T>> Cells
		{
			get
			{
				return new ReadOnlyDictionary<Point, SparseCell<T>>(cells);
			}
		}

		/// <summary>
		///   Gets the size of all cells.
		/// </summary>
		public Vector2 CellSize { get; }

		/// <summary>
		///   Gets a read-only collection of the items in this grid.
		/// </summary>
		public ReadOnlyCollection<T> Items
		{
			get
			{
				return items.AsReadOnly();
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="SparseCellGrid{T}" /> class.
		/// </summary>
		public SparseCellGrid()
		{
			CellSize = DefaultCellSize;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="SparseCellGrid{T}" /> class.
		/// </summary>
		/// <param name="cellSize">The size of all cells.</param>
		public SparseCellGrid(Vector2 cellSize)
		{
			if (cellSize.IsNaN() || cellSize.X <= 0f || cellSize.Y <= 0f) { throw new ArgumentException("The sparse cell grid constructor received a cell size that is not a number or has zero or negative area.", nameof(cellSize)); }

			CellSize = cellSize;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="SparseCellGrid{T}" /> class.
		/// </summary>
		/// <param name="cellSize">The size of all cells.</param>
		/// <param name="items">
		///   A collection of all items to be contained within this grid.
		/// </param>
		public SparseCellGrid(Vector2 cellSize, IEnumerable<T> items)
		{
			if (cellSize.IsNaN() || cellSize.X <= 0f || cellSize.Y <= 0f) { throw new ArgumentException("The sparse cell grid constructor received a cell size that is not a number or has zero or negative area.", nameof(cellSize)); }
			if (items == null || !items.Any()) { throw new ArgumentException("The sparse cell grid constructor received a collection of items that was null or empty.", nameof(items)); }

			CellSize = cellSize;
		}

		/// <summary>
		///   Adds an item to this grid.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item), "When trying to add an item to the sparse cell grid, the item to be added was null.");
			if (item.Position.IsNaN()) throw new ArgumentException("The item to add to the sparse cell grid has a position that is not a number.", nameof(item.Position));
			if (item.Size.IsNaN() || item.Size.X == 0f || item.Size.Y == 0f) throw new ArgumentOutOfRangeException(nameof(item.Size), $"The item to add to the sparse cell grid has no area. Width: {item.Size.X}, Height: {item.Size.Y}.");

			items.Add(item);

			LocalAdd(item);
		}

		/// <summary>
		///   Draws each cell as white rectangular edges with their cell number
		///   printed in the corner.
		/// </summary>
		public void DrawCells()
		{
			foreach (KeyValuePair<Point, SparseCell<T>> cellPair in cells)
			{
				Point cellNumber = cellPair.Key;
				BoundingRectangle cellBounds = cellPair.Value.Bounds;
				BitmapFont debugFont = GameServices.DebugFont;

				GameServices.SpriteBatch.DrawRectangleEdges(cellBounds.ToRectangle(), Color.White);

				if (debugFont != null)
				{
					Vector2 drawStringPosition = new Vector2(cellBounds.Position.X + 3f, cellBounds.Position.Y + 3f);
					Vector2 drawCountPosition = new Vector2(cellBounds.Right - 11f, cellBounds.Top + 3f);
					debugFont.DrawString($"{cellNumber.X},{cellNumber.Y}", drawStringPosition);
					debugFont.DrawString($"{cellPair.Value.Items.Count}", drawCountPosition);
				}
			}
		}

		/// <summary>
		///   Gets the enumerator for this grid.
		/// </summary>
		/// <returns>
		///   An enumerator that enumerates over every item in this grid cell by cell.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach (T item in cells.SelectMany(c => cells.Values.SelectMany(v => v.Items)).Distinct()) // good god
			{
				yield return item;
			}
		}

		/// <summary>
		///   The explicit interface implementation of IEnumerable.GetEnumerator().
		/// </summary>
		/// <returns>The enumerator returned by the GetEnumerator() method.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Removes an item from this grid.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(T item)
		{
			if (item == null) throw new ArgumentNullException(nameof(item), "The item to remove from the sparse cell grid was null.");

			items.Remove(item);

			LocalRemove(item);
		}

		/// <summary>
		///   Removes all items from this grid that match a predicate.
		/// </summary>
		/// <param name="predicate">
		///   All the removed items must match this predicate.
		/// </param>
		public void RemoveAllWhere(Predicate<T> predicate)
		{
			cells.Values.ForEach(c => c.Items.RemoveWhere(predicate));
		}

		/// <summary>
		///   Checks and updates the containing cells of all items that have
		///   moved since the last update. Removes any empty cells.
		/// </summary>
		public void Update()
		{
			// To update the sparse cell grid, we need to remove and re-add any
			// items that have moved since the last update.
			foreach (T item in items)
			{
				if (item.HasMoved)
				{
					LocalRemove(item);
					LocalAdd(item);
					item.HasMoved = false;
				}
			}

			cells.RemoveAll(c => c.IsEmpty);
		}

		/// <summary>
		///   Gets the cell number at a given position.
		/// </summary>
		/// <param name="position">The position to get the cell number for.</param>
		/// <returns>
		///   A two-dimensional, zero-based index of the cell that the given
		///   position is within.
		/// </returns>
		internal Point GetCellNumberAtPosition(Vector2 position)
		{
			return new Point((int)Math.Floor(position.X / CellSize.X), (int)Math.Floor(position.Y / CellSize.Y));
		}

		internal IEnumerable<T> GetItemsNearItem(T item)
		{
			SparseCellRange range = GetCellsItemIsIn(item);

			HashSet<T> itemsProcessedSoFar = new HashSet<T>();
			itemsProcessedSoFar.Add(item);

			for (int y = range.TopLeft.Y; y <= range.BottomRight.Y; y++)
			{
				for (int x = range.TopLeft.X; x <= range.BottomRight.X; x++)
				{
					Point cellNumber = new Point(x, y);

					if (!cells.ContainsKey(cellNumber)) { continue; }

					foreach (T cellItem in cells[cellNumber].Items)
					{
						if (itemsProcessedSoFar.Add(cellItem)) { yield return cellItem; }
					}
				}
			}
		}

		internal IEnumerable<T> ItemsInCells(SparseCellRange range)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Adds an item to a given cell.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <param name="cellNumber">
		///   The two-dimensional, zero-based index of the cell to which to add
		///   the item.
		/// </param>
		private void AddItemToCell(T item, Point cellNumber)
		{
			if (!cells.ContainsKey(cellNumber))
			{
				cells.Add(cellNumber, new SparseCell<T>(CellSize, cellNumber));
			}

			cells[cellNumber].Add(item);
		}

		/// <summary>
		///   Gets a range containing the two-dimensional, zero-based indices of
		///   every cell a given item is in.
		/// </summary>
		/// <param name="item">The item to get the cells for.</param>
		/// <returns>The range of cells the item is in.</returns>
		private SparseCellRange GetCellsItemIsIn(T item)
		{
			Point cellTopLeft = GetCellNumberAtPosition(new Vector2(item.Position.X, item.Position.Y));
			Point cellBottomRight = GetCellNumberAtPosition(new Vector2(item.Position.X + item.Size.X, item.Position.Y + item.Size.Y));

			return new SparseCellRange(cellTopLeft, cellBottomRight);
		}

		/// <summary>
		///   Adds an item to the cell grid without adding it to the Items collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		private void LocalAdd(T item)
		{
			// LocalAdd and LocalRemove are needed so we don't douch our Items
			// collections in Update(), which changes the Items collection and
			// invalidates the enumerator
			var range = GetCellsItemIsIn(item);

			for (int y = range.TopLeft.Y; y <= range.BottomRight.Y; y++)
			{
				for (int x = range.TopLeft.X; x <= range.BottomRight.X; x++)
				{
					AddItemToCell(item, new Point(x, y));
				}
			}
		}

		/// <summary>
		///   Removes an item from the cell grid without removing it from the
		///   Items collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		private void LocalRemove(T item)
		{
			foreach (var cell in cells)
			{
				cell.Value.Remove(item);
			}
		}
	}
}
