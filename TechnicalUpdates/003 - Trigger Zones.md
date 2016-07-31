# Super Mario Limitless

## Technical Update #003 - Trigger Zones

### Description

Many video games have the idea of *trigger zones* - areas of the game in which a player entering performs some actions or calls some callback. This document outlines how SML is going to implement them

### Behavior

A trigger zone is a rectangular portion of a `Section`. Trigger zones are named objects with a position and size in the world. They possess two `Action<Sprite>` callbacks - one for when a player enters the zone, and another for when a player leaves the zone.

Each frame, any trigger zone within `CameraSystem.ActiveBounds` checks for players within it. Any new players that are intersecting the zone in any way are added to this list and the player-entered callback is called. Any players no longer intersecting the zone are removed from this list and the player-left callback is called.

Trigger zones must have a non-zero area that is at least partially within the `Section.Bounds`.

### Implementation

As of 2016-07-31, there are two primary kinds of game objects in `Section` instances: tiles and sprites. Trigger zones don't fit neatly into either of those kinds, so the creation of a new kind of object may be required.

* Class `TriggerZone`
  * Field `List<Sprite> playersWithin`
  * Value Property^1 `Vector2 Position`
  * Value Property `Vector2 Size`
  * Calculated Property^2 `BoundingRectangle Bounds`
  * Value Property `Action<Sprite> PlayerEnteredAction`
  * Value Property `Action<Sprite> PlayerLeftAction`
  * Constructor `TriggerZone()`
  * Method `object GetSerializableObjects()`
  * Method `void DeserializeCustomObjects(JObject)` or `void DeserializeCustomObjects(JsonHelper)`
  
^1 Value properties are autoproperties or properties that merely get or set a private field without much calculation.

^2 Calculated properties are getter-only properties that return a value calculated from other fields, properties, or methods.