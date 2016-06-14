using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Collections
{
	/// <summary>
	/// Represents a single cell in a <see cref="SparseCellGrid{T}"/> instance.
	/// </summary>
	/// <typeparam name="T">A type implementing the <see cref="IPositionable2"/> interface.</typeparam>
	public sealed class SparseCell<T> where T : IPositionable2
	{
		/// <summary>
		/// A collection of all the items intersecting this cell.
		/// </summary>
		private HashSet<T> cellItems = new HashSet<T>();

		/// <summary>
		/// The size of this cell.
		/// </summary>
		private Vector2 cellSize;

		/// <summary>
		/// The two-dimensional, zero-based index of this cell.
		/// </summary>
		private Point cellNumber;

		/// <summary>
		/// Gets a collection of all the items intersecting this cell.
		/// </summary>
		public HashSet<T> Items
		{
			get
			{
				return cellItems;
			}
		}

		/// <summary>
		/// Gets the bounding rectangle enclosing this cell.
		/// </summary>
		public BoundingRectangle Bounds { get; }

		/// <summary>
		/// Gets a value indicating whether this cell has no items.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return cellItems.Count == 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SparseCell{T}"/> class.
		/// </summary>
		/// <param name="cCellSize">The size of this cell.</param>
		/// <param name="cCellNumber">The two-dimensional, zero-based index of this cell.</param>
		public SparseCell(Vector2 cCellSize, Point cCellNumber)
		{
			if (cCellSize.IsNaN() || cCellSize.X <= 0f || cCellSize.Y <= 0f) { throw new ArgumentException("The sparse cell constructor received a cell size that is not a number or has zero or negative area.", nameof(cCellSize)); }
			// cCellNumber doesn't need to be validated - all int pairs are valid

			cellSize = cCellSize;
			cellNumber = cCellNumber;

			Bounds = CreateBounds();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SparseCell{T}"/> class.
		/// </summary>
		/// <param name="items">A collection of items to place within this cell.</param>
		/// <param name="cCellSize">The size of this cell.</param>
		/// <param name="cCellNumber">The two-dimensional, zero-based index of this cell.</param>
		public SparseCell(IEnumerable<T> items, Vector2 cCellSize, Point cCellNumber)
		{
			if (items == null || !items.Any()) { throw new ArgumentException("The sparse cell constructor received a collection of items that was null or empty.", nameof(items)); }
			if (cCellSize.X <= 0f || cCellSize.Y <= 0f || cCellSize.IsNaN()) { throw new ArgumentException("The sparse cell constructor received a cell size that is not a number or has zero or negative area.", nameof(cCellSize)); }

			cellItems = new HashSet<T>(items);
			cellSize = cCellSize;
			cellNumber = cCellNumber;

			Bounds = CreateBounds();
		}

		/// <summary>
		/// Adds an item to this cell.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(T item)
		{
			if (!ItemIntersectsCell(item)) { throw new ArgumentOutOfRangeException(nameof(item.Position), $"When trying to add an item to a sparse cell, the item was found to be outside of the cell. Please validate the positioning of the item or which cell it should go in. Expected range: from {new Vector2(Bounds.Top, Bounds.Left)} to {new Vector2(Bounds.Bottom, Bounds.Right)} (cell {cellNumber.X},{cellNumber.Y}). Item's actual properties: position {item.Position}, size {item.Size}"); }

			cellItems.Add(item);
		}

		/// <summary>
		/// Removes an item from this cell.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public bool Remove(T item)
		{
			return cellItems.Remove(item);
		}

		/// <summary>
		/// Creates the bounding rectangle of this cell.
		/// </summary>
		/// <returns>The bounding rectangle of this cell.</returns>
		private BoundingRectangle CreateBounds()
		{
			// seems rather wasteful to keep constructing the rectangle all the time
			Vector2 cellPosition = new Vector2(cellNumber.X * cellSize.X, cellNumber.Y * cellSize.Y);

			return new BoundingRectangle(cellPosition.X, cellPosition.Y, cellSize.X, cellSize.Y);
		}

		/// <summary>
		/// Returns a value indicating whether a given item intersects this cell.
		/// </summary>
		/// <param name="item">The item to check for intersection.</param>
		/// <returns>True if the item intersects this cell (including a tangent, edges-only intersection), False if it does not.</returns>
		public bool ItemIntersectsCell(T item)
		{
			if (item == null) { throw new ArgumentNullException(nameof(item), "When attempting to determine if an item falls within a given sparse grid cell, the item provided was a null reference. Please ensure a null reference isn't passed to this method."); }

			if (item.Size == Vector2.Zero || item.Size.IsNaN())
			{
				return false;
			}

			return new BoundingRectangle(item.Position, item.Position + item.Size).IntersectsIncludingEdges(Bounds);
		}
	}
}
