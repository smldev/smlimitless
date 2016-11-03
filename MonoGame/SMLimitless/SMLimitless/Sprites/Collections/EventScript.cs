//-----------------------------------------------------------------------
// <copyright file="EventScript.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   A C# script that is used to manipulate levels and the objects within them.
	/// </summary>
	public class EventScript
	{
		/// <summary>
		///   Gets or sets the text of the C# script.
		/// </summary>
		public string Script { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="EventScript" /> class.
		/// </summary>
		public EventScript()
		{
		}

		/// <summary>
		///   Loads a script, given a JSON string containing a script.
		/// </summary>
		/// <param name="json">A valid JSON string.</param>
		[Obsolete]
		public void Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);
			Script = (string)obj["script"];
		}

		/// <summary>
		///   Returns an anonymous object containing key objects of this script.
		/// </summary>
		/// <returns>An anonymous object containing the script.</returns>
		[Obsolete]
		public object GetSerializableObjects()
		{
			return new
			{
				script = Script
			};
		}

		/// <summary>
		///   Returns a JSON string from the key objects of this script.
		/// </summary>
		/// <returns>A JSON string containing this script.</returns>
		[Obsolete]
		public string Serialize()
		{
			return JObject.FromObject(GetSerializableObjects()).ToString();
		}
	}
}
