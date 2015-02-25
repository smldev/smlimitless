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

		#endregion
	}
}
