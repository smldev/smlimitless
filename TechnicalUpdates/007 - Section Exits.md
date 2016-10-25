# Technical Update #007 - Section Exits

## Description

A **section exit** is an invisible game object that exists in sections. Section exits allow the player to move within a section and between sections.

A section exit has a position in the section. This position is not required to be aligned to any layer grid. The section exit has a size equal to `GameServices.GameObjectSize` or a multiple thereof.

Section exits have a specified source behavior; this is the way a player enters the section exit and how the player is animated as they enter the section exit. During such animations, the player will not interact with the world, including with other sprites, tiles, or gravity.

	* **Pipe Down**: The user must press Down while standing on solid ground while within the area of the section exit. The player will be moved left or right such that their hitbox falls (horizontally) within the section exit, then they will sink into the tile they stand on. A pipe warp sound will play.
	* **Pipe Up**: The user must press Up while contacting a solid tile from below, and they must be in the air (or suspended in water). The player will move to be within the section exit's sides, and will then move up into the tile.
	* **Pipe Left**: The user must press Left while contacting a solid tile from the right side and they must be standing on solid ground (or suspended in water). The player will move leftward into the tile.
	* **Pipe Right**: The user must press Right while contacting a solid tile from the left and they must be standing on solid ground (or suspended in water). The player will move rightward into the tile.
	* **Door**: The player must press Up while standing on a solid tile. The player will not be animated.
	* **Immediate**: The player will automatically enter the section exit when they contact the exit in any manner.

Section exits are paired; one exit will lead to another. Section exits can be one-way (a source and a destination) or two-way (two sources which are also two destinations). Section exits cannot overlap; any given point in a section may have only one exit. Multiple sources can lead to the same destination, but a single source may only go to one destination.

Section exits acting as destinations have destination behaviors which occur when the player is teleported to the destination. These behave much as source behaviors and the player also doesn't interact with the world.

	* **Pipe Up**: The player emerges upward from the destination.
	* **Pipe Down**: The player emerges downward from the destination.
	* **Pipe Left**: The player emerges from the left to the right from the destination.
	* **Pipe Right**: The player emerges from the right to the left from the destination
	* **None**: The player merely appears and immediately begins interacting with the world.
	
### Intra-Section Exits

An intra-section exit is a pair of section exits within the same section. The player can enter one and be teleported to the other. After the source behavior completes, the player's position is simply set to the destination section exit.

If the source is less than twice the screen width or height from the section exit, the camera system will scroll to the player's new position before they emerge from the destination. This will be done through a new camera system method that scrolls to a given position. After the scroll is complete, the player will emerge. There is a minimum one-second delay after the source behavior completes and the destination behavior begins.

If the source is more than twice the screen width or height from the section exit, an iris-in effect will play toward a point `SectionExit.IrisPoint` on the source (i.e. bottom-center from Pipe Down, center-left from Pipe Left, etc.). The camera system will then immediately move to the player's new position, a one-second delay occurs, and then an iris-out effect will play from the iris point on the destination.

Section exits with a source behavior of Immediate cannot be on the same screen as their destination.

### Inter-Section Exits

An inter-section exit is a pair of section exits between two sections. The player enters one exit, and the instance of the player is removed from the first section's players and added to the second section's players.

Inter-section exits always trigger an iris-out and an iris-in. There is no mandatory delay here; the time it takes to unload a section and load another should be sufficient for the delay.

### Where is the player teleported?

Entering a source section exit has been covered above. Exiting a destination section exit is a bit more complicated.

Section exits may have any size that is a multiple of `GameServices.GameObjectSize`. This is to allow large exits with Immediate source behavior instead of having the level designer place a row or column of small, square exits all over the place. As such, though, destination section exits may also be any such size.

For section exits with Pipe Up destination behavior, the player will be placed such that their bottom center point is at the bottom-center point of the section exit. (The movement of the player upward is only visual).

