//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
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

namespace SMLimitless.Debug
{
	/// <summary>
	/// Enumerates the different levels of log messages.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Represents a message that is merely informational in nature;
		/// messages of this variety provide useful information for debugging,
		/// but are not particularly dire.
		/// </summary>
		Information,

		/// <summary>
		/// Represents a message that conveys a warning about the state of the
		/// game or other components.
		/// </summary>
		Warning,

		/// <summary>
		/// Represnts a message that notifies about an error that occurred in
		/// the game.
		/// </summary>
		Error
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
	[Obsolete]
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
	/// Enumerates the solid sides of a rectangular tile.
	/// </summary>
	[Flags]
	public enum TileRectSolidSides
	{
		/// <summary>
		/// No side is solid.
		/// </summary>
		Passive = 0x00,

		/// <summary>
		/// The top side is solid.
		/// </summary>
		Top = 0x01,

		/// <summary>
		/// The bottom side is solid.
		/// </summary>
		Bottom = 0x02,

		/// <summary>
		/// The left side is solid.
		/// </summary>
		Left = 0x04,
		
		/// <summary>
		/// The right side is solid.
		/// </summary>
		Right = 0x08,
	}

	/// <summary>
	/// Enumerates the solid sides of a sloped tile.
	/// </summary>
	[Flags]
	public enum TileTriSolidSides
	{
		/// <summary>
		/// No side is solid.
		/// </summary>
		Passive = 0x00,

		/// <summary>
		/// The sloped side is solid.
		/// </summary>
		Slope = 0x01,

		/// <summary>
		/// The vertical side is solid.
		/// </summary>
		VerticalLeg = 0x02,

		/// <summary>
		/// The horizontal side is solid.
		/// </summary>
		HorizontalLeg = 0x04
	}

	[Flags]
	public enum TileAdjacencyFlags
	{
		NoAdjacentSlopes = 0x00,
		SlopeOnLeft = 0x01,
		SlopeOnRight = 0x02
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

    /// <summary>
    /// Enumerates the different states of sprites.
    /// </summary>
    public enum SpriteState
    {
        /// <summary>
        /// The default state.
        /// </summary>
        Default,

        /// <summary>
        /// Sprites in this state have been temporarily stopped by the player and are usually harmless.
        /// </summary>
        Stunned,

        /// <summary>
        /// Sprites in this state are projectiles that can render damage to other sprites.
        /// </summary>
        Projectile,

        /// <summary>
        /// Sprites in this state are fire projectiles that can render damage to other sprites.
        /// </summary>
        ProjectileFire,

        /// <summary>
        /// Sprites in this state are ice projectiles that can freeze other sprites.
        /// </summary>
        ProjectileIce,

        /// <summary>
        /// Sprites in this state are dead and should not be interacted with.
        /// </summary>
        Dead
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