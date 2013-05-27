//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless
{
    /// <summary>
    /// Enumerates mouse buttons for the input manager.
    /// Credit to RCIX of StackExchange Game Development.
    /// </summary>
    public enum MouseButtons
    {
        /// <summary>
        /// Represents the left mouse button.
        /// </summary>
        LeftButton,

        /// <summary>
        /// Represents the middle mouse button.
        /// </summary>
        MiddleButton,

        /// <summary>
        /// Represents the right mouse button.
        /// </summary>
        RightButton,

        /// <summary>
        /// Represents the first extra button, usually on the left.
        /// </summary>
        ExtraButton1,

        /// <summary>
        /// Represents the second extra button, usually on the right.
        /// </summary>
        ExtraButton2
    }

    /// <summary>
    /// Enumerates the directions a sprite faces when first loaded in a level, or when the sprite enters the visible area.
    /// </summary>
    public enum SpriteDirection
    {
        /// <summary>
        /// The sprite will always face the player.
        /// </summary>
        FacePlayer,

        /// <summary>
        /// The sprite will always face left.
        /// </summary>
        Left,

        /// <summary>
        /// The sprite will always face right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Enumerates the four cardinal directions.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Represents up (negative Y).
        /// </summary>
        Up,

        /// <summary>
        /// Represents down (positive Y).
        /// </summary>
        Down,

        /// <summary>
        /// Represents left (negative X).
        /// </summary>
        Left,

        /// <summary>
        /// Represents right (positive X).
        /// </summary>
        Right
    }

    /// <summary>
    /// Enumerates which sides of a tile can be collided with.
    /// </summary>
    public enum TileCollisionType
    {
        /// <summary>
        /// This tile is completely solid on all sides.
        /// Sprites that are embedded in the tiles will be forced to the left.
        /// Sprites that are embedded between a tile and a moving tile (or two moving tiles) will be destroyed.
        /// </summary>
        Solid,

        /// <summary>
        /// This tile is only solid on the top.
        /// Sprites will pass through all the other sides.
        /// </summary>
        TopSolid,

        /// <summary>
        /// This tile is not solid on any side.
        /// </summary>
        Passive
    }
}
