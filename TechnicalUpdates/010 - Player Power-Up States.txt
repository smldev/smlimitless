# Super Mario Limitless

## Technical Update #010 - Player Powerup States

This document will cover:
* What powerups and player powerup states are.
* Which powerups will be implemented in Alpha v0.01.
* How the inheritance of a base Player class can add new behaviors while keeping existing behavior.

## Description

### What are powerups and player powerup states?

Powerups are sprites that can power up a player when a player comes in contact with them. Powerups typically spawn when a player hits an item block.

Players have a set of behaviors (such as walking, running, and jumping) and a set of properties (such as appearance and size). Players also have a powerup state, such as small, super, or fire, that can modify behaviors or properties. Players transition between powerup states as they claim powerups or receive damage.

### Which powerups will be implemented in Alpha v0.01?

The following powerups will be implemented:

* **Small**: The default state of a player. This player has no special behaviors or appearance, and is small in size.
* **Super**: The state caused by a player contacting a Super Mushroom. The player's height is increased. If the player is damaged (but not killed through falling in a pit or in lava), their powerup state reverts to Small.
* **Fire**: The state caused by a player contacting a Fire Flower. The player's appearance will change to a different set of graphics. The player has the height of a Super player. The player can launch fireballs by pressing Run or Alt Run. Two of these fireballs can be onscreen at any time, and can kill sprites and damage bosses. If the player is damaged but not killed, their powerup state reverts to Super.

Even though there are only three powerup states planned for Alpha v0.01, the design of this feature will support all future powerup states.

## Powerup Transitions

When a player contacts a powerup sprite, a short animated graphic will play that depicts the change in state. The rest of the section is inactive during the animation. Codewise, the transition process occurs as follows:

1. A new instance of the powered-up player is created.
2. Key properties from the old player, such as position and other physics properties, are copied to the new player.
3. The old player is removed from the section.
4. The section is paused.
5. An animated graphics effect is played in the position of the old player.
6. The new player is added to the section.
7. The section is unpaused.

### Design

[Currently](https://github.com/smldev/smlimitless/commit/cefd84ff553d7c7ef514409462a1415badb8ec83), the `PlayerMario` class implements the following `protected virtual` members that describe its behavior:

* `Vector2 TargetVelocity { get; protected set; }`
* `bool IsJumping { get; set; }`
* `bool IsSpinJumping { get; set; }`
* `bool IsPlayerMoving { get; set; }`
* `bool IsRunning { get; set; }`
* `Tile WallBesidePlayer { get; set; }`
* `bool IsSlidingDownWall` { get; set; }`
* `Direction FacingDirection { get; set; }`
* `void BaseUpdate()`
* `void ApplyAccelerationToVelocity(float)`
* `void CheckForWalkRunInput()`
* `void DetermineHorizontalAcceleration()`
* `void SprintIfAllowed()`
* `void CancelHorizontalAcceleration()`
* `void ApplyTileSurfaceFriction()`
* `void CheckForJumpInput()`
* `void CheckForSpinJumpInput()`
* `float GetJumpImpulse()`
* `float GetSpinJumpImpulse()`
* `void CheckForGroundPoundInput()`
* `void CheckForSlideInput()`
* `void CheckForInAirSpinInput()`
* `void PlaySound(CachedSound, EventHandler)`
* `void PlaySound(CachedSound)`
* `void DeterminePlayerGraphicsObject()`
* `void SetPlayerGraphicsObject(string)`
* `void InitializeSounds()`

In addition to these 8 properties and 19 methods, every `Player*` class inherits the members of the `Sprite` class.

In order to amend the behavior of a powered-up player, a new class that derives from a `Player*` class will be created. The only methods that would be changed are those that perform or modify the behaviors in question.

As an example, let's look at what the differences are between a Super player and a Fire Player. A Fire Player can shoot fireballs in the direction they're facing. There can be up to two fireballs onscreen for each player at one time. Fireballs bounce off the ground but are destroyed when they hit walls, enemies, or the bottom of the world. Some enemies are killed by one fireball, others are killed by multiple fireballs, and others still are immune. Code-side changes are as follows:

* The Fire Player has a different appearance. Ideally, the Fire Player's graphics contains graphics objects for all the behaviors it inherits from the Super Player, plus any additional graphics objects needed for the fireball throwing.

The following methods must be overrode:
* `DeterminePlayerGraphicsObject()`: Check if the player is doing any new behavior, and set it graphics object accordingly if they are. If not, call the base method.
* `SetPlayerGraphicsObject(string)`: Because the `Player*.graphics` field is private, each derived class will have to own its own `graphics` field, and must have its own code to change it.
* Code that checks if a player has begun running must also call a method that creates the fireball, if allowed, and adds it to the section. 

Fire players are clearly not very complicated, but other eventual power-up states, such as the Caped Player, will be more complex, with changes to the player's controls and additions to player physics.

#### Transition Effect

When a player powers up, an animated graphic depicting the transition is played. This graphic will be coded as an instance of the `SMLimitless.Sprites.Particle` class, which has an `IGraphicsObject` property. A property `Action OnAnimationCompleted` will be added to the `AnimatedGraphicsObject` class. This property provides a callback that will be called when an animations completes, **but only for run-once graphics**. The callback will be invoked in the body of the `Update()` method:

	if (IsRunOnce)
	{
		IsRunning = false;
		OnAnimationCompleted();
		return;
	}