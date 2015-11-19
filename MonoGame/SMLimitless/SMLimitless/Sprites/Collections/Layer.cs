using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
	public sealed class Layer
	{
		public bool IsMainLayer { get; }
		private bool isInitialized;
		private bool isContentLoaded;
		private bool isActive;

		private Section owner;

		private SizedGrid<Tile> tiles;
		private List<Sprite> sprites = new List<Sprite>();  // bit a philosophy change here. In master, sections owned sprites, layers, and the BG. Here, sections own layers own tiles/sprites, and the BG.

		private BoundingRectangle bounds = BoundingRectangle.NaN;
		private Vector2 position = new Vector2(float.NaN);
		private Vector2 velocity = Vector2.Zero;

		public Layer(Section cOwner, bool isMainLayer = false)
		{
			owner = cOwner;
			IsMainLayer = isMainLayer;
		}

		internal void AddTiles(IEnumerable<Tile> tiles)
		{
			// The sized grid, being a 2D array, suffers from the same un-resizability that the Array does.
			// Adding a new tile will be O(n) worst case because we may have to resize the grid,
			// but if we have a bunch of tiles to add at once, we only need to resize the grid once, if at all.

			if (!tiles.Any()) return;
			if (!this.tiles.DoesRangeAlignToGrid(tiles)) { throw new ArgumentException("Tried to add tiles to a grid that weren't grid aligned."); }

			BoundingRectangle allTilesBound = tiles.GetBoundsOfPositionables();

			// so, here's probably a good point to talk about cell coordinate agnosticity
			// Tiles will have cell coordinates since they're in a grid where each cell has coordinates,
			// but you can't rely on them, and here's why:
			int allTilesBoundWidthInCells = (int)(allTilesBound.Width / this.tiles.CellWidth);
			int allTilesBoundHeightInCells = (int)(allTilesBound.Height / this.tiles.CellHeight);

			// ...adding tiles that are to the left and/or above the old grid...
			Vector2 newGridOrigin = new Vector2(allTilesBound.X, allTilesBound.Y);
			SizedGrid<Tile> newGrid = new SizedGrid<Tile>(this.tiles.CellWidth, this.tiles.CellHeight, 
														  allTilesBoundWidthInCells, allTilesBoundHeightInCells);

			// ...forces a change of the cell coordinates of every tile already in the grid.
			
			// ...
			// so apparently a SizedGrid has no position of its own...
			// brb
		}
	}
}
