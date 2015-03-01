using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Extensions;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers.Serializer002Types
{
	/// <summary>
	/// Contains a collection of unique tiles and all the positions they appear in a Layer.
	/// </summary>
	internal class LayerTileSaveData
	{
		internal Dictionary<TileSaveData, TilePositionCloud> Tiles { get; set; }
		private int id = 0;

		private LayerTileSaveData()
		{

		}

		public LayerTileSaveData(Layer layer)
		{
			Tiles = new Dictionary<TileSaveData, TilePositionCloud>();

			foreach (Tile tile in layer.Tiles)
			{
				TileSaveData saveData = new TileSaveData(tile);
				saveData.TileSaveID = this.id;

				if (!this.Tiles.ContainsKey(saveData))
				{
					this.Tiles.Add(saveData, new TilePositionCloud() {TileSaveID = this.id});
					this.id++;
				}
				this.Tiles[saveData].Positions.Add(tile.InitialPosition);
			}
		}

		public static LayerTileSaveData Merge(List<TileSaveData> tileData, List<TilePositionCloud> tilePositions)
		{
			tileData = tileData.OrderBy(t => t.TileSaveID).ToList();
			tilePositions = tilePositions.OrderBy(t => t.TileSaveID).ToList();

			var result = new LayerTileSaveData();
			result.Tiles = new Dictionary<TileSaveData,TilePositionCloud>();

			for (int i = 0; i < tileData.Count; i++)
			{
				result.Tiles.Add(tileData[i], tilePositions[i]); // WYLO: I don't think the deserializer is getting graphics resources correctly. Debug here and find out what exactly we're reading.
			}

			return result;
		}

		public void LoadIntoLayer(Layer layer)
		{
			foreach (var tileEntry in this.Tiles)
			{
				var tileData = tileEntry.Key;
				var tilePositions = tileEntry.Value;

				Tile tile = AssemblyManager.GetTileByFullName(tileData.TypeName);
				tile.Name = tileData.Name;
				tile.GraphicsResourceName = tile.GraphicsResourceName;
				tile.Collision = tileData.CollisionType;
				tile.InitialState = tileData.State;
				tile.Position = Vector2.Zero;
				tile.InitialPosition = Vector2.Zero;
				
				if (!(tileData.CustomData is JToken))
				{
					throw new InvalidCastException("LayerTileSaveData.LoadIntoLayer(Layer): The custom data for a tile was not in the correct format.");
				}

				tile.DeserializeCustomObjects(new JsonHelper((JToken)tileData.CustomData));

				foreach (Vector2 position in tilePositions.Positions)
				{
					Tile resultTile = tile.Clone();
					resultTile.InitialPosition = position;
					resultTile.Position = position;

					tile.Initialize(layer.Owner);
					layer.Owner.AddTile(resultTile);

					if (!layer.IsMainLayer) { layer.Tiles.Add(resultTile); }
				}
			}
		}
	}
}
