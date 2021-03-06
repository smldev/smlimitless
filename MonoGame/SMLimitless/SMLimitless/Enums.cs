﻿//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using SMLimitless.Editor.Attributes;

namespace SMLimitless
{
	/// <summary>
	///   Enumerates the four cardinal directions.
	/// </summary>
	public enum Direction
	{
		/// <summary>
		///   Represents an invalid or default direction.
		/// </summary>
		None = 0,

		/// <summary>
		///   Represents up (negative Y).
		/// </summary>
		Up,

		/// <summary>
		///   Represents down (positive Y).
		/// </summary>
		Down,

		/// <summary>
		///   Represents left (negative X).
		/// </summary>
		Left,

		/// <summary>
		///   Represents right (positive X).
		/// </summary>
		Right
	}
}

namespace SMLimitless.Debug
{
	/// <summary>
	///   Enumerates the different levels of log messages.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		///   Represents a message that is merely informational in nature;
		///   messages of this variety provide useful information for debugging,
		///   but are not particularly dire.
		/// </summary>
		Information,

		/// <summary>
		///   Represents a message that conveys a warning about the state of the
		///   game or other components.
		/// </summary>
		Warning,

		/// <summary>
		///   Represnts a message that notifies about an error that occurred in
		///   the game.
		/// </summary>
		Error
	}
}

namespace SMLimitless.Editor
{
	/// <summary>
	///   An enumeration of states that the level editor can be in.
	/// </summary>
	public enum EditorState
	{
		/// <summary>
		///   An object is selected and can be placed in the section.
		/// </summary>
		ObjectSelected,

		/// <summary>
		///   No object is selected, but an object can be selected by clicking it.
		/// </summary>
		Cursor,

		/// <summary>
		///   Any object clicked on will be removed from the section.
		/// </summary>
		Delete
	}
}

namespace SMLimitless.Input
{
	/// <summary>
	///   Enumerates mouse buttons for the input manager. Credit to RCIX of
	///   StackExchange Game Development.
	/// </summary>
	public enum MouseButtons
	{
		/// <summary>
		///   Represents the left mouse button.
		/// </summary>
		LeftButton,

		/// <summary>
		///   Represents the middle mouse button.
		/// </summary>
		MiddleButton,

		/// <summary>
		///   Represents the right mouse button.
		/// </summary>
		RightButton,

		/// <summary>
		///   Represents the first extra button, usually on the left.
		/// </summary>
		ExtraButton1,

		/// <summary>
		///   Represents the second extra button, usually on the right.
		/// </summary>
		ExtraButton2
	}
}

namespace SMLimitless.Interfaces
{
	/// <summary>
	///   Defines a "forward" and "backward" direction for effects. Every effect
	///   may use this differently, but the general definition is that a forward
	///   effect changes something on the screen, and a backward effect undoes
	///   that change.
	/// </summary>
	public enum EffectDirection
	{
		/// <summary>
		///   The forward direction.
		/// </summary>
		Forward,

		/// <summary>
		///   The backward direction.
		/// </summary>
		Backward
	}
}

namespace SMLimitless.Physics
{
	/// <summary>
	///   An enumeration of the collidable shapes that SML supports.
	/// </summary>
	public enum CollidableShape
	{
		/// <summary>
		///   A parallelogram with sides parallel to the coordinate axes.
		/// </summary>
		Rectangle,

		/// <summary>
		///   A rectangle split in half from corner to corner.
		/// </summary>
		RightTriangle
	}

	/// <summary>
	///   Enumerates the cardinal directions as a set of flags.
	/// </summary>
	[Flags]
	public enum FlaggedDirection
	{
		/// <summary>
		///   Represents no direction set.
		/// </summary>
		None = 0,

		/// <summary>
		///   Represents the upward cardinal direction (negative Y).
		/// </summary>
		Up = 1,

		/// <summary>
		///   Represents the downward cardinal direction (positive Y).
		/// </summary>
		Down = 2,

		/// <summary>
		///   Represents the leftward cardinal direction (negative X).
		/// </summary>
		Left = 4,

		/// <summary>
		///   Represents the rightward cardinal direction (positive X).
		/// </summary>
		Right = 8
	}

