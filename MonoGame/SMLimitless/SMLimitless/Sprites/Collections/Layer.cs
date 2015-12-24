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
	public sealed class Layer : IName
	{
		public bool IsMainLayer { get; private set; }
		private bool isInitialized;
		private bool isContentLoaded;
		private bool isActive;

		private Section owner;

		private SizedGrid<Tile> tiles;  // TODO: this should be set on deserialize
		private List<Sprite> sprites = new List<Sprite>();

		private BoundingRectangle bounds = BoundingRectangle.NaN;
		private Vector2 position = new Vector2(float.NaN);
		private Vector2 velocity = Vector2.Zero;

		[DefaultValue(""), Description("The name of this layer to be used in event scripting. This field is optional.")]
		public string Name { get; set; }

		public Layer(Section cOwner, bool isMainLayer = false)
		{
			owner = cOwner;
			IsMainLayer = isMainLayer;

			// temporary
			tiles = new SizedGrid<Tile>(Vector2.Zero, (int)GameServices.GameObjectSize.X, (int)GameServices.GameObjectSize.Y, 1, 1);
		}

		public void Initialize()
		{
			if (!isInitialized)
			{
				tiles.ForEach(t => t.Initialize(owner));
				sprites.ForEach(s => s.Initialize(owner));

				isInitialized = true;
			}
		}

		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				tiles.ForEach(t => t.LoadContent());
				sprites.ForEach(s => s.LoadContent());

				isContentLoaded = true;
			}
		}

		public void Update()
		{
			// TODO: change this method to account for active/inactive layers, tiles and sprites
			tiles.ForEach(t => t.Update());
			sprites.ForEach(s => s.Update());
		}

		public void Draw(Color color)
		{
			bounds.DrawOutline(color);
		}

		internal void AddTiles(IEnumerable<Tile> tiles)
		{
			// The sized grid, being a 2D array, suffers from the same un-resizability that the Array does.
			// Adding a new tile will be O(n) worst case because we may have to resize the grid,
			// but if we have a bunch of tiles to add at once, we only need to resize the grid once, if at all.

			if (!tiles.Any()) return;
			if (!this.tiles.DoesRangeAlignToGrid(tiles)) { throw new ArgumentException("Tried to add tiles to a grid that weren't grid aligned."); }

			BoundingRectangle allTilesBound = tiles.Concat(this.tiles).GetBoundsOfPositionables();

			// so, here's probably a good point to talk about cell coordinate agnosticity
			// Tiles will have cell coordinates since they're in a grid where each cell has coordinates,
			// but you can't rely on them, and here's why:
			int allTilesBoundWidthInCells = (int)(allTilesBound.Width / this.tiles.CellWidth);
			int allTilesBoundHeightInCells = (int)(allTilesBound.Height / this.tiles.CellHeight);

			// ...adding tiles that are to the left and/or above the old grid...
			Vector2 newGridOrigin = new Vector2(allTilesBound.X, allTilesBound.Y);
			SizedGrid<Tile> newGrid = new SizedGrid<Tile>(newGridOrigin, this.tiles.CellWidth, this.tiles.CellHeight, 
														  allTilesBoundWidthInCells, allTilesBoundHeightInCells);

			// ...forces a change of the cell coordinates of every tile already in the grid.
			// We also have to move the layer's position accordingly.
			position = newGridOrigin;

			this.tiles.ForEach(t => newGrid.Add(t));
			tiles.ForEach(t => newGrid.Add(t));

			this.tiles = newGrid;
		}

		internal void AddTile(Tile tile)
		{
			AddTiles(new[] { tile });
		}

		public void RemoveTile(Tile tile)
		{
			// Unlike when adding tiles, removing a tile doesn't shrink the grid even if the grid could shrink
			// also holy cow the RemoveTile(Tile) implementation on master is *horrible*
			tiles.Remove(tile);
		}

		internal void AddSprite(Sprite sprite)
		{
			sprites.Add(sprite);
		}

		internal void SetMainLayer()
		{
			if (owner.MainLayer != null)
			{
				throw new InvalidOperationException("Tried to set a section's main layer, but the section already has a main layer.");
			}

			IsMainLayer = true;
			Vector2 objectSize = GameServices.GameObjectSize;
			SizedGrid<Tile> newGrid = new SizedGrid<Tile>(Vector2.Zero, (int)objectSize.X, (int)objectSize.Y,
														  (int)(owner.Bounds.Width / objectSize.X), (int)(owner.Bounds.Height / objectSize.Y));
			tiles.ForEach(t => newGrid.Add(t));
			tiles = newGrid;
			owner.MainLayer = this;
		}

		public void Move(Vector2 position)
		{
			if (IsMainLayer) { throw new InvalidOperationException("Cannot move the main layer."); }

			Vector2 distance = position - this.position;
			Translate(distance);
		}

		public void Translate(Vector2 distance)
		{
			if (IsMainLayer) { throw new InvalidOperationException("Cannot translate the main layer."); }

			position += distance;

			// Move every tile and sprite first.
			tiles.ForEach(t => t.Position += distance);
			sprites.ForEach(s => s.Position += distance);

			// Finally, move the grid.
			tiles.Position += distance;
		}
	}
}
