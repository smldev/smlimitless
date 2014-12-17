using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Interfaces
{
	/// <summary>
	/// A contract fulfilled by types that can load or save level files/instances.
	/// </summary>
	internal interface ILevelSerializer
	{
		// Notes: Any serializers will call Level.GetSerializableObjects() to actually get the objects.
		// We might need to move GetSerializableObjects() here someday, but we need to make sure we won't
		// need access to any inaccessible members of Level first, which we might.

		// Never mind, we need to move GetSerializeObjects() in here after all.

		/// <summary>
		/// Gets a string that contains the serializer version that this type implements.
		/// </summary>
		string SerializerVersion { get; }

		/// <summary>
		/// Returns a string representing the level as a JSON file.
		/// This string can be saved to disk as a level file.
		/// </summary>
		/// <param name="level">The level to serialize.</param>
		/// <returns>A string representing the level as a JSON file.</returns>
		string Serialize(Level level);

		/// <summary>
		/// Returns a fully-loaded Level instance given a string containing a level as a JSON file.
		/// </summary>
		/// <param name="json">A string containing a level as a JSON file.</param>
		/// <returns>A fully-loaded level instance.</returns>
		Level Deserialize(string json);
	}
}
