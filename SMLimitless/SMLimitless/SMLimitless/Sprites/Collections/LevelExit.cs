//-----------------------------------------------------------------------
// <copyright file="ExitData.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SMLimitless.Interfaces;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// Represents an exit. This class is used in saving levels to file.
    /// </summary>
    internal sealed class LevelExit : ISerializable
    {
        /// <summary>
        /// Gets the index of the exit.
        /// This starts at 0 for the first exit and increments for each exit after.
        /// </summary>
        public int ExitIndex { get; private set; }

        /// <summary>
        /// Gets the direction of this exit.
        /// When this exit is cleared, hidden tiles on the world map
        /// will be revealed in this direction.
        /// </summary>
        public Direction ExitDirection { get; private set; }

        /// <summary>
        /// Gets the name of the sprite serving as the exit.
        /// If the sprite has no name, a name will be provided for it.
        /// </summary>
        public string ObjectName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelExit"/> class.
        /// </summary>
        public LevelExit(int exitIndex, Direction exitDirection, string objectName)
        {
            this.ExitIndex = exitIndex;
            this.ExitDirection = exitDirection;
            this.ObjectName = objectName;
        }

        public object GetSerializableObjects()
        {
            return new
            {
                exitIndex = this.ExitIndex,
                exitDirection = this.ExitDirection,
                objectName = this.ObjectName
            };
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            this.ExitIndex = (int)obj["exitIndex"];
            this.ExitDirection = (Direction)(int)obj["exitDirection"];
            this.ObjectName = (string)obj["objectName"];
        }
    }
}
