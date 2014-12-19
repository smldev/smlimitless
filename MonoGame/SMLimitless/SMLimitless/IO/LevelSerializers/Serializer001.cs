using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
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
	}
}
