using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.IO.LevelSerializers.Serializer003Types;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers
{
	internal sealed class Serializer003 : ILevelSerializer
	{
		public string SerializerVersion => "Version v0.03";

		public string Serialize(Level level) => JObject.FromObject(GetSerializableObjects(level)).ToString();

		internal object GetSerializableObjects(Level level)
		{
			return new
			{
				header = new
				{
					version = SerializerVersion,
					name = level.Name,
					author = level.Author
				},
				contentPackages = level.ContentFolderPaths,
				levelExits = GetLevelExitObjects(level),
				sections = GetSectionObjects(level),
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
					scrollSettings = GetAutoscrollSettingsObject(section),
					background = GetBackgroundObject(section),
					layers = GetLayerObjects(section),
					sprites = GetSpriteObjects(section),
					paths = GetPathObjects(section)
				});
			}

			return result;
		}

		internal object GetAutoscrollSettingsObject(Section section)
		{
			return new
			{
				scrollType = (int)section.AutoscrollSettings.ScrollType,
				speed = (section.AutoscrollSettings.ScrollType == CameraScrollType.AutoScroll) ? section.AutoscrollSettings.Speed.Serialize() : new Vector2(float.NaN).Serialize(),
				pathName = (section.AutoscrollSettings.ScrollType == CameraScrollType.AutoScrollAlongPath) ? section.AutoscrollSettings.PathName : ""
			};
		}

		internal object GetBackgroundObject(Section section)
		{
			return new
			{
				topColor = section.Background.TopColor.Serialize(),
				bottomColor = section.Background.BottomColor.Serialize(),
				layers = GetBackgroundLayerObjects(section.Background)
			};
		}

		internal object GetBackgroundLayerObjects(Background background)
		{
			List<object> result = new List<object>(background.Layers.Count);

			foreach (var layer in background.Layers)
			{
				result.Add(new
				{
					resourceName = layer.BackgroundTextureResourceName,
					scrollDirection = (int)layer.ScrollDirection,
					scrollRate = layer.ScrollRate
				});
			}

			return result;
		}

		internal object GetLayerObjects(Section section)
		{
			var result = new List<object>(section.Layers.Count);

			foreach (var layer in section.Layers)
			{
				LayerTileSaveData layerData = new LayerTileSaveData(layer);
				result.Add(new
				{
					index = layer.Index,
					name = layer.Name,
					isMainLayer = layer.IsMainLayer,
					position = layer.Tiles.Position.Serialize(),
					gridWidth = layer.Tiles.Width,
					gridHeight = layer.Tiles.Height,
					uniqueTiles = GetUniqueTileObjects(layerData),
					tilePositions = GetTilePositionCloudObjects(layerData)
				});
			}

			return result;
		}

		internal object GetUniqueTileObjects(LayerTileSaveData layerData)
		{
			var result = new List<object>(layerData.Tiles.Count);

			foreach (var uniqueTile in layerData.Tiles.Keys)
			{
				result.Add(new
				{
					id = uniqueTile.TileSaveID,
					typeName = uniqueTile.TypeName,
					solidSides = uniqueTile.SolidSides,
					name = uniqueTile.Name,
					graphicsResource = uniqueTile.GraphicsResourceName,
					state = uniqueTile.InitialState,
					customData = uniqueTile.CustomData
				});
			}

			return result;
		}

		internal object GetTilePositionCloudObjects(LayerTileSaveData layerData)
		{
			var result = new List<object>(layerData.Tiles.Count);

			foreach (var positionCloud in layerData.Tiles.Values)
			{
				result.Add(new
				{
					id = positionCloud.TileSaveID,
					positions = positionCloud.CellNumbers.SerializeCompact(sorted: true)
				});
			}

			
			return result;
		}

		internal object GetSpriteObjects(Section section)
		{
			var result = new List<object>();

			foreach (var sprite in section.Sprites)
			{
				result.Add(new
				{
					typeName = sprite.GetType().FullName,
					position = sprite.InitialPosition.Serialize(),
					isActive = sprite.IsActive,
					state = (int)sprite.InitialState,
					collision = (int)sprite.TileCollisionMode,
					name = sprite.Name,
					message = sprite.Message,
					isHostile = sprite.IsHostile,
					isMoving = sprite.IsMoving,
					direction = (int)sprite.Direction,
					customObjects = sprite.GetCustomSerializableObjects()
				});
			}

			return result;
		}

		internal object GetPathObjects(Section section)
		{
			var result = new List<object>(section.Paths.Count);

			foreach (var path in section.Paths)
			{
				result.Add(new
				{
					points = path.Points.SerializeCompact()
				});
			}

			return result;
		}

		internal object GetLevelExitObjects(Level level)
		{
			var result = new List<object>(level.LevelExits.Count);

			foreach (var levelExit in level.LevelExits)
			{
				result.Add(new
				{
					exitIndex = levelExit.ExitIndex,
					exitDirection = (int)levelExit.ExitDirection,
					objectName = levelExit.ObjectName
				});
			}

			return result;
		}

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
				throw new ArgumentException("The deserialization process encountered an error.", ex);
			}

			if ((string)obj["header"]["version"] != SerializerVersion)
			{
				throw new ArgumentException($"This level was created with a different version of the serializer. Expected {SerializerVersion}, got {(string)obj["header"]["version"]}.");
			}

			result.Name = (string)obj["header"]["name"];
			result.Author = (string)obj["header"]["author"];
			result.EventScript.Script = (string)obj["script"];

			JArray contentObjects = (JArray)obj["contentPackages"];
			JArray sectionObjects = (JArray)obj["sections"];
			JArray levelExitObjects = (JArray)obj["levelExits"];

			result.ContentFolderPaths = contentObjects.ToObject<List<string>>();
			Content.ContentPackageManager.AddPackageFromFolder(result.ContentFolderPaths[0]); // TODO: make this not hardcoded

			result.Sections = DeserializeSections(sectionObjects, result);
			result.LevelExits = DeserializeLevelExits(levelExitObjects);

			result.ActiveSection = result.Sections.First(s => s.Index == 0);

			return result;
		}

		internal List<LevelExit> DeserializeLevelExits(JArray levelExitObjects)
		{
			List<LevelExit> result = new List<LevelExit>();

			foreach (var entry in levelExitObjects)
			{
				LevelExit levelExit = new LevelExit();

				levelExit.ExitIndex = (int)entry["exitIndex"];
				levelExit.ExitDirection = (Direction)(int)entry["exitDirection"];
				levelExit.ObjectName = (string)entry["objectName"];

				result.Add(levelExit);
			}

			return result;
		}

		internal List<Section> DeserializeSections(JArray sectionObjects, Level ownerLevel)
		{
			List<Section> result = new List<Section>();

			foreach (var entry in sectionObjects)
			{
				Section section = new Section(ownerLevel);

				section.Index = (int)entry["index"];
				section.Name = (string)entry["name"];
				section.Bounds = BoundingRectangle.FromSimpleString((string)entry["bounds"]);
				section.AutoscrollSettings = DeserializeAutoscrollSettings((JObject)entry["scrollSettings"]);
				section.Background = DeserializeBackground((JObject)entry["background"], section);

				JArray layersData = (JArray)entry["layers"];
				JArray spritesData = (JArray)entry["sprites"];
				JArray pathsData = (JArray)entry["paths"];

				section.Layers = DeserializeLayers(layersData, section);
				section.Sprites = DeserializeSprites(spritesData);
				section.Paths = DeserializePaths(pathsData);

				section.IsLoaded = true;
				result.Add(section);
			}

			return result;
		}

		internal SectionAutoscrollSettings DeserializeAutoscrollSettings(JObject obj)
		{
			SectionAutoscrollSettings result = new SectionAutoscrollSettings();

			result.ScrollType = (CameraScrollType)(int)obj["scrollType"];
			if (result.ScrollType == CameraScrollType.AutoScroll) { obj["speed"].ToVector2(); }
			if (result.ScrollType == CameraScrollType.AutoScrollAlongPath) { result.PathName = (string)obj["pathName"]; }

			return result;
		}

		internal Background DeserializeBackground(JObject obj, Section ownerSection)
		{
			Background result = new Background(ownerSection);

			result.TopColor = obj["topColor"].ToColor();
			result.BottomColor = obj["bottomColor"].ToColor();

			JArray layersData = (JArray)obj["layers"];
			result.Layers = DeserializeBackgroundLayers(layersData, ownerSection.Camera, ownerSection.Bounds);

			return result;
		}

		internal List<BackgroundLayer> DeserializeBackgroundLayers(JArray layers, Camera2D ownerCamera, BoundingRectangle ownerBounds)
		{
			List<BackgroundLayer> result = new List<BackgroundLayer>(layers.Count);

			foreach (var layerData in layers)
			{
				BackgroundLayer layer = new BackgroundLayer(ownerCamera, ownerBounds);

				string resourceName = (string)layerData["resourceName"];
				BackgroundScrollDirection direction = (BackgroundScrollDirection)(int)layerData["scrollDirection"];
				float scrollRate = (float)layerData["scrollRate"];

				layer.BackgroundTextureResourceName = resourceName;
				layer.ScrollDirection = direction;
				layer.ScrollRate = scrollRate;

				result.Add(layer);
			}

			return result;
		}

		internal List<Layer> DeserializeLayers(JArray layerObjects, Section ownerSection)
		{
			List<Layer> result = new List<Layer>();

			foreach (var entry in layerObjects)
			{
				Layer layer = new Layer(ownerSection, (bool)entry["isMainLayer"]);

				layer.Index = (int)entry["index"];
				layer.Name = (string)entry["name"];
				layer.IsMainLayer = (bool)entry["isMainLayer"];

				JArray tileDataArray = (JArray)entry["uniqueTiles"];
				JArray positionsArray = (JArray)entry["tilePositions"];
				Vector2 gridSize = new Vector2((int)entry["gridWidth"], (int)entry["gridHeight"]);
				Vector2 position = entry["position"].ToVector2();
				layer.Tiles = new SizedGrid<Tile>(position, (int)GameServices.GameObjectSize.X, (int)GameServices.GameObjectSize.Y, (int)gridSize.X, (int)gridSize.Y);
				layer.Position = position;
				var uniqueTiles = DeserializeTileSaveData(tileDataArray);
				var tilePositions = DeserializeTilePositionClouds(positionsArray);
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
				saveData.SolidSides = (int)entry["solidSides"];
				saveData.Name = (string)entry["name"];
				saveData.GraphicsResourceName = (string)entry["graphicsResource"];
				saveData.InitialState = (string)entry["state"];
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
				positionCloud.CellNumbers = PointExtensions.DeserializeCompact((string)entry["positions"]);

				result.Add(positionCloud);
			}

			return result;
		}

		internal SparseCellGrid<Sprite> DeserializeSprites(JArray spriteObjects)
		{
			SparseCellGrid<Sprite> result = new SparseCellGrid<Sprite>(new Microsoft.Xna.Framework.Vector2(64f));

			foreach (var entry in spriteObjects)
			{
				string typeName = (string)entry["typeName"];
				Sprite sprite = AssemblyManager.GetSpriteByFullName(typeName);

				sprite.InitialPosition = sprite.Position = entry["position"].ToVector2();
				sprite.IsActive = (bool)entry["isActive"];
				sprite.InitialState = sprite.State = (SpriteState)(int)entry["state"];
				sprite.TileCollisionMode = (SpriteCollisionMode)(int)entry["collision"];
				sprite.Name = (string)entry["name"];
				sprite.Message = (string)entry["message"];
				sprite.IsHostile = (bool)entry["isHostile"];
				sprite.IsMoving = (bool)entry["isMoving"];
				sprite.Direction = (SpriteDirection)(int)entry["direction"];
				sprite.DeserializeCustomObjects(new JsonHelper(entry["customObjects"]));

				result.Add(sprite);
			}

			return result;
		}

		internal List<Path> DeserializePaths(JArray pathObjects)
		{
			List<Path> result = new List<Path>();

			foreach (var entry in pathObjects)
			{
				Path path = new Path(null);

				JArray points = (JArray)entry["points"];
				foreach (var point in points)
				{
					path.Points.Add(point.ToVector2());
				}
				result.Add(path);
			}

			return result;
		}
	}
}