	/// <summary>
	///   Defines the horizontal directions of left, right, and none.
	/// </summary>
	public enum HorizontalDirection : int
	{
		/// <summary>
		///   The leftward cardinal direction (negative X).
		/// </summary>
		Left = -1,

		/// <summary>
		///   No direction.
		/// </summary>
		None = 0,

		/// <summary>
		///   The rightward cardinal direction (positive X).
		/// </summary>
		Right = 1
	}

	/// <summary>
	///   Enumerates the types a physics setting can be.
	/// </summary>
	public enum PhysicsSettingType
	{
		/// <summary>
		///   Represents a setting that is integral (a whole number).
		/// </summary>
		Integer,

		/// <summary>
		///   Represents a setting that is a floating point number.
		/// </summary>
		FloatingPoint,

		/// <summary>
		///   Represents a setting that is a Boolean true/false value.
		/// </summary>
		Boolean,

		/// <summary>
		///   Represents a setting that has another type not otherwise listed here.
		/// </summary>
		Other
	}

	/// <summary>
	///   Enumerates the division of the space around a rectangle.
	/// </summary>
	public enum RectangularSpaceDivision
	{
		/// <summary>
		///   The default/invalid value.
		/// </summary>
		None,

		/// <summary>
		///   The area within the rectangle.
		/// </summary>
		[EnumValue("Within!")]
		Within,

		/// <summary>
		///   The area directly above the rectangle.
		/// </summary>
		Above,

		/// <summary>
		///   The area directly below the rectangle.
		/// </summary>
		[EnumValue("Below!")]
		Below,

		/// <summary>
		///   The area directly to the left of the rectangle.
		/// </summary>
		Left,

		/// <summary>
		///   The area directly to the right of the rectangle.
		/// </summary>
		[EnumValue("Right!")]
		Right,

		/// <summary>
		///   The area above and to the left of the rectangle.
		/// </summary>
		[EnumValue("Above and to the left!")]
		AboveLeft,

		/// <summary>
		///   The area above and to the right of the rectangle.
		/// </summary>
		AboveRight,

		/// <summary>
		///   The area below and to the left of the rectangle.
		/// </summary>
		BelowLeft,

		/// <summary>
		///   The area below and to the right of the rectangle.
		/// </summary>
		BelowRight
	}

	/// <summary>
	///   An enumeration of the different types of collision resolutions between
	///   tiles and sprites.
	/// </summary>
	public enum ResolutionType
	{
		/// <summary>
		///   Not a resolution (resolution of zero).
		/// </summary>
		None,

		/// <summary>
		///   A resolution between a sprite and a straight edge of a tile.
		/// </summary>
		Normal,

		/// <summary>
		///   A resolution between a sprite and the sloped side of a sloped tile.
		/// </summary>
		Slope
	}

	/// <summary>
	///   An enumeration defining which sides of a right triangle are sloped.
	/// </summary>
	public enum RtSlopedSides
	{
		/// <summary>
		///   A default value. Do not use.
		/// </summary>
		Default,

		/// <summary>
		///   The top and left sides of the right triangle are sloped.
		/// </summary>
		TopLeft,

		/// <summary>
		///   The top and right sides of the right triangle are sloped.
		/// </summary>
		TopRight,

		/// <summary>
		///   The bottom and left sides of the right triangle are sloped.
		/// </summary>
		BottomLeft,

		/// <summary>
		///   The bottom and right sides of the right triangle are sloped.
		/// </summary>
		BottomRight
	}

	/// <summary>
	///   Defines the vertical directions of up, down, and none.
	/// </summary>
	public enum VerticalDirection : int
	{
		/// <summary>
		///   The upward cardinal direction (negative Y).
		/// </summary>
		Up = -1,

		/// <summary>
		///   No direction.
		/// </summary>
		None = 0,

		/// <summary>
		///   The downward cardinal direction (positive Y).
		/// </summary>
		Down = 1
	}
}

namespace SMLimitless.Sprites
{
	/// <summary>
	///   Enumerates the states of activity for sprites with regards to the
	///   sprite's visibility onscreen.
	/// </summary>
	public enum SpriteActiveState
	{
		/// <summary>
		///   The sprite is always active; it will never deactivate if it goes offscreen.
		/// </summary>
		AlwaysActive,

