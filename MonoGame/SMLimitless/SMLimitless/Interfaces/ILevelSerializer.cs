//-----------------------------------------------------------------------
// <copyright file="ILevelSerializer.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Interfaces
{
	/// <summary>
	///   A contract fulfilled by types that can load or save level files/instances.
	/// </summary>
	internal interface ILevelSerializer
	{
		/// <summary>
		///   Gets a string that contains the serializer version that this type implements.
		/// </summary>
		string SerializerVersion { get; }

		/// <summary>
		///   Returns a fully-loaded Level instance given a string containing a
		///   level as a JSON file.
		/// </summary>
		/// <param name="json">A string containing a level as a JSON file.</param>
		/// <returns>A fully-loaded level instance.</returns>
		Level Deserialize(string json);

		/// <summary>
		///   Returns a string representing the level as a JSON file. This string
		///   can be saved to disk as a level file.
		/// </summary>
		/// <param name="level">The level to serialize.</param>
		/// <returns>A string representing the level as a JSON file.</returns>
		string Serialize(Level level);
	}
}
