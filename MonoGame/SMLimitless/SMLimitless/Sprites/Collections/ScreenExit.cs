//-----------------------------------------------------------------------
// <copyright file="ScreenExit.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// An exit that allows players to move between sections.
    /// </summary>
    public class ScreenExit
    {
		/// <summary>
		/// Gets the position of the destination.
		/// </summary>
		public Vector2 DestinationPosition { get; private set; }

		/// <summary>
		/// Gets the index of the destination section.
		/// </summary>
		public int DestinationSectionIndex { get; private set; }

		/// <summary>
		/// Gets the behavior of this exit when the player enters it.
		/// </summary>
		public ScreenExitBehavior EntranceBehavior { get; private set; }

		/// <summary>
        /// Gets the behavior of the exit when the player leaves it.
        /// </summary>
        public ScreenExitBehavior ExitBehavior { get; private set; }

		/// <summary>
        /// Gets the position of this exit in the section.
        /// </summary>
        public Vector2 Position { get; private set; }

		/// <summary>
        /// Gets the index of this exit.
        /// </summary>
        public int SectionIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenExit"/> class.
        /// </summary>
        public ScreenExit() { }

        /// <summary>
        /// Initializes this screen exit.
        /// </summary>
        public void Initialize()
        {
        }

		/// <summary>
        /// Gets an anonymous object containing key objects of this screen exit.
        /// </summary>
        /// <returns>An anonymous object.</returns>
		[Obsolete]
        public object GetSerializableObjects()
        {
            return new
            {
                sectionIndex = this.SectionIndex,
                position = this.Position,
                entranceBehavior = this.EntranceBehavior,
                destinationSectionIndex = this.DestinationSectionIndex,
                destinationPosition = this.DestinationPosition,
                exitBehavior = this.ExitBehavior
            };
        }

        /// <summary>
        /// Returns a JSON string containing key objects of this screen exit.
        /// </summary>
        /// <returns>A valid JSON string.</returns>
		[Obsolete]
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a screen exit given a JSON string containing key objects of the exit.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
		[Obsolete]
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);
            this.SectionIndex = (int)obj["sectionIndex"];
            this.Position = obj["position"].ToVector2();
            this.EntranceBehavior = (ScreenExitBehavior)(int)obj["entranceBehavior"];
            this.DestinationSectionIndex = (int)obj["destinationSectionIndex"];
            this.DestinationPosition = obj["destinationPosition"].ToVector2();
            this.ExitBehavior = (ScreenExitBehavior)(int)obj["exitBehavior"];
        }
    }
}
