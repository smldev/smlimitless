# Super Mario Limitless

## Technical Update #004 - Question Blocks

### Description

A Question Block is an iconic element in the Mario series. It is a solid tile which appears as a glowing block with a question mark in it. It typically contains a coin or a powerup.

In Limitless, a Question Block is a Tile with an animated graphics object. When the player hits it from below, or ground pounds it from above, the tile "bounces" up or down, converts into an Empty Block, and releases a Sprite instance.

Any sprite can be placed in a Question Block, but the most common sprites are likely to be coin-giving sprite effects and powerups.

Question Blocks can contain one or more instances of a sprite (`FixedQuantity`), allow the player to release as many sprites as they can in a fixed time period (`FixedTime`), allow the player to release as many sprites as they can, up to a limit, in a fixed time period (`FixedTimeWithMaximumQuantity`), or allow the player to release as many sprites as they can, up to a limit, in a fixed time period, followed up with a specified action if the player can vacate the block in time (`FixedTimeWithBonusAction`).

### Implementation

When the Question Block is hit, either from below or from a ground pound above, its graphics bounce upward or downward. The distance they move is defined by the `MaximumVisualDisplacement` physics setting, and how long the displacement lasts is defined by the `VisualDisplacementLength` physics setting.

Visual displacement has two phases - one where its heading up or down, and one where its returning to the standard position. Thus, the `VisualDisplacementLength` setting is equal to the entire length of the displacement, and each phase is half the length.

When the Question Block is hit, it releases an item, a clone of the `ContainedSprite` property. If this sprite does not have the attribute `FreeImmediatelyFromItemBlock`, it will be held in the `SpawningSprite` property.

The `SpawningSprite` will rise out of the Question Block. The time this process takes is equal to the `SpriteSpawnLength` setting. While spawning, the sprite will not be updated, but it will be drawn. It will spawn with a center X equal to the Question Block's center X and a Y equal to the Question Block's Y.

Drawing a spawning sprite is a bit different than just calling `Draw()` on the sprite. The sprite must not be drawn below the top of the Question Block (or above the bottom for ground-pounds). The process to draw a spawning sprite is as follows:

  1. Using the `ContainedSpriteGraphicsName` property, load the graphics object through the `ContentPackageManager` and load its content.
  2. Take the first `Texture2D` of the frame and copy it to a new, cropped texture of the right height (`textureHeight = block.Top - sprite.Top` or `textureHeight = sprite.Bottom - block.Bottom`).
  3. Draw this texture where the sprite is.
  4. Move the sprite upward.
  
Once the spawn has completed, the sprite is added to the Section proper (a frame before it ends, `AddSpriteOnNextFrame` is called).

If the sprite does have the attribute `FreeImmediatelyFromItemBlock`, it will simply spawn with center X equal to the block's center X and its Bottom equal to the block's Top. Examples of such sprites are coin-effect sprites that give the player a coin.

* Class `QuestionBlock : Tile`
  * Static Field `PhysicsSetting<float> MaximumVisualDisplacement`
  * Static Field `PhysicsSetting<float> VisualDisplacementLength` (expressed in seconds)
  * Static Field `PhysicsSetting<float> SpriteSpawnLength` (expressed in seconds)
  * Field `VisualDisplacementTarget`
  * Field `VisualDisplacement`
  * Property `Sprite ContainedSprite`
  * Property `Sprite SpawningSprite`
  * Property `int Quantity`
  * Property `float TimeLimit` (expressed in seconds)
  
* Enum `QuestionBlockItemReleaseType`
  * Member `FixedQuantity`
  * Member `FixedTime`
  * Member `FixedTimeWithMaximumQuantity`
  * Member `FixedTimeWithBonusAction`