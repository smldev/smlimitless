//-----------------------------------------------------------------------
// <copyright file="LevelExit.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   Represents an exit. This class is used in saving levels to file.
	/// </summary>
	internal sealed class LevelExit
	{
		/// <summary>
		///   Gets or sets the direction of this exit. When this exit is cleared,
		///   hidden tiles on the world map will be revealed in this direction.
		/// </summary>
		public Direction ExitDirection { get; internal set; }

		/// <summary>
		///   Gets or sets the index of the exit. This starts at 0 for the first
		///   exit and increments for each exit after.
		/// </summary>
		public int ExitIndex { get; internal set; }

		/// <summary>
		///   Gets or sets the name of the sprite serving as the exit. If the
		///   sprite has no name, a name will be provided for it.
		/// </summary>
		public string ObjectName { get; internal set; }

		/// <summary>
		///   Loads a level exit, given a JSON string containing key objects of
		///   the exit.
		/// </summary>
		/// <param name="json">A valid JSON string.</param>
		[Obsolete]
		public void Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);

			ExitIndex = (int)obj["exitIndex"];
			ExitDirection = (Direction)(int)obj["exitDirection"];
			ObjectName = (string)obj["objectName"];
		}

		/// <summary>
		///   Gets an anonymous object containing key objects of this exit.
		/// </summary>
		/// <returns>
		///   An anonymous object containing key objects of this exit.
		/// </returns>
		[Obsolete]
		public object GetSerializableObjects()
		{
			return new
			{
				exitIndex = ExitIndex,
				exitDirection = (int)ExitDirection,
				objectName = ObjectName
			};
		}

		/// <summary>
		///   Creates a JSON string from the key objects of this exit.
		/// </summary>
		/// <returns>A valid JSON string.</returns>
		[Obsolete]
		public string Serialize()
		{
			return JObject.FromObject(GetSerializableObjects()).ToString();
		}
	}
}