		/// <summary>
		///   The sprite is currently onscreen (within active bounds).
		/// </summary>
		Active,

		/// <summary>
		///   The sprite has deactivated by moving out of the active bounds.
		/// </summary>
		WaitingToLeaveBounds,

		/// <summary>
		///   The sprite has deactivated by having the active bounds move out of
		///   its initial location.
		/// </summary>
		Inactive
	}

	/// <summary>
	///   Enumerates different modes of collision handling that sprites can
	///   choose between.
	/// </summary>
	public enum SpriteCollisionMode
	{
		/// <summary>
		///   Sprites that are colliding with solid tiles or other sprites are
		///   offset such that they are moved out of the tile(s). Then, collision
		///   handler methods are called on both the sprite and the tile.
		/// </summary>
		OffsetNotify,

		/// <summary>
		///   Sprites that are colliding with solid tiles or other sprites are
		///   offset such that they are moved out of the tile(s). No collision
		///   handlers are run.
		/// </summary>
		OffsetOnly,

		/// <summary>
		///   Collision handler methods are called on both the colliding sprite
		///   and colliding tile. No offsetting is performed.
		/// </summary>
		NotifyOnly,

		/// <summary>
		///   The sprite does not collide with any object.
		/// </summary>
		NoCollision
	}

	/// <summary>
	///   Enumerates the directions a sprite faces when first loaded in a level,
	///   or when the sprite enters the visible area.
	/// </summary>
	public enum SpriteDirection
	{
		/// <summary>
		///   The sprite will be loaded facing the player.
		/// </summary>
		FacePlayer,

		/// <summary>
		///   The sprite will be loaded facing left.
		/// </summary>
		Left,

		/// <summary>
		///   The sprite will be loaded facing right.
		/// </summary>
		Right
	}

	/// <summary>
	///   Enumerates the different states of sprites.
	/// </summary>
	[Obsolete]
	public enum SpriteState
	{
		/// <summary>
		///   The default state.
		/// </summary>
		Default,

		/// <summary>
		///   Sprites in this state have been temporarily stopped by the player
		///   and are usually harmless.
		/// </summary>
		Stunned,

		/// <summary>
		///   Sprites in this state are projectiles that can render damage to
		///   other sprites.
		/// </summary>
		Projectile,

		/// <summary>
		///   Sprites in this state are fire projectiles that can render damage
		///   to other sprites.
		/// </summary>
		ProjectileFire,

		/// <summary>
		///   Sprites in this state are ice projectiles that can freeze other sprites.
		/// </summary>
		ProjectileIce,

		/// <summary>
		///   Sprites in this state are dead and should not be interacted with.
		/// </summary>
		Dead
	}

	/// <summary>
	///   An enumeration listing if there is a sloped tile on either side of a tile.
	/// </summary>
	[Flags]
	public enum TileAdjacencyFlags
	{
		/// <summary>
		///   There are no sloped tiles on either side of this tile.
		/// </summary>
		NoAdjacentSlopes = 0x00,

		/// <summary>
		///   There is a sloped tile to the left of this tile.
		/// </summary>
		SlopeOnLeft = 0x01,

		/// <summary>
		///   There is a sloped tile to the right of this tile.
		/// </summary>
		SlopeOnRight = 0x02
	}

	/// <summary>
	///   Enumerates which sides of a tile can be collided with.
	/// </summary>
	[Obsolete]
	public enum TileCollisionType
	{
		/// <summary>
		///   This tile is completely solid on all sides. Sprites that are
		///   embedded in the tiles will be forced to the left. Sprites that are
		///   embedded between a tile and a moving tile (or two moving tiles)
		///   will be destroyed.
		/// </summary>
		Solid,

		/// <summary>
		///   This tile is only solid on the top. Sprites will pass through all
		///   the other sides.
		/// </summary>
		TopSolid,

		/// <summary>
		///   This tile is only solid on the bottom. Sprites will pass through
		///   all the other sides.
		/// </summary>
		BottomSolid,

		/// <summary>
		///   This tile is only solid on the left side. Sprites will pass through
		///   all the other sides.
		/// </summary>
		LeftSolid,

		/// <summary>
		///   This tile is only solid on the right side. Sprites will pass
		///   through all the other sides.
		/// </summary>
		RightSolid,

