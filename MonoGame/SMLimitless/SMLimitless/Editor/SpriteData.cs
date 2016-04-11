using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Editor
{
	/// <summary>
	/// A class containing information about the default state for a sprite in an assembly, used by the editor to place instance of this sprite.
	/// </summary>
	public sealed class SpriteData
	{
		/// <summary>
		/// Gets the full type name of the sprite.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Gets the name of the graphics resource to use on the editor button.
		/// </summary>
		public string EditorResourceName { get; }

		/// <summary>
		/// Gets the rectangle on the editor button graphics resource that contains the texture to use on the editor button.
		/// </summary>
		public Rectangle EditorTextureSourceRectangle { get; }

		/// <summary>
		/// Gets an integer representing the initial value of the State property of the sprite.
		/// </summary>
		public int State { get; }

		/// <summary>
		/// Gets an integer representing the initial value of the Collision property of the sprite.
		/// </summary>
		public int Collision { get; }

		/// <summary>
		/// Gets a JSON object containing custom objects for the sprite.
		/// </summary>
		public JObject CustomData { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SpriteData"/> class.
		/// </summary>
		/// <param name="typeName">The full type name of the sprite.</param>
		/// <param name="editorResourceName">The name of the graphics resource to use on the editor button.</param>
		/// <param name="editorTextureSourceRectangle">The rectangle on the editor button graphics resource that contains the texture to use on the editor button.</param>
		/// <param name="state">An integer representing the initial value of the State property of the sprite.</param>
		/// <param name="collision">An integer representing the initial value of the Collision property of the sprite.</param>
		/// <param name="customData">A JSON object containing custom objects for the sprite.</param>
		public SpriteData(string typeName, string editorResourceName, Rectangle editorTextureSourceRectangle,
						  int state, int collision, JObject customData)
		{
			TypeName = typeName;
			EditorResourceName = editorResourceName;
			EditorTextureSourceRectangle = editorTextureSourceRectangle;
			State = state;
			Collision = collision;
			CustomData = customData;
		}
	}
}
