using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Editor
{
	/// <summary>
	/// A class containing information about every available tile type in an assembly, as well as each variety of each tile.
	/// </summary>
	public sealed class TileData
	{
		private List<TileDefaultState> defaultStates = new List<TileDefaultState>();

		/// <summary>
		/// Gets the full type name of the tile.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Gets a read-only list of the default states of this tile.
		/// </summary>
		public IReadOnlyList<TileDefaultState> DefaultStates
		{
			get
			{
				return defaultStates.AsReadOnly();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TileData"/> class.
		/// </summary>
		/// <param name="typeName">The full type name of the tile.</param>
		/// <param name="defaultStates">An enumerable containing default states for this tile.</param>
		public TileData(string typeName, IEnumerable<TileDefaultState> defaultStates)
		{
			TypeName = typeName;
			this.defaultStates = defaultStates.ToList();
		}
	}
	
	/// <summary>
	/// A class containing information about a certain variety of a tile.
	/// </summary>
	public sealed class TileDefaultState
	{
		/// <summary>
		/// Gets the full type name of the tile for which this <see cref="TileDefaultState"/> is a variety of.
		/// </summary>
		public string TypeName { get; }

		/// <summary>
		/// Gets an integer representing either a <see cref="Sprites.TileRectSolidSides"/> or <see cref="Sprites.TileTriSolidSides"/> enum instance, depending on what shape the tile has.
		/// </summary>
		public int SolidSides { get; }

		/// <summary>
		/// Gets an integer representing the <see cref="Sprites.TileCollisionType"/> enum instance for this variety.
		/// </summary>
		public int CollisionType { get; }

		/// <summary>
		/// Gets the name of the graphics resource for this variety of tile.
		/// </summary>
		public string GraphicsResource { get; }

		/// <summary>
		/// Gets the initial state for this variety of tile.
		/// </summary>
		public string State { get; }

		/// <summary>
		/// Gets a JSON object containing this variety's custom data.
		/// </summary>
		public JObject CustomData { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TileDefaultState"/> class.
		/// </summary>
		/// <param name="typeName">The full type name of the tile for which this <see cref="TileDefaultState"/> instance is a variety of.</param>
		/// <param name="solidStates">An integer representing either a <see cref="Sprites.TileRectSolidSides"/> or <see cref="Sprites.TileTriSolidSides"/> enum instance, depending on what shape the tile has.</param>
		/// <param name="collisionType">An integer representing the <see cref="Sprites.TileCollisionType"/> enum instance for this variety.</param>
		/// <param name="graphicsResource">The name of the graphics resource for this variety of tile.</param>
		/// <param name="state">The initial state for this variety of tile.</param>
		/// <param name="customData">A JSON object containing this variety's custom data.</param>
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
