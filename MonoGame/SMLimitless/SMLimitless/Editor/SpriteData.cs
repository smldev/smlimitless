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
	public sealed class SpriteData
	{
		public string TypeName { get; }
		public string EditorResourceName { get; }
		public Rectangle EditorTextureSourceRectangle { get; }
		public int State { get; }
		public int Collision { get; }
		public JObject CustomData { get; }

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
