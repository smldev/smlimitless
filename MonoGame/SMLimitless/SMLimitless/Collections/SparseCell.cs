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
	public sealed class SparseCell<T> where T : IPositionable2
	{
		private HashSet<T> cellItems = new HashSet<T>();
		private Vector2 cellSize;
		private Point cellNumber;

		public HashSet<T> Items
		{
			get
			{
				return cellItems;
			}
		}

		public BoundingRectangle Bounds { get; }

		public bool IsEmpty
		{
			get
			{
				return cellItems.Count == 0;
			}
		}

		public SparseCell(Vector2 cCellSize, Point cCellNumber)
		{
			if (cCellSize.IsNaN() || cCellSize.X <= 0f || cCellSize.Y <= 0f) { throw new ArgumentException("The sparse cell constructor received a cell size that is not a number or has zero or negative area.", nameof(cCellSize)); }
			// cCellNumber doesn't need to be validated - all int pairs are valid

			cellSize = cCellSize;
			cellNumber = cCellNumber;

			Bounds = CreateBounds();
		}

		public SparseCell(IEnumerable<T> items, Vector2 cCellSize, Point cCellNumber)
		{
			if (items == null || !items.Any()) { throw new ArgumentException("The sparse cell constructor received a collection of items that was null or empty.", nameof(items)); }
			if (cCellSize.X <= 0f || cCellSize.Y <= 0f || cCellSize.IsNaN()) { throw new ArgumentException("The sparse cell constructor received a cell size that is not a number or has zero or negative area.", nameof(cCellSize)); }

			cellItems = new HashSet<T>(items);
			cellSize = cCellSize;
			cellNumber = cCellNumber;

			Bounds = CreateBounds();
		}

		private BoundingRectangle CreateBounds()
		{
			// seems rather wasteful to keep constructing the rectangle all the time
			Vector2 cellPosition = new Vector2(cellNumber.X * cellSize.X, cellNumber.Y * cellSize.Y);

			return new BoundingRectangle(cellPosition.X, cellPosition.Y, cellSize.X, cellSize.Y);
		}

		public bool ItemIntersectsCell(T item)
		{
			if (item == null) { throw new ArgumentNullException(nameof(item), "When attempting to determine if an item falls within a given sparse grid cell, the item provided was a null reference. Please ensure a null reference isn't passed to this method."); }

			if (item.Size == Vector2.Zero || item.Size.IsNaN())
			{
				return false;
			}

			return new BoundingRectangle(item.Position, item.Position + item.Size).IntersectsIncludingEdges(Bounds);
		}

		public void Add(T item)
		{
			if (!ItemIntersectsCell(item)) { throw new ArgumentOutOfRangeException(nameof(item.Position), $"When trying to add an item to a sparse cell, the item was found to be outside of the cell. Please validate the positioning of the item or which cell it should go in. Expected range: from {new Vector2(Bounds.Top, Bounds.Left)} to {new Vector2(Bounds.Bottom, Bounds.Right)} (cell {cellNumber.X},{cellNumber.Y}). Item's actual properties: position {item.Position}, size {item.Size}"); }

			cellItems.Add(item);
		}

		public void Remove(T item)
		{
			////if (!cellItems.Contains(item)) { throw new ArgumentException("Tried to remove an item from a sparse cell that wasn't in the sparse cell. Please validate the item's cell.", nameof(item)); }

			cellItems.Remove(item);
		}
	}
}
