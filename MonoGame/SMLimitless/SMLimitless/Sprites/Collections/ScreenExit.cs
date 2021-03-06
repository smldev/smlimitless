﻿//-----------------------------------------------------------------------
// <copyright file="ScreenExit.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   An exit that allows players to move between sections.
	/// </summary>
	[Obsolete]
	public class ScreenExit
	{
		/// <summary>
		///   Gets the position of the destination.
		/// </summary>
		public Vector2 DestinationPosition { get; private set; }

		/// <summary>
		///   Gets the index of the destination section.
		/// </summary>
		public int DestinationSectionIndex { get; private set; }

		/// <summary>
		///   Gets the behavior of this exit when the player enters it.
		/// </summary>
		public ScreenExitBehavior EntranceBehavior { get; private set; }

		/// <summary>
		///   Gets the behavior of the exit when the player leaves it.
		/// </summary>
		public ScreenExitBehavior ExitBehavior { get; private set; }

		/// <summary>
		///   Gets the position of this exit in the section.
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		///   Gets the index of this exit.
		/// </summary>
		public int SectionIndex { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="ScreenExit" /> class.
		/// </summary>
		public ScreenExit() { }

		/// <summary>
		///   Loads a screen exit given a JSON string containing key objects of
		///   the exit.
		/// </summary>
		/// <param name="json">A valid JSON string.</param>
		[Obsolete]
		public void Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);
			SectionIndex = (int)obj["sectionIndex"];
			Position = obj["position"].ToVector2();
			EntranceBehavior = (ScreenExitBehavior)(int)obj["entranceBehavior"];
			DestinationSectionIndex = (int)obj["destinationSectionIndex"];
			DestinationPosition = obj["destinationPosition"].ToVector2();
			ExitBehavior = (ScreenExitBehavior)(int)obj["exitBehavior"];
		}

		/// <summary>
		///   Gets an anonymous object containing key objects of this screen exit.
		/// </summary>
		/// <returns>An anonymous object.</returns>
		[Obsolete]
		public object GetSerializableObjects()
		{
			return new
			{
				sectionIndex = SectionIndex,
				position = Position,
				entranceBehavior = EntranceBehavior,
				destinationSectionIndex = DestinationSectionIndex,
				destinationPosition = DestinationPosition,
				exitBehavior = ExitBehavior
			};
		}

		/// <summary>
		///   Initializes this screen exit.
		/// </summary>
		public void Initialize()
		{
		}

		/// <summary>
		///   Returns a JSON string containing key objects of this screen exit.
		/// </summary>
		/// <returns>A valid JSON string.</returns>
		[Obsolete]
		public string Serialize()
		{
			return JObject.FromObject(GetSerializableObjects()).ToString();
		}
	}
}
