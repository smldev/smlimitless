using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers.Serializer003Types
{
	internal sealed class LayerTileSaveData
	{
		internal Dictionary<TileSaveData, TilePositionCloud> Tiles { get; set; }
		private int id = 0;

		private LayerTileSaveData()
		{

		}

		public LayerTileSaveData(Layer layer)
		{
			Tiles = new Dictionary<TileSaveData, TilePositionCloud>();
			HashSet<TileSaveData> tilesProcessedSoFar = new HashSet<TileSaveData>();
			var tileGrid = layer.Tiles;
			
			foreach (var gridCellWithTile in tileGrid.EnumerateItemsWithGridCells())
			{
				TileSaveData saveData = new TileSaveData(gridCellWithTile.Item3);
				saveData.TileSaveID = id;

				if (!Tiles.ContainsKey(saveData))
				{
					Tiles.Add(saveData, new TilePositionCloud() { TileSaveID = id });
					id++;
				}

				Vector2 storedPosition = new Vector2(gridCellWithTile.Item1 * GameServices.GameObjectSize.X, gridCellWithTile.Item2 * GameServices.GameObjectSize.Y);
				if (gridCellWithTile.Item3.Position != storedPosition) { System.Diagnostics.Debugger.Break(); }

				Tiles[saveData].CellNumbers.Add(new Point(x: gridCellWithTile.Item1, y: gridCellWithTile.Item2));
			}
		}

		public static LayerTileSaveData Merge(List<TileSaveData> tileData, List<TilePositionCloud> tilePositions)
		{
			tileData = tileData.OrderBy(t => t.TileSaveID).ToList();
			tilePositions = tilePositions.OrderBy(t => t.TileSaveID).ToList();

			var result = new LayerTileSaveData();
			result.Tiles = new Dictionary<TileSaveData, TilePositionCloud>();

			for (int i = 0; i < tileData.Count; i++)
			{
				result.Tiles.Add(tileData[i], tilePositions[i]);
			}

			return result;
		}

		public void LoadIntoLayer(Layer layer)
		{
			foreach (var tileEntry in Tiles)
			{
				var tileData = tileEntry.Key;
				var tilePositions = tileEntry.Value;

				Tile tile = AssemblyManager.GetTileByFullName(tileData.TypeName);
				tile.Name = tileData.Name;
				tile.GraphicsResourceName = tileData.GraphicsResourceName;
				tile.InitialState = tileData.InitialState;
				tile.Position = Vector2.Zero;
				tile.InitialPosition = Vector2.Zero;
				
				if (tile.TileShape == Physics.CollidableShape.Rectangle)
				{
					tile.RectSolidSides = (TileRectSolidSides)tileData.SolidSides;
				}
				else
				{
					tile.TriSolidSides = (TileTriSolidSides)tileData.SolidSides;
				}

				if (!(tileData.CustomData is JToken))
				{
					throw new InvalidCastException("LayerTileSaveData.LoadIntoLayer(Layer): The custom data for a tile was not in the correct format.");
				}

				tile.DeserializeCustomObjects(new JsonHelper((JToken)tileData.CustomData));

				List<Tile> tilesToAdd = new List<Tile>();

				foreach (Point tileNumber in tilePositions.CellNumbers)
				{
					float xPosition = layer.Tiles.Position.X + (tileNumber.X * layer.Tiles.CellWidth);
					float yPosition = layer.Tiles.Position.Y + (tileNumber.Y * layer.Tiles.CellHeight);

					Tile resultTile = tile.Clone();
					resultTile.InitialPosition = resultTile.Position = new Vector2(xPosition, yPosition);

					resultTile.Initialize(layer.Owner);
					tilesToAdd.Add(resultTile);
				}

				layer.AddTiles(tilesToAdd);
			}
		}
	}
}
