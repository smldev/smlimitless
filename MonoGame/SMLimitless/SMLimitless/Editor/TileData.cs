using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Editor
{
	public sealed class TileData
	{
		private List<TileDefaultState> defaultStates = new List<TileDefaultState>();

		public string TypeName { get; }
		public IReadOnlyList<TileDefaultState> DefaultStates
		{
			get
			{
				return defaultStates.AsReadOnly();
			}
		}

		public TileData(string typeName, IEnumerable<TileDefaultState> defaultStates)
		{
			TypeName = typeName;
			this.defaultStates = defaultStates.ToList();
		}
	}
	
	public sealed class TileDefaultState
	{
		public string TypeName { get; }
		public int SolidSides { get; }
		public int CollisionType { get; }
		public string GraphicsResource { get; }
		public string State { get; }
		public JObject CustomData { get; }

		public TileDefaultState(string typeName, int solidStates, int collisionType, string graphicsResource, string state, JObject customData)
		{
			TypeName = typeName;
			SolidSides = solidStates;
			CollisionType = collisionType;
			GraphicsResource = graphicsResource;
			State = state;
			CustomData = customData;
		}
	}
}
