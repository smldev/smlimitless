using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Editor;
using SMLimitless.Physics;

namespace SMLimitless.IO
{
	/// <summary>
	///   Contains information about game objects relevant to the level editor.
	/// </summary>
	public sealed class EditorObjectData
	{
		private List<SpriteData> spriteData = new List<SpriteData>();
		private List<TileData> tileData = new List<TileData>();

		/// <summary>
		///   Gets a read-only list containing the information for all sprites in
		///   an assembly.
		/// </summary>
		public IReadOnlyList<SpriteData> SpriteData
		{
			get
			{
				return spriteData.AsReadOnly();
			}
		}

		/// <summary>
		///   Gets a read-only list containing the information for all tiles in
		///   an assembly.
		/// </summary>
		public IReadOnlyList<TileData> TileData
		{
			get
			{
				return tileData.AsReadOnly();
			}
		}

		/// <summary>
		///   Reads information for game objects from a JSON file.
		/// </summary>
		/// <param name="jsonPath">The path to the JSON file.</param>
		public void ReadData(string jsonPath)
		{
			string json = File.ReadAllText(jsonPath);
			JObject obj = JObject.Parse(json);

			ReadTileData((JArray)obj["tileData"]);
			ReadSpriteData((JArray)obj["spriteData"]);
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
				JObject customObjects = (JObject)sprite["customObjects"];

				spriteData.Add(new SpriteData(typeName, editorResourceName, editorTextureSourceRectangle, state, collision, customObjects));
			}
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
					int solidSides = (int)defaultState["solidSides"];
					int collisionType = (int)defaultState["collisionType"];
					string graphicsResource = (string)defaultState["graphicsResource"];
					string state = (string)defaultState["state"];
					JObject customData = (JObject)defaultState["customData"];

					var tileDefaultState = new TileDefaultState(typeName, solidSides, collisionType, graphicsResource, state, customData);
					tileDefaultStates.Add(tileDefaultState);
				}

				TileData tileData = new TileData(typeName, tileDefaultStates);
				this.tileData.Add(tileData);
			}
		}
	}
}
