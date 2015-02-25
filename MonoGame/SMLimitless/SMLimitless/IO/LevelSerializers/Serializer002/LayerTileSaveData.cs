using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Extensions;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers.Serializer002
{
	/// <summary>
	/// Contains a collection of unique tiles and all the positions they appear in a Layer.
	/// </summary>
	internal class LayerTileSaveData
	{
		private Dictionary<TileSaveData, TilePositionCloud> tiles;

		public LayerTileSaveData(Layer layer)
		{
			tiles = new Dictionary<TileSaveData, TilePositionCloud>();

			foreach (Tile tile in layer.Tiles)
			{
				TileSaveData saveData = new TileSaveData(tile);
				if (!this.tiles.ContainsKey(saveData))
				{
					this.tiles.Add(saveData, new TilePositionCloud());
				}
				this.tiles[saveData].Positions.Add(tile.InitialPosition);
			}
		}
	}
}
