using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Editor;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.IO
{
	public sealed class EditorObjectData
	{
		private List<TileData> tileData = new List<TileData>();
		private List<SpriteData> spriteData = new List<SpriteData>();

		public IReadOnlyList<TileData> TileData
		{
			get
			{
				return tileData.AsReadOnly();
			}
		}

		public IReadOnlyList<SpriteData> SpriteData
		{
			get
			{
				return spriteData.AsReadOnly();
			}
		}

		public void ReadData(string jsonPath)
		{
			string json = File.ReadAllText(jsonPath);
			JObject obj = JObject.Parse(json);

			ReadTileData((JArray)obj["tileData"]);
			ReadSpriteData((JArray)obj["spriteData"]);
		}

		private void ReadTileData(JArray tiles)
		{
			foreach (var tile in tiles)
			{
				string typeName = (string)tile["typeName"];

				var defaultStates = (JArray)tile["defaultStates"];
				List<TileDefaultState> tileDefaultStates = new List<TileDefaultState>();
				foreach (var defaultState in defaultStates)
				{
					int solidSides = (int)defaultStates["solidSides"];
					int collisionType = (int)defaultStates["collisionType"];
					string graphicsResource = (string)defaultStates["graphicsResource"];
					string state = (string)defaultStates["state"];
					JObject customData = (JObject)defaultStates["customData"];

					var tileDefaultState = new TileDefaultState(solidSides, collisionType, graphicsResource, state, customData);
					tileDefaultStates.Add(tileDefaultState);
				}

				TileData tileData = new TileData(typeName, tileDefaultStates);
				this.tileData.Add(tileData);
			}
		}

		private void ReadSpriteData(JArray sprites)
		{
			foreach (var sprite in sprites)
			{
				string typeName = (string)sprite["typeName"];
				string editorResourceName = (string)sprite["editorResourceName"];
				Rectangle editorTextureSourceRectangle = BoundingRectangle.FromSimpleString((string)sprite["editorTextureSourceRectangle"]).ToRectangle();
				int state = (int)sprite["state"];
				int collision = (int)sprite["collision"];
				JObject customObjects = (JObject)sprite["customObject"];

				spriteData.Add(new SpriteData(typeName, editorResourceName, editorTextureSourceRectangle, state, collision, customObjects));
			}
		}
	}
}
