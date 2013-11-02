//-----------------------------------------------------------------------
// <copyright file="ExitData.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Sprites.Collections.Structures
{
    /// <summary>
    /// Represents an exit. This class is used in saving levels to file.
    /// </summary>
    internal sealed class ExitData
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
        /// Initializes a new instance of the <see cref="ExitData"/> class.
        /// </summary>
        /// <param name="index">The value of the ExitIndex property.</param>
        /// <param name="direction">The value of the ExitDirection property.</param>
        /// <param name="name">The value of ObjectName property.</param>
        public ExitData(int index, Direction direction, string name)
        {
            this.ExitIndex = index;
            this.ExitDirection = direction;
            this.ObjectName = name;
        }
    }
}