The remaining destination behaviors, thus, can be defined similarly:
	* Pipe Down: The player's top center point is at the top-center point of the section exit.
	* Pipe Left: The player's bottom left point is at the bottom-left point of the section exit.
	* Pipe Right: The player's bottom right point is at the bottom-right point of the section exit.
	* None: Same as Pipe Up.
	
### Multiplayer Behavior

For a section with more than one player, if a player enters any source section exit with the destination not also onscreen, the camera system will stop scrolling and zooming entirely and a five-second timer begins. Other players may enter the source, but if they do not within five seconds, the section will freeze and the iris transition will begin.

The players will then emerge from the destination in the order they entered the source. If they did not enter the source, their order will be determined by linear distance from the iris point of the source, smallest to largest.

For an exit pair that are has both exits on the same screen, players entering the source will wait one second, then exit the destination.

## Implementation
Section exits have internal IDs of type `int`. Section exits refer to other section exits by their ID, creating pairs. IDs must be unique across a level. They are assigned by the level editor when they are placed in the level and written to the level file.

	* Type `SectionExit`
		* `const float DefaultTimeout = 1.0f`
		* `Section Owner {get; internal set;}`
		* `int ID {get; internal set;}`
		* `SectionExitType ExitType {get; internal set;}`
		* `Vector2 Position {get; internal set;}`
		* `Vector2 Size {get; internal set;}`
		* `BoundingRectangle Hitbox {get; internal set;}
		* `float Timeout {get; internal set;}`
		* `ExitSourceBehavior SourceBehavior {get; internal set;}`
		* `ExitDestinationBehavior DestinationBehavior {get; internal set;}`
		* `Vector2 IrisPoint {get;}`
		* `Queue<Sprite> PlayersInExit {get;}`
		* `bool CanPlayerEnter(Sprite player)`: Checks `InputManager.IsCurrentActionPress` and position of player.
		* `void PlayerEntered(Sprite player)`
		* `void DrawPlayerEntering()`
	* Modifications to type `Section`
		* `const int DefaultSectionExitFramesUntilTransition = 300`
		* `int SectionExitFramesUntilTransition`
		* `SectionExit ExitLock`: If a player has entered an exit and other players are still in the section, this is the exit the player entered. Stops players from going into multiple exits at the same time.
		* `bool IsActive {get; set;}`
		* `void IrisIn(Vector2 point, float duration)`
		* `void IrisOut(Vector2 point, float duration)`
		* `void OnSectionExit(SectionExit exit, Sprite player)`
		* Modify `AddSprite` and `RemoveSprite` to add/remove players from both `Sprites` and `Players`.
	* Modifications to type `CameraSystem`
		* `void ScrollToPoint(Vector2 point)`: Clears `Owner.IsActive` before scroll and sets it after scroll.
		* `void ScrollToPoint(Vector2 point, float velocity)`
		* `bool IsFrozen {get; set;}`
	* Modifications to type `Level`
		* `internal IDGenerator SectionExitIDGenerator {get; private set;}`
		* `SectionExit GetSectionExitByID(int id)`
		* `void OnSectionExit(Section source, Section destination, IEnumerable<Sprite> players)`
		* `private void ChangeActiveSection(Section newSection)`
	* Type `SMLimitless.Components.IDGenerator`
		* Assigns unique, contiguous `int` IDs for each given instance.
		* `private int lastIDAssigned = 0`
		* `int GetNewID()`
		* Throws exception if the ID range is exhausted.
		* No mechanism (yet) to free IDs
	* Enum `SectionExitType`
		* `Source`
		* `Destination`
		* `TwoWay`
	* Enum `ExitSourceBehavior`
		* `PipeDown`
		* `PipeUp`
		* `PipeLeft`
		* `PipeRight`
		* `Door`
		* `Immediate`
	* Enum `ExitDestinationBehavior`
		* `PipeUp`
		* `PipeDown`
		* `PipeRight`
		* `PipeLeft`
		* `None`