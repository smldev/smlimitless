using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.IO.LevelSerializers.Serializer002Types;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers
{
	internal sealed class Serializer002 : ILevelSerializer
	{
		private Serializer001 baseSerializer;

		public string SerializerVersion
		{
			get
			{
				return "Version v0.02";
			}
		}

		public Serializer002()
		{
			this.baseSerializer = new Serializer001(); // why are these not static
		}

		#region Object Serializers
		public string Serialize(Level level)
		{
			return JObject.FromObject(this.GetSerializableObjects(level)).ToString();
		}

		internal object GetSerializableObjects(Level level)
		{
			return new
			{
				header = new
				{
					version = this.SerializerVersion,
					name = level.Name,
					author = level.Author
				},
				contentPackages = level.ContentFolderPaths,
				levelExits = this.baseSerializer.GetLevelExitObjects(level),
				sections = this.GetSectionObjects(level),
				script = level.EventScript.Script
			};
		}

		internal object GetSectionObjects(Level level)
		{
			var result = new List<object>(level.Sections.Count);

			foreach (var section in level.Sections)
			{
				result.Add(new
				{
					index = section.Index,
					name = section.Name,
					bounds = section.Bounds.Serialize(),
					scrollType = (int)section.ScrollType,
					autoscrollSpeed = (section.ScrollType == CameraScrollType.AutoScroll) ? section.AutoscrollSpeed.Serialize() : new Vector2(float.NaN).Serialize(),
					autoscrollPathName = (section.ScrollType == CameraScrollType.AutoScrollAlongPath) ? section.AutoscrollPathName : null,
					background = this.baseSerializer.GetBackgroundObject(section),
					layers = this.GetLayerObjects(section),
					sprites = this.baseSerializer.GetSpriteObjects(section),
					paths = this.baseSerializer.GetPathObjects(section)
				});
			}

			return result;
		}
		
		internal object GetLayerObjects(Section section)
		{
			var result = new List<object>(section.Layers.Count);

			foreach (var layer in section.Layers)
			{
				LayerTileSaveData tileData = new LayerTileSaveData(layer);
				result.Add(new
				{
					index = layer.Index,
					name = layer.Name,
					isMainLayer = layer.IsMainLayer,
					anchorPoint = (layer.AnchorPosition != LayerAnchorPosition.Invalid) ? layer.AnchorPoint.Serialize() : new Vector2(float.NaN, float.NaN).Serialize(),
					anchorPosition = (int)layer.AnchorPosition,
					uniqueTiles = this.GetUniqueTileObjects(tileData),
					tilePositions = this.GetTilePositionCloudObjects(tileData)
				});
			}

			return result;
		}

		internal object GetUniqueTileObjects(LayerTileSaveData tileData)
		{
			var result = new List<Object>(tileData.Tiles.Count);

			foreach (var uniqueTile in tileData.Tiles.Keys)
			{
				result.Add(new
				{
					id = uniqueTile.TileSaveID,
					typeName = uniqueTile.TypeName,
					collisionType = (int)uniqueTile.CollisionType,
					name = uniqueTile.Name,
					graphicsResource = uniqueTile.GraphicsResource,
					state = uniqueTile.State,
					customData = uniqueTile.CustomData
				});
			}

			return result;
		}

		internal object GetTilePositionCloudObjects(LayerTileSaveData tileData)
		{
			var result = new List<object>(tileData.Tiles.Count);

			foreach (var positionCloud in tileData.Tiles.Values)
			{
				result.Add(new
				{
					id = positionCloud.TileSaveID,
					positions = positionCloud.Positions.SerializeCompact()
				});
			}

			return result;
		}
		#endregion

		#region Object Deserializers
		public Level Deserialize(string json)
		{
			JObject obj = null;
			Level result = new Level();

			try
			{
				obj = JObject.Parse(json);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Serializer002.Deserialize(string): The deserialization process encountered an error.", ex);
			}

			// Check if the versions match.
			if ((string)obj["header"]["version"] != this.SerializerVersion)
			{
				throw new ArgumentException(string.Format("Serializer002.Deserialize(string): This level was created with a different version of the serializer. Expected {0}, got {1}.", Level.SerializerVersion, (string)obj["header"]["version"]));
			}

			// Deserialize the root objects first.
			result.Name = (string)obj["header"]["name"];
			result.Author = (string)obj["header"]["author"];
			result.EventScript.Script = (string)obj["script"];

			// Then deserialize the nested objects.
			JArray contentObjects = (JArray)obj["contentPackages"];
			JArray sectionObjects = (JArray)obj["sections"];
			JArray levelExitObjects = (JArray)obj["levelExits"];

			result.ContentFolderPaths = contentObjects.ToObject<List<string>>();
			Content.ContentPackageManager.AddPackageFromFolder(System.IO.Directory.GetCurrentDirectory() + @"\" + result.ContentFolderPaths[0]); // oh, hardcoded to local dir

			result.Sections = this.DeserializeSections(sectionObjects, result);
			result.LevelExits = this.baseSerializer.DeserializeLevelExits(levelExitObjects);

			result.ActiveSection = result.Sections.First(s => s.Index == 0);

			return result;
		}

		internal List<Section> DeserializeSections(JArray sectionObjects, Level ownerLevel)
		{
			var result = new List<Section>();

			foreach (var entry in sectionObjects)
			{
				Section section = new Section(ownerLevel);

				section.Index = (int)entry["index"];
				section.Name = (string)entry["name"];
				section.Bounds = BoundingRectangle.FromSimpleString((string)entry["bounds"]);
				section.ScrollType = (CameraScrollType)(int)entry["scrollType"];
				section.AutoscrollSpeed = entry["autoscrollSpeed"].ToVector2();
				section.AutoscrollPathName = (string)entry["autoscrollPathName"];
				section.Background = this.baseSerializer.DeserializeBackground((JObject)entry["background"], section);

				JArray layersData = (JArray)entry["layers"];
				JArray spritesData = (JArray)entry["sprites"];
				JArray pathsData = (JArray)entry["paths"];

				section.Layers = this.DeserializeLayers(layersData, section);
				section.Sprites = this.baseSerializer.DeserializeSprites(spritesData);
				section.Paths = this.baseSerializer.DeserializePaths(pathsData);

				section.IsSectionLoaded = true;
				result.Add(section);
			}

			return result;
		}

		internal List<Layer> DeserializeLayers(JArray layerObjects, Section ownerSection)
		{
			var result = new List<Layer>();

			foreach (var entry in layerObjects)
			{
				Layer layer = new Layer(ownerSection);

				layer.Index = (int)entry["index"];
				layer.Name = (string)entry["name"];
				layer.IsMainLayer = (bool)entry["isMainLayer"];
				layer.AnchorPosition = (LayerAnchorPosition)(int)entry["anchorPosition"];

				if (layer.IsMainLayer)
				{
					ownerSection.SetMainLayer(layer);
				}

				JArray tileDataArray = (JArray)entry["uniqueTiles"];
				JArray positionsArray = (JArray)entry["tilePositions"];
				var uniqueTiles = this.DeserializeTileSaveData(tileDataArray);
				var tilePositions = this.DeserializeTilePositionClouds(positionsArray);
				LayerTileSaveData layerTileData = LayerTileSaveData.Merge(uniqueTiles, tilePositions);
				layerTileData.LoadIntoLayer(layer);

				result.Add(layer);
			}

			return result;
		}

		internal List<TileSaveData> DeserializeTileSaveData(JArray tileSaveObjects)
		{
			var result = new List<TileSaveData>();

			foreach (var entry in tileSaveObjects)
			{
				TileSaveData saveData = new TileSaveData();

				saveData.TileSaveID = (int)entry["id"];
				saveData.TypeName = (string)entry["typeName"];
				saveData.CollisionType = (TileCollisionType)(int)entry["collisionType"];
				saveData.Name = (string)entry["name"];
				saveData.GraphicsResource = (string)entry["graphicsResouce"];
				saveData.State = (string)entry["state"];
				saveData.CustomData = entry["customData"];

				result.Add(saveData);
			}
			
			return result;
		}

		internal List<TilePositionCloud> DeserializeTilePositionClouds(JArray positionObjects)
		{
			var result = new List<TilePositionCloud>();

			foreach (var entry in positionObjects)
			{
				TilePositionCloud positionCloud = new TilePositionCloud();

				positionCloud.TileSaveID = (int)entry["id"];
				positionCloud.Positions = ((string)entry["positions"]).DeserializeCompact();

				result.Add(positionCloud);
			}

			return result;
		}
		#endregion
	}
}
