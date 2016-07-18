Super Mario Limitless
Technical Update #001 - Sprite Activity States and their Transitions

Sprites have a Position and InitialPosition. The former property changes throughout gameplay, but the latter shouldn't change. As a sprite moves in and out of the rectangle represented by the Section.CameraSystem.ActiveBounds property, it transitions between three states: `Active`, `WaitingToRespawn`, and `Inactive`. Let's take a look at all three.

`SpriteActiveState`:
* `Active`: Both the `Sprite.Position` and the `Sprite.InitialPosition` properties are within the active bounds. `Active` sprites are eligible for updating and drawing.
* `WaitingToLeaveBounds`: The `Sprite.InitialPosition` property is within the active bounds, but the `Sprite.Position` property is not. For instance, consider a sprite that walks out of the active bounds without the camera moving. This sprite can't respawn until the camera leaves and re-enters its `InitialPosition`, so it must wait to reactivate until the camera has moved as such.
* `Inactive`: Both the `Sprite.InitialPosition` property and the `Sprite.Position` property are outside of the active bounds. If the active bounds moves back over the `InitialPosition`, the sprite will reactivate.

State Transitions:
* `Sprite.Position` moves out of `ActiveBounds` but `Sprite.InitialPosition` does not
  * Sprite's active state becomes `SpriteActiveState.WaitingToLeaveBounds`.
  * Method `Sprite.Deactivate()` is called. Sprites may choose to remove themselves from the section (i.e. if they have been killed but haven't fallen out of the stage yet).
  * Sprite is no longer updated or drawn.
  * If still around, the sprite is teleported back to its `InitialPosition`.
* `Sprite.InitialPosition` moves out of `ActiveBounds` but `Sprite.Position` does not
  * Nothing happens.
* `Sprite.InitialPosition` moves out of `ActiveBounds` on a sprite with state `WaitingToLeaveBounds`
  * Sprite's active state becomes `SpriteActiveState.Inactive`.
* `Sprite.InitialPosition` moves back inbounds on a sprite with state `InActive`
  * Sprite's active state becomes `SpriteActiveState.Active`.
  * Method `Sprite.Activate()` is called.
  * Sprite is now updated and drawn again.