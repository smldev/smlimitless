﻿using System;
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
	internal class Serializer001 : ILevelSerializer
	{
		public string SerializerVersion
		{
			get
			{
				return "Version 0.01";
			}
		}

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

		private object GetBackgroundObject(Section section)
		{
			return new
			{
				topColor = section.Background.TopColor.Serialize(),
				bottomColor = section.Background.BottomColor.Serialize(),
				layers = this.GetBackgroundLayerObjects(section.Background)
			};
		}

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
					tiles = this.GetTileObjects(layer)
				});
			}

			return result;
		}

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

		public string Serialize(Level level)
		{
			return JObject.FromObject(this.GetSerializableObjects(level)).ToString();
		}

		public Level Deserialize(string json)
		{
			return null;
			// WYLO: Port the deserializer code from the old serializer here.
			// Also, make sure that you turn those newly-internal fields into properties.
		}

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

		private List<Section> DeserializeSections(JArray sectionObjects, Level ownerLevel)
		{
			List<Section> result = new List<Section>();

			foreach (var entry in sectionObjects)
			{
				Section section = new Section(ownerLevel);
				section.Initialize();

				section.Index = (int)entry["index"];
				section.Name = (string)entry["name"];
				section.Bounds = BoundingRectangle.FromSimpleString((string)entry["bounds"]);
				section.ScrollType = (CameraScrollType)(int)entry["scrollType"];
				section.AutoscrollSpeed = entry["autoscrollSpeed"].ToVector2();
				section.AutoscrollPathName = (string)entry["autoscrollPathName"];
				section.Background = this.DeserializeBackground((JObject)entry["background"], section);
			}
		}

		private Background DeserializeBackground(JObject backgroundObject, Section section)
		{
			Background result = new Background(section);

			result.TopColor = backgroundObject["topColor"].ToColor();
			result.BottomColor = backgroundObject["bottomColor"].ToColor();

			JArray layersData = (JArray)backgroundObject["layers"];
			result.Layers = this.DeserializeBackgroundLayers(layersData, section.Camera, section.Bounds);

			return result;
		}

		private List<BackgroundLayer> DeserializeBackgroundLayers(JArray layers, Camera2D camera, BoundingRectangle bounds)
		{
			List<BackgroundLayer> result = new List<BackgroundLayer>(layers.Count);

			foreach (var layerData in layers)
			{
				BackgroundLayer layer = new BackgroundLayer(camera, bounds);

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

		private List<Layer> DeserializeLayers(JArray layerObjects, Section ownerSection)
		{
			List<Layer> result = new List<Layer>();

			foreach(var entry in layerObjects)
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

		private List<Tile> DeserializeTiles(JArray tileObjects)
		{
			List<Tile> result = new List<Tile>();

			foreach (var entry in tileObjects)
			{
				string typeName = (string)entry["typeName"];
				Tile tile = AssemblyManager.GetTileByFullName(typeName);

				tile.Collision = (TileCollisionType)(int)entry["collision"];
				tile.Name = (string)entry["name"];
				tile.GraphicsResourceName = (string)entry["graphicsResourceName"];
				tile.InitialPosition = entry["position"].ToVector2();
				tile.Position = tile.InitialPosition;
				tile.InitialState = (string)entry["state"];
				tile.State = tile.InitialState;
				tile.DeserializeCustomObjects(new JsonHelper(entry["customData"]));

				result.Add(tile);
			}

			return result;
		}

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
	}
}
