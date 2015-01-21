//-----------------------------------------------------------------------
// <copyright file="Serializer001.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.IO.LevelSerializers
{
	/// <summary>
	/// Serializes and deserializes levels and their objects according to the Version 0.01 specification.
	/// </summary>
	internal class Serializer001 : ILevelSerializer
	{
		/// <summary>
		/// Gets the version of level file that this serializer can load and save
		/// </summary>
		public string SerializerVersion
		{
			get
			{
				return "Version 0.01";
			}
		}

		#region Object Serializers
		/// <summary>
		/// Creates an anonymous object that contains all the key serialization objects of a given level.
		/// </summary>
		/// <param name="level">The level from which to get the objects.</param>
		/// <returns>A collection of anonymous objects containing key objects of the level.</returns>
		private object GetSerializableObjects(Level level)
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
				levelExits = this.GetLevelExitObjects(level),
				sections = this.GetSectionObjects(level),
				script = level.EventScript.Script
			};
		}

		/// <summary>
		/// Creates a collection of anonymous objects that contain all the key serialization objects of the level exits of a given level.
		/// </summary>
		/// <param name="level">The level from which to get the level exit objects.</param>
		/// <returns>A collection of anonymous objects containing key objects of the level exits.</returns>
		private object GetLevelExitObjects(Level level)
		{
			List<object> result = new List<object>(level.LevelExits.Count);

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

		/// <summary>
		/// Creates a collection of anonymous objects that contains all the key serialization objects of all the sections of a given level.
		/// </summary>
		/// <param name="level">The level from which to get the section objects.</param>
		/// <returns>A collection of anonymous objects containing key objects of the level's sections.</returns>
		private object GetSectionObjects(Level level)
		{
			List<object> result = new List<object>(level.Sections.Count);

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
					background = this.GetBackgroundObject(section),
					layers = this.GetLayerObjects(section),
					sprites = this.GetSpriteObjects(section),
					paths = this.GetPathObjects(section)
				});
			}

			return result;
		}

		/// <summary>
		/// Creates a collection of anonymous objects that contain all the key serialization objects of all the sprites in a given section.
		/// </summary>
		/// <param name="section">The section from which to get the sprite objects.</param>
		/// <returns>A collection of anonymous objects containing key objects of the sprites.</returns>
		private object GetSpriteObjects(Section section)
		{
			List<object> result = new List<object>(section.Sprites.Count);

			foreach (var sprite in section.Sprites)
			{
				result.Add(new
				{
					typeName = sprite.GetType().FullName,
					position = sprite.InitialPosition.Serialize(),
					isActive = sprite.IsActive,
					state = (int)sprite.InitialState,
					collision = (int)sprite.CollisionMode,
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

		/// <summary>
		/// Creates an anonymous object containing the key serialization objects of a section's background.
		/// </summary>
		/// <param name="section">The section from which to get the key objects.</param>
		/// <returns>An anonymous object containing the background objects.</returns>
		private object GetBackgroundObject(Section section)
		{
			return new
			{
				topColor = section.Background.TopColor.Serialize(),
				bottomColor = section.Background.BottomColor.Serialize(),
				layers = this.GetBackgroundLayerObjects(section.Background)
			};
		}

		/// <summary>
		/// Creates a collection of anonymous objects containing key serialization objects of a given background's layers.
		/// </summary>
		/// <param name="background">The background from which to get the layer objects.</param>
		/// <returns>A collection of anonymous objects containing background layer objects.</returns>
		private object GetBackgroundLayerObjects(Background background)
		{
			List<object> result = new List<object>(background.Layers.Count);

			foreach (var backgroundLayer in background.Layers)
			{
				result.Add(new
				{
					resourceName = backgroundLayer.BackgroundTextureResourceName,
					scrollDirection = (int)backgroundLayer.ScrollDirection,
					scrollRate = backgroundLayer.ScrollRate
				});
			}

			return result;
		}

		/// <summary>
		/// Creates a collection of anonymous objects that contain all the key serialization objects of a given section's layers.
		/// </summary>
		/// <param name="section">The section from which to get the layer objects.</param>
		/// <returns>A collection of anonymous objects containing key layer objects.</returns>
		private object GetLayerObjects(Section section)
		{
			List<object> result = new List<object>(section.Layers.Count);

			foreach (var layer in section.Layers)
			{
				result.Add(new
				{
					index = layer.Index,
					name = layer.Name,
					isMainLayer = layer.IsMainLayer,
					anchorPoint = (layer.AnchorPosition != LayerAnchorPosition.Invalid) ? layer.AnchorPoint.Serialize() : new Vector2(float.NaN, float.NaN).Serialize(),
					anchorPosition = (int)layer.AnchorPosition,
					tiles = this.GetTileObjects(layer)
				});
			}

			return result;
		}

		/// <summary>
		/// Creates a collection of anonymous objects that contain key serialization objects of the tiles of a given layer.
		/// </summary>
		/// <param name="layer">The layer from which to get the tile objects.</param>
		/// <returns>A collection of anonymous objects that contain tile objects.</returns>
		private object GetTileObjects(Layer layer)
		{
			List<object> result = new List<object>(layer.Tiles.Count);

			foreach (var tile in layer.Tiles)
			{
				result.Add(new
				{
					typeName = tile.GetType().FullName,
					collisionType = (int)tile.Collision,
					name = tile.Name,
					graphicsResource = tile.GraphicsResourceName,
					position = tile.InitialPosition.Serialize(),
					state = tile.InitialState,
					customData = tile.GetCustomSerializableObjects()
				});
			}

			return result;
		}

		/// <summary>
		/// Creates a collection of anonymous objects that contain key serialization objects of a section's paths.
		/// </summary>
		/// <param name="section">The section from which to get the path objects.</param>
		/// <returns>A collection of anonymous objects that contains key path objects.</returns>
		private object GetPathObjects(Section section)
		{
			List<object> result = new List<object>(section.Paths.Count);

			foreach (var path in section.Paths)
			{
				result.Add(new
				{
					points = path.Points.Serialize()
				});
			}

			return result;
		}

		/// <summary>
		/// Creates a JSON string containing a given level.
		/// </summary>
		/// <param name="level">The level to serialize.</param>
		/// <returns>A JSON string containing a given level.</returns>
		public string Serialize(Level level)
		{
			return JObject.FromObject(this.GetSerializableObjects(level)).ToString();
		}
		#endregion

		#region Object Deserializers
		/// <summary>
		/// Creates a level from a JSON string.
		/// </summary>
		/// <param name="json">A JSON string containing a level.</param>
		/// <returns>A level created from the JSON string.</returns>
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
				throw new ArgumentException("Serializer001.Deserialize(string): The deserialization process encountered an error.", ex);
			}

			// Check if the versions match.
			if ((string)obj["header"]["version"] != this.SerializerVersion)
			{
				throw new ArgumentException(string.Format("Level.Deserialize(string): This level was created with a different version of the serializer. Expected {0}, got {1}.", Level.SerializerVersion, (string)obj["header"]["version"]));
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
			result.LevelExits = this.DeserializeLevelExits(levelExitObjects);
			
			result.ActiveSection = result.Sections.First(s => s.Index == 0);

			return result;
		}

		/// <summary>
		/// Deserializes level exits for a level.
		/// </summary>
		/// <param name="levelExitObjects">An JSON array of level exit objects.</param>
		/// <returns>A collection of level exits.</returns>
		private List<LevelExit> DeserializeLevelExits(JArray levelExitObjects)
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

		/// <summary>
		/// Deserializes sections for a level.
		/// </summary>
		/// <param name="sectionObjects">A JSON array of section objects.</param>
		/// <param name="ownerLevel">The level that owns the sections being deserialized.</param>
		/// <returns>A collection of sections.</returns>
		private List<Section> DeserializeSections(JArray sectionObjects, Level ownerLevel)
		{
			List<Section> result = new List<Section>();

			foreach (var entry in sectionObjects)
			{
				Section section = new Section(ownerLevel);

				section.Index = (int)entry["index"];
				section.Name = (string)entry["name"];
				section.Bounds = BoundingRectangle.FromSimpleString((string)entry["bounds"]);
				section.ScrollType = (CameraScrollType)(int)entry["scrollType"];
				section.AutoscrollSpeed = entry["autoscrollSpeed"].ToVector2();
				section.AutoscrollPathName = (string)entry["autoscrollPathName"];
				section.Background = this.DeserializeBackground((JObject)entry["background"], section);

				JArray layersData = (JArray)entry["layers"];
				JArray spritesData = (JArray)entry["sprites"];
				JArray pathsData = (JArray)entry["paths"];

				section.Layers = this.DeserializeLayers(layersData, section);
				section.Sprites = this.DeserializeSprites(spritesData);
				section.Paths = this.DeserializePaths(pathsData);

				section.IsSectionLoaded = true;

				result.Add(section);
			}

			return result;
		}

		/// <summary>
		/// Deserializes the background for a section.
		/// </summary>
		/// <param name="backgroundObject">A JSON object containing the background objects.</param>
		/// <param name="ownerSection">The section that owns the background being deserialized.</param>
		/// <returns>A background.</returns>
		private Background DeserializeBackground(JObject backgroundObject, Section ownerSection)
		{
			Background result = new Background(ownerSection);

			result.TopColor = backgroundObject["topColor"].ToColor();
			result.BottomColor = backgroundObject["bottomColor"].ToColor();

			JArray layersData = (JArray)backgroundObject["layers"];
			result.Layers = this.DeserializeBackgroundLayers(layersData, ownerSection.Camera, ownerSection.Bounds);

			return result;
		}

		/// <summary>
		/// Deserialized the background layers for a background.
		/// </summary>
		/// <param name="layers">A JSON array of the background layer objects.</param>
		/// <param name="ownerCamera">The camera being used in the section.</param>
		/// <param name="ownerBounds">The size of the bounds of the section.</param>
		/// <returns>A collection of background layers.</returns>
		private List<BackgroundLayer> DeserializeBackgroundLayers(JArray layers, Camera2D ownerCamera, BoundingRectangle ownerBounds)
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

		/// <summary>
		/// Deserializes the layers for a section.
		/// </summary>
		/// <param name="layerObjects">A JSON array of layer objects.</param>
		/// <param name="ownerSection">The section that owns the layers being deserialized.</param>
		/// <returns>A collection of layers.</returns>
		private List<Layer> DeserializeLayers(JArray layerObjects, Section ownerSection)
		{
			List<Layer> result = new List<Layer>();

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

				JArray tileArray = (JArray)entry["tiles"];
				var tiles = this.DeserializeTiles(tileArray);
				foreach (Tile tile in tiles)
				{
					tile.Initialize(ownerSection);
					ownerSection.AddTile(tile);

					if (!layer.IsMainLayer) { layer.Tiles.Add(tile); }
				}

				result.Add(layer);
			}

			return result;
		}

		/// <summary>
		/// Deserializes the tiles for a layer.
		/// </summary>
		/// <param name="tileObjects">A JSON array of the tile objects.</param>
		/// <returns>A collection of tiles.</returns>
		private List<Tile> DeserializeTiles(JArray tileObjects)
		{
			List<Tile> result = new List<Tile>();

			foreach (var entry in tileObjects)
			{
				string typeName = (string)entry["typeName"];
				Tile tile = AssemblyManager.GetTileByFullName(typeName);

				tile.Collision = (TileCollisionType)(int)entry["collisionType"];
				tile.Name = (string)entry["name"];
				tile.GraphicsResourceName = (string)entry["graphicsResource"];
				tile.InitialPosition = entry["position"].ToVector2();
				tile.Position = tile.InitialPosition;
				tile.InitialState = (string)entry["state"];
				tile.State = tile.InitialState;
				tile.DeserializeCustomObjects(new JsonHelper(entry["customData"]));

				result.Add(tile);
			}

			return result;
		}

		/// <summary>
		/// Deserializes the sprites for a section.
		/// </summary>
		/// <param name="spriteObjects">A JSON array of the sprite objects.</param>
		/// <returns>A collection of sprite objects.</returns>
		private List<Sprite> DeserializeSprites(JArray spriteObjects)
		{
			List<Sprite> result = new List<Sprite>();

			foreach (var entry in spriteObjects)
			{
				string typeName = (string)entry["typeName"];
				Sprite sprite = AssemblyManager.GetSpriteByFullName(typeName);

				sprite.InitialPosition = entry["position"].ToVector2();
				sprite.Position = sprite.InitialPosition;
				sprite.IsActive = (bool)entry["isActive"];
				sprite.InitialState = (SpriteState)(int)entry["state"];
				sprite.State = sprite.InitialState;
				sprite.CollisionMode = (SpriteCollisionMode)(int)entry["collision"];
				sprite.Name = (string)entry["name"];
				sprite.Message = (string)entry["message"];
				sprite.IsHostile = (bool)entry["isHostile"];
				sprite.IsMoving = (bool)entry["isMoving"];
				sprite.Direction = (SpriteDirection)(int)entry["direction"];
				sprite.DeserializeCustomObjects(new JsonHelper(entry["customObject"]));

				result.Add(sprite);
			}
			
			return result;
		}

		/// <summary>
		/// Deserializes the paths for a section.
		/// </summary>
		/// <param name="pathObjects">A JSON array of the path objects.</param>
		/// <returns>A collection of paths.</returns>
		private List<Path> DeserializePaths(JArray pathObjects)
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
		#endregion
	}
}
