//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="The Limitless Development Team">
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
        // TODO: turn this into a flags enum [ Top | Bottom | Left | Right | Slope ]

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

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// Enumerates the ways a camera scrolls through a section.
    /// </summary>
    public enum CameraScrollType
    {
        /// <summary>
        /// The camera freely moves horizontally and vertically.
        /// </summary>
        FreelyMoving,
        
        /// <summary>
        /// The camera is fixed at a certain position.
        /// </summary>
        Fixed,

        /// <summary>
        /// The camera only moves horizontally.
        /// </summary>
        HorizontalOnly,

        /// <summary>
        /// The camera only moves horizontally unless
        /// the player is at running speed or there
        /// are multiple players at once.
        /// </summary>
        HorizontalUnlessRunning,

        /// <summary>
        /// The camera only moves vertically.
        /// </summary>
        VerticalOnly,

        /// <summary>
        /// The camera is moving at a fixed rate at a certain direction.
        /// </summary>
        AutoScroll,

        /// <summary>
        /// The camera is moving along a predetermined path.
        /// </summary>
        AutoScrollAlongPath
    }

    /// <summary>
    /// Enumerates how players interact with
    /// screen exits and entrances.
    /// </summary>
    public enum ScreenExitBehavior
    {
        /// <summary>
        /// The default behavior.
        /// This is considered an invalid state.
        /// </summary>
        Default,

        /// <summary>
        /// The player enters the exit by pressing Down
        /// while within the exit.
        /// </summary>
        PipeDown,

        /// <summary>
        /// The player enters the exit by jumping to it
        /// and pressing Up while within the exit.
        /// </summary>
        PipeUp,

        /// <summary>
        /// The player enters the exit by walking and
        /// pressing Left while within the exit.
        /// </summary>
        PipeLeft,

        /// <summary>
        /// The player enters the exit by walking
        /// and pressing Right while within the exit.
        /// </summary>
        PipeRight,

        /// <summary>
        /// The players enters the exit by pressing Up
        /// while within the exit.
        /// </summary>
        DoorEnter
    }

    /// <summary>
    /// Enumerates the direction in which
    /// a section background scrolls.
    /// </summary>
    public enum BackgroundScrollDirection
    {
        /// <summary>
        /// The background remains fixed on the screen.
        /// </summary>
        /// <remarks>
        /// If the background is larger than the screen,
        /// the background will shift slightly as the player
        /// moves through the level. If the camera is at the
        /// top of the level, the top of the background will
        /// be displayed. Likewise, if the camera is at the
        /// bottom of the level, the bottom of the background
        /// will be displayed. This applies across both axes.
        /// </remarks>
        Fixed,

        /// <summary>
        /// The background will scroll and repeat horizontally.
        /// </summary>
        /// <remarks>
        /// If the background is taller than the screen,
        /// the background will shift slightly as the players
        /// moves vertically through the level. If the camera is at the
        /// top of the level, the top of the background will
        /// be displayed. Likewise, if the camera is at the
        /// bottom of the level, the bottom of the background
        /// will be displayed.
        /// </remarks>
        Horizontal,

        /// <summary>
        /// The background will scroll and repeat vertically.
        /// </summary>
        /// <remarks>
        /// If the background is wider than the screen,
        /// the background will shift slightly as the players
        /// moves horizontally through the level. If the camera is
        /// at the left edge of the level, the left edge of the
        /// background will be displayed. Likewise, if the camera
        /// is at the right edge of the level, the right edge of the
        /// background will be displayed.
        /// </remarks>
        Vertical,

        /// <summary>
        /// The background will scroll and repeat along both axes.
        /// </summary>
        Both
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

    /// <summary>
    /// An enumeration of the different types of
    /// collision resolutions between tiles and sprites.
    /// </summary>
    public enum ResolutionType
    {
        /// <summary>
        /// Not a resolution (resolution of zero).
        /// </summary>
        None,

        /// <summary>
        /// A resolution between a sprite and a straight edge of a tile.
        /// </summary>
        Normal,

        /// <summary>
        /// A resolution between a sprite and the sloped side of a sloped tile.
        /// </summary>
        Slope
    }
}