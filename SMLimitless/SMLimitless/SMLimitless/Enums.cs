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
    /// Enumerates the four cardinal directions.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Represents an invalid or default direction.
        /// </summary>
        None = 0,

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
    /// Enumerates the result of an operation that checks
    /// if more than one X or Y component in that collection
    /// are equal. Used to determine if a sprite collides with a
    /// row and/or a column of tiles.
    /// </summary>
    public enum VectorCollectionEqualityTypes
    {
        /// <summary>
        /// None of the vectors have equal components.
        /// </summary>
        NoEquality,

        /// <summary>
        /// Two or more of the vectors have equal X components.
        /// </summary>
        SomeXComponentsEqual,

        /// <summary>
        /// Two or more of the vectors have equal Y components.
        /// </summary>
        SomeYComponentsEqual,

        /// <summary>
        /// Two or more of the vectors have equal X and/or Y components.
        /// </summary>
        Both
    }
}

namespace SMLimitless.Input
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
}

namespace SMLimitless.Sprites
{
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
        /// This tile is only solid on the bottom.
        /// Sprites will pass through all the other sides.
        /// </summary>
        BottomSolid,

        /// <summary>
        /// This tile is only solid on the left side.
        /// Sprites will pass through all the other sides.
        /// </summary>
        LeftSolid,

        /// <summary>
        /// This tile is only solid on the right side.
        /// Sprites will pass through all the other sides.
        /// </summary>
        RightSolid,

        /// <summary>
        /// This tile is not solid on any side.
        /// </summary>
        Passive
    }

    /// <summary>
    /// Enumerates different modes of collision handling
    /// that sprites can choose between.
    /// </summary>
    public enum SpriteCollisionMode
    {
        /// <summary>
        /// Sprites that are colliding with solid tiles
        /// are offset such that they are moved out of the tile(s).
        /// Then, collision handler methods are called on both
        /// the sprite and the tile.
        /// </summary>
        OffsetNotify,

        /// <summary>
        /// Sprites that are colliding with solid tiles
        /// are offset such that they are moved out of the tile(s).
        /// No collision handlers are run.
        /// </summary>
        OffsetOnly,

        /// <summary>
        /// Collision handler methods are called on both the
        /// colliding sprite and colliding tile. No offsetting
        /// is performed.
        /// </summary>
        NotifyOnly,

        /// <summary>
        /// The sprite does not collide with any object.
        /// </summary>
        NoCollision
    }
}

namespace SMLimitless.Physics
{
    /// <summary>
    /// Defines the horizontal directions of left, right, and none.
    /// </summary>
    public enum HorizontalDirection : int
    {
        /// <summary>
        /// The leftward cardinal direction (negative X).
        /// </summary>
        Left = -1,

        /// <summary>
        /// No direction.
        /// </summary>
        None = 0,

        /// <summary>
        /// The rightward cardinal direction (positive X).
        /// </summary>
        Right = 1
    }

    /// <summary>
    /// Defines the vertical directions of up, down, and none.
    /// </summary>
    public enum VerticalDirection : int
    {
        /// <summary>
        /// The upward cardinal direction (negative Y).
        /// </summary>
        Up = -1,

        /// <summary>
        /// No direction.
        /// </summary>
        None = 0,

        /// <summary>
        /// The downward cardinal direction (positive Y).
        /// </summary>
        Down = 1
    }

    /// <summary>
    /// An enumeration of the collidable shapes that
    /// SML supports.
    /// </summary>
    public enum CollidableShape
    {
        /// <summary>
        /// A parallelogram with sides parallel to the coordinate axes.
        /// </summary>
        Rectangle,

        /// <summary>
        /// A rectangle split in half from corner to corner.
        /// </summary>
        RightTriangle
    }

    public enum ResolutionType
    {
        None,
        Normal,
        Slope
    }
}