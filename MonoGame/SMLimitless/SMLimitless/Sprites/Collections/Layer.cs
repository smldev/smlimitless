using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	/// Represents a grid containing tiles, itself containing sprites.
	/// </summary>
	public sealed class Layer : IName
	{
		/// <summary>
		/// Gets a value indicating whether this layer is the main layer in its owner section.
		/// </summary>
		public bool IsMainLayer { get; internal set; }
		private bool isInitialized;
		private bool isContentLoaded;
		private bool isActive;

		internal Section Owner { get; private set; }

		internal SizedGrid<Tile> Tiles { get; set; }
		private List<Sprite> sprites = new List<Sprite>();

		/// <summary>
		/// Gets the bounds of this layer; the rectangle containing all tiles in the layer.
		/// </summary>
		public BoundingRectangle Bounds { get; private set; } = BoundingRectangle.NaN;

		/// <summary>
		/// Gets the position of the layer; the position of the top-left corner of the <see cref="Bounds"/>.
		/// </summary>
		public Vector2 Position { get; internal set; } = new Vector2(float.NaN);

		/// <summary>
		/// Gets the numeric index of this layer in its owner section.
		/// </summary>
		public int Index { get; internal set; }
		private Vector2 velocity = Vector2.Zero;

		/// <summary>
		/// Gets or sets the name of this layer.
		/// </summary>
		[DefaultValue(""), Description("The name of this layer to be used in event scripting. This field is optional.")]
		public string Name { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Layer"/> class.
		/// </summary>
		/// <param name="cOwner">The section that owns this layer.</param>
		/// <param name="isMainLayer">A value indicating whether this layer is the main layer in its owner section. Defaults to false.</param>
		public Layer(Section cOwner, bool isMainLayer = false)
		{
			Owner = cOwner;
			IsMainLayer = isMainLayer;
			if (IsMainLayer) { Owner.MainLayer = this; }
		}

		/// <summary>
		/// Initializes the tiles and sprites in this layer.
		/// </summary>
		public void Initialize()
		{
			if (!isInitialized)
			{
				Tiles.ForEach(t => t.Initialize(Owner));
				sprites.ForEach(s => s.Initialize(Owner));

				isInitialized = true;
			}
		}

		/// <summary>
		/// Loads the content of the tiles and sprites in this layer.
		/// </summary>
		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Tiles.ForEach(t => t.LoadContent());
				sprites.ForEach(s => s.LoadContent());

				isContentLoaded = true;
			}
		}

		/// <summary>
		/// Updates the contents of this layer.
		/// </summary>
		public void Update()
		{
			// TODO: change this method to account for active/inactive layers, tiles and sprites
			Tiles.ForEach(t => t.Update());
			sprites.ForEach(s => s.Update());
		}

		/// <summary>
		/// Draws the outline of the <see cref="Bounds"/> of this layer.
		/// </summary>
		/// <param name="color">The color to draw the outline.</param>
		public void Draw(Color color)
		{
			Bounds.DrawOutline(color);
		}

		/// <summary>
		/// Adds the tiles in an enumerable to this layer.
		/// </summary>
		/// <param name="tiles">An enumerable of zero or more tiles.</param>
		/// <exception cref="ArgumentException">Thrown if any tile in the enumerable is not grid-aligned.</exception>
		internal void AddTiles(IEnumerable<Tile> tiles)
		{
			// The sized grid, being a 2D array, suffers from the same un-resizability that the Array does.
			// Adding a new tile will be O(n) worst case because we may have to resize the grid,
			// but if we have a bunch of tiles to add at once, we only need to resize the grid once, if at all.

			if (!tiles.Any()) return;
			if (!this.Tiles.DoesRangeAlignToGrid(tiles)) { throw new ArgumentException("Tried to add tiles to a grid that weren't grid aligned."); }

			BoundingRectangle allTilesBound = tiles.Concat(this.Tiles).GetBoundsOfPositionables();

			// so, here's probably a good point to talk about cell coordinate agnosticity
			// Tiles will have cell coordinates since they're in a grid where each cell has coordinates,
			// but you can't rely on them, and here's why:
			int allTilesBoundWidthInCells = (int)(allTilesBound.Width / this.Tiles.CellWidth);
			int allTilesBoundHeightInCells = (int)(allTilesBound.Height / this.Tiles.CellHeight);

			// ...adding tiles that are to the left and/or above the old grid...
			float newGridOriginX = (allTilesBound.X < this.Tiles.Position.X) ? allTilesBound.X : this.Tiles.Position.X;
			float newGridOriginY = (allTilesBound.Y < this.Tiles.Position.Y) ? allTilesBound.Y : this.Tiles.Position.Y;
			int newGridWidth = (allTilesBoundWidthInCells * this.Tiles.CellWidth > this.Tiles.Width) ? allTilesBoundWidthInCells * this.Tiles.CellWidth : (this.Tiles.Width);
			int newGridHeight = (allTilesBoundHeightInCells * this.Tiles.CellHeight > this.Tiles.Height) ? allTilesBoundHeightInCells * this.Tiles.CellHeight : (this.Tiles.Height);
			Vector2 newGridOrigin = new Vector2(newGridOriginX, newGridOriginY);
			SizedGrid<Tile> newGrid = new SizedGrid<Tile>(newGridOrigin, this.Tiles.CellWidth, this.Tiles.CellHeight, 
														  newGridWidth, newGridHeight);

			// ...forces a change of the cell coordinates of every tile already in the grid.
			// We also have to move the layer's position accordingly.
			Position = newGridOrigin;

			this.Tiles.ForEach(t => newGrid.Add(t));
			tiles.ForEach(t => newGrid.Add(t));

			foreach (Tile tile in newGrid)
			{
				// Update adjacency flags
				Vector2 tileCell = GetCellNumberAtPosition(tile.Position);
				Vector2 cellLeft = new Vector2(tileCell.X - 1f, tileCell.Y);
				Vector2 cellRight = new Vector2(tileCell.X + 1f, tileCell.Y);

				Tile tileLeft = (cellLeft.X >= 0f) ? GetTile(cellLeft) : null;
				Tile tileRight = (cellRight.X < newGrid.Width) ? GetTile(cellRight) : null;

				if (tileLeft != null && tileLeft.TileShape == CollidableShape.RightTriangle && (tileLeft.SlopedSides == RtSlopedSides.TopLeft || tileLeft.SlopedSides == RtSlopedSides.BottomLeft))
				{
					tile.AdjacencyFlags |= TileAdjacencyFlags.SlopeOnLeft;
				}

				if (tileRight != null && tileRight.TileShape == CollidableShape.RightTriangle && (tileRight.SlopedSides == RtSlopedSides.TopRight || tileRight.SlopedSides == RtSlopedSides.BottomRight)) 
				{
					tile.AdjacencyFlags |= TileAdjacencyFlags.SlopeOnRight;
				}
			}

			this.Tiles = newGrid;
			Bounds = this.Tiles.Bounds;

			tiles.ForEach(t => Owner.Tiles.Add(t));
		}

		internal void AddTile(Tile tile)
		{
			AddTiles(new[] { tile });
		}

		/// <summary>
		/// Removes a tile from this layer.
		/// </summary>
		/// <param name="tile">The tile to remove.</param>
		public void RemoveTile(Tile tile)
		{
			// Unlike when adding tiles, removing a tile doesn't shrink the grid even if the grid could shrink
			// also holy cow the RemoveTile(Tile) implementation on master is *horrible*
			Tiles.Remove(tile);
		}

		internal void AddSprite(Sprite sprite)
		{
			sprites.Add(sprite);
		}

		/// <summary>
		/// Gets the cell number at a given position.
		/// </summary>
		/// <param name="position">The given position.</param>
		/// <returns>A <see cref="Vector2"/> containing the cell number at the position.</returns>
		/// <remarks>Returns cell numbers for positions outside the <see cref="Bounds"/> of the layer.</remarks>
		public Vector2 GetCellNumberAtPosition(Vector2 position)
		{
			Vector2 adjustedPosition = position - this.Position;
			return new Vector2((adjustedPosition.X / Tiles.CellWidth), (adjustedPosition.Y / Tiles.CellHeight)).Floor();
		}

		/// <summary>
		/// Gets the cell number at a given position, or the cell number along an edge or corner for out-of-bounds positions.
		/// </summary>
		/// <param name="position">The given position.</param>
		/// <returns>A <see cref="Vector2"/> representing a cell number within the layer.</returns>
		public Vector2 GetClampedCellNumberAtPosition(Vector2 position)
		{
			Vector2 cellNumber = GetCellNumberAtPosition(position);
			cellNumber.X = MathHelper.Clamp(cellNumber.X, 0, Tiles.Width);
			cellNumber.Y = MathHelper.Clamp(cellNumber.Y, 0, Tiles.Height);
			return cellNumber;
		}

		public bool CellWithinBounds(Vector2 cellNumber)
		{
			return cellNumber.X >= 0f && cellNumber.X < Tiles.Width && cellNumber.Y >= 0f && cellNumber.Y < Tiles.Height;
		}

		/// <summary>
		/// Gets a tile at a given cell number.
		/// </summary>
		/// <param name="x">The X number of the cell; the cells away from the left-most cell.</param>
		/// <param name="y">The Y number of the cell; the cells below the top-most cell.</param>
		/// <returns>A tile in the cell, or null if there is no tile there.</returns>
		public Tile GetTile(int x, int y)
		{
			return Tiles[x, y];
		}
		
		/// <summary>
		/// Gets a tile at a given cell number.
		/// </summary>
		/// <param name="cellNumber">A <see cref="Vector2"/> containing the cell number.</param>
		/// <returns>A tile in the cell, or null if there is no tile there.</returns>
		public Tile GetTile(Vector2 cellNumber)
		{
			return GetTile((int)cellNumber.X, (int)cellNumber.Y);
		}

		/// <summary>
		/// Gets a tile in a given cell, or null if there is no tile in that cell, or the given cell is out of bounds.
		/// </summary>
		/// <param name="cellNumber">The cell for which to get the tile.</param>
		/// <returns>A Tile or null.</returns>
		public Tile SafeGetTile(Vector2 cellNumber)
		{
			if (Tiles.IndexWithinBounds((int)cellNumber.X, (int)cellNumber.Y))
			{
				return GetTile(cellNumber);
			}
			return null;
		}

		internal void SetMainLayer()
		{
			if (Owner.MainLayer != null)
			{
				throw new InvalidOperationException("Tried to set a section's main layer, but the section already has a main layer.");
			}

			IsMainLayer = true;
			Vector2 objectSize = GameServices.GameObjectSize;
			SizedGrid<Tile> newGrid = new SizedGrid<Tile>(Vector2.Zero, (int)objectSize.X, (int)objectSize.Y,
														  (int)(Owner.Bounds.Width / objectSize.X), (int)(Owner.Bounds.Height / objectSize.Y));
			Tiles.ForEach(t => newGrid.Add(t));
			Tiles = newGrid;
			Owner.MainLayer = this;
			Owner.Layers.Insert(0, this);
		}

		/// <summary>
		/// Moves this layer to a given position.
		/// </summary>
		/// <param name="position">The position to move the layer to.</param>
		/// <exception cref="InvalidOperationException">Thrown when attempting to move the main layer. The main layer always has its position at the origin.</exception>
		public void Move(Vector2 position)
		{
			if (IsMainLayer) { throw new InvalidOperationException("Cannot move the main layer."); }

			Vector2 distance = position - this.Position;
			Translate(distance);
		}

		/// <summary>
		/// Moves the layer and its contents by a given distance.
		/// </summary>
		/// <param name="distance">The distance to move the layer.</param>
		/// <exception cref="InvalidOperationException">Thrown when attempting to translate the main layer. The main layer always has its position at the origin.</exception>
		public void Translate(Vector2 distance)
		{
			if (IsMainLayer) { throw new InvalidOperationException("Cannot translate the main layer."); }

			Position += distance;
			Tiles.Position += distance;

			// Move every tile and sprite first.
			Tiles.ForEach(t => t.Position += distance);
			sprites.ForEach(s => s.Position += distance);

			// Finally, move the grid.
			Tiles.Position += distance;
		}
	}
}