		/// <summary>
		///   This tile is not solid on any side.
		/// </summary>
		Passive
	}

	/// <summary>
	///   Enumerates the solid sides of a rectangular tile.
	/// </summary>
	[Flags]
	public enum TileRectSolidSides
	{
		/// <summary>
		///   This collidable shape is not a rectangle.
		/// </summary>
		NotARectangle = 0xF0,

		/// <summary>
		///   No side is solid.
		/// </summary>
		Passive = 0x00,

		/// <summary>
		///   The top side is solid.
		/// </summary>
		Top = 0x01,

		/// <summary>
		///   The bottom side is solid.
		/// </summary>
		Bottom = 0x02,

		/// <summary>
		///   The left side is solid.
		/// </summary>
		Left = 0x04,

		/// <summary>
		///   The right side is solid.
		/// </summary>
		Right = 0x08,
	}

	/// <summary>
	///   Enumerates the solid sides of a sloped tile.
	/// </summary>
	[Flags]
	public enum TileTriSolidSides
	{
		/// <summary>
		///   This collidable shape is not a triangle.
		/// </summary>
		NotATriangle = 0xF8,

		/// <summary>
		///   No side is solid.
		/// </summary>
		Passive = 0x00,

		/// <summary>
		///   The sloped side is solid.
		/// </summary>
		Slope = 0x01,

		/// <summary>
		///   The vertical side is solid.
		/// </summary>
		VerticalLeg = 0x02,

		/// <summary>
		///   The horizontal side is solid.
		/// </summary>
		HorizontalLeg = 0x04
	}
}

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   Enumerates the direction in which a section background scrolls.
	/// </summary>
	public enum BackgroundScrollDirection
	{
		/// <summary>
		///   The background remains fixed on the screen.
		/// </summary>
		/// <remarks>
		///   If the background is larger than the screen, the background will
		///   shift slightly as the player moves through the level. If the camera
		///   is at the top of the level, the top of the background will be
		///   displayed. Likewise, if the camera is at the bottom of the level,
		///   the bottom of the background will be displayed. This applies across
		///   both axes.
		/// </remarks>
		Fixed,

		/// <summary>
		///   The background will scroll and repeat horizontally.
		/// </summary>
		/// <remarks>
		///   If the background is taller than the screen, the background will
		///   shift slightly as the players moves vertically through the level.
		///   If the camera is at the top of the level, the top of the background
		///   will be displayed. Likewise, if the camera is at the bottom of the
		///   level, the bottom of the background will be displayed.
		/// </remarks>
		Horizontal,

		/// <summary>
		///   The background will scroll and repeat vertically.
		/// </summary>
		/// <remarks>
		///   If the background is wider than the screen, the background will
		///   shift slightly as the players moves horizontally through the level.
		///   If the camera is at the left edge of the level, the left edge of
		///   the background will be displayed. Likewise, if the camera is at the
		///   right edge of the level, the right edge of the background will be displayed.
		/// </remarks>
		Vertical,

		/// <summary>
		///   The background will scroll and repeat along both axes.
		/// </summary>
		Both
	}

	/// <summary>
	///   Enumerates the ways a camera scrolls through a section.
	/// </summary>
	public enum CameraScrollType
	{
		/// <summary>
		///   The camera freely moves horizontally and vertically.
		/// </summary>
		FreelyMoving,

		/// <summary>
		///   The camera is fixed at a certain position.
		/// </summary>
		Fixed,

		/// <summary>
		///   The camera only moves horizontally.
		/// </summary>
		HorizontalOnly,

		/// <summary>
		///   The camera only moves horizontally unless the player is at running
		///   speed or there are multiple players at once.
		/// </summary>
		HorizontalUnlessRunning,

		/// <summary>
		///   The camera only moves vertically.
		/// </summary>
		VerticalOnly,

		/// <summary>
		///   The camera is moving at a fixed rate at a certain direction.
		/// </summary>
		AutoScroll,

		/// <summary>
		///   The camera is moving along a predetermined path.
		/// </summary>
		AutoScrollAlongPath
	}

	/// <summary>
	///   Enumerates the ways a player can emerge from a section exit.
	/// </summary>
	public enum ExitDestinationBehavior
	{
		/// <summary>
		///   The default/invalid value.
		/// </summary>
		Default,

		/// <summary>
		///   This exit is not a destination and the player cannot emerge from it.
		/// </summary>
		NotADestination,

		/// <summary>
		///   The player will emerge upward out of the exit.
		/// </summary>
		PipeUp,

		/// <summary>
		///   The player will emerge downward out of the exit.
		/// </summary>
		PipeDown,

		/// <summary>
		///   The player will emerge rightward out of the exit.
		/// </summary>
		PipeRight,

		/// <summary>
		///   The player will emerge leftward out of the exit.
		/// </summary>
		PipeLeft,

		/// <summary>
		///   The player will simply appear.
		/// </summary>
		None
	}

	/// <summary>
	///   Enumerates the ways a player can enter a section exit.
	/// </summary>
	public enum ExitSourceBehavior
	{
		/// <summary>
		///   The default/invalid value.
		/// </summary>
		Default,

		/// <summary>
		///   This exit is not a source and cannot be entered.
		/// </summary>
		NotASource,

		/// <summary>
		///   The player will enter the exit when the user presses Down.
		/// </summary>
		PipeDown,

		/// <summary>
		///   The player will enter the exit when the user presses Up.
		/// </summary>
		PipeUp,

		/// <summary>
		///   The player will enter the exit when the user presses Left.
		/// </summary>
		PipeLeft,

		/// <summary>
		///   The player will enter the exit when the user presses Right.
		/// </summary>
		PipeRight,

		/// <summary>
		///   The player will enter the exit when the user presses Up. but will
		///   not move up into the exit.
		/// </summary>
		Door,

		/// <summary>
		///   The player will enter the exit when they contact the exit.
		/// </summary>
		Immediate
	}

	/// <summary>
	///   Enumerates the states of a player entering a section exit.
	/// </summary>
	public enum ExitTransitionState
	{
		/// <summary>
		///   The default value.
		/// </summary>
		Default,

		/// <summary>
		///   There is no transition currently occuring.
		/// </summary>
		NoTransitionOccurring,

		/// <summary>
		///   An exit effect is in progress (i.e. a player is visually entering a pipe).
		/// </summary>
		ExitEffectInProgress,

		/// <summary>
		///   The game is waiting on all other players to enter the exit or for
		///   the multiplayer wait timer to expire.
		/// </summary>
		MultiplayerWait,

		/// <summary>
		///   An iris-in effect is playing, rendering the screen blank.
		/// </summary>
		IrisIn,

		/// <summary>
		///   A short delay between the change in sections is occurring.
		/// </summary>
		TransitionDelay,

		/// <summary>
		///   An iris-out effect is playing, now showing a new section.
		/// </summary>
		IrisOut
	}

	/// <summary>
	///   Enumerates how players interact with screen exits and entrances.
	/// </summary>
	[Obsolete]
	public enum ScreenExitBehavior
	{
		/// <summary>
		///   The default behavior. This is considered an invalid state.
		/// </summary>
		Default,

		/// <summary>
		///   The player enters the exit by pressing Down while within the exit.
		/// </summary>
		PipeDown,

		/// <summary>
		///   The player enters the exit by jumping to it and pressing Up while
		///   within the exit.
		/// </summary>
		PipeUp,

		/// <summary>
		///   The player enters the exit by walking and pressing Left while
		///   within the exit.
		/// </summary>
		PipeLeft,

		/// <summary>
		///   The player enters the exit by walking and pressing Right while
		///   within the exit.
		/// </summary>
		PipeRight,

		/// <summary>
		///   The players enters the exit by pressing Up while within the exit.
		/// </summary>
		DoorEnter
	}

	/// <summary>
	///   Enumerates the different types of section exits.
	/// </summary>
	public enum SectionExitType
	{
		/// <summary>
		///   The default/invalid value.
		/// </summary>
		Default,

		/// <summary>
		///   Indicates a section exit that the player can enter.
		/// </summary>
		Source,

		/// <summary>
		///   Indicates a section exit that the player will emerge from.
		/// </summary>
		Destination,

		/// <summary>
		///   Indicates a section exit that acts as both a source and a destination.
		/// </summary>
		TwoWay
	}
}
