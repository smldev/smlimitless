# Super Mario Limitless

## Technical Update #002 - Lakitu's Implementation

### Behavior

Lakitu is an enemy sprite that hovers over the player and throws things, typically Spiny Eggs. Although his behavior varied across the games, the first Lakitu that will be implemented is the SMB version.

This Lakitu hovers at a fixed height on the screen (not necessarily in the section as the camera might move vertically). Lakitu moves horizontally along his vertical position, chasing one of two desired positions, one on the left-hand side of the screen, and the other on the righthand side. When Lakitu reaches one desired position, his desired position switches to the other side.

Lakitu throws things which have a certain upward velocity and a sideways velocity slightly more than Lakitu himself in whichever direction Lakitu is travelling. The things are Sprite instances that are cloned when Lakitu drops them. There is a two-second delay between when Lakitu throws things. Lakitu keeps track of all the things he has thrown that are onscreen - when these sprites move offscreen, they are permanently removed from his "thrown" list. Lakitu can only have so many onscreen sprites.

Lakitu's spawn is controlled by the position of two vertical lines that form a vertical band of space in the section. When any player is within the band, Lakitu spawns from in front of the player, and when all players leave the band, Lakitu leaves the screen in the direction he's facing and despawns.

Lakitu can be killed by stomping on him. As soon as Lakitu drops offscreen, he is deactivated and his health is reset. A timeout begins before he can respawn.

### Implementation

Lakitu is made out of three Sprite instances - Lakitu himself (`SmlSprites.SMB.Enemies.Lakitu`) and two "listeners" (`SmlSprites.Helpers.VerticalLinePlayerListener`), one for each side of Lakitu's vand.

`VerticalLinePlayerListener` is a sprite that waits for any player to cross over the line, where it raises an event `EventHandler<ListenerTriggeredEventArgs>` (spawning/despawning Lakitu, in this case). It cannot be collided with. Lakitu exists within the level but isn't always active (if inactive, calls to `Update()` and `Draw()` do nothing). Activity in this context is separate from the `ActiveState` property, instead it is a private field for the Lakitu. Writes to the `ActiveState` property will always result in the property having the `SpriteActiveState.Active` property, as Lakitu maintains his own active information.

When a player enters the band, Lakitu is activated, placed in a position in front of the player, and given an acceleration toward the player. When all players leave the band, Lakitu's acceleration is set away from the player, and when the section deactivates it, it loses velocity and acceleration. Lakitu's X acceleration is constant (though its sign isn't), and will change direction when Lakitu passes over the player.

Lakitu has a desired Y position in the level which he tries to maintain, which is based on the Y position of the camera. If the camera moves, he will accelerate towards his new desired Y position, stopping as soon as he gets there.

Lakitu holds a reference to a sprite which it clones whenever it wishes to throw a sprite. It throws a sprite as long as its list of onscreen sprites isn't too large. The sprite created has an X velocity of Lakitu's velocity plus a certain amount and a slightly-upward Y velocity.

When `Deactivate()` is called, Lakitu checks if he fell out of the active bounds (and if he's dead), he resets his dead state and his health, despawns, and sets a timer. When the player is in the band (or re-enters it), Lakitu will respawn when the timer hits 0. The timer always decreases.

### Physics Settings

* `DesiredYPositionPercentage`: A value from 0 (top of screen) to 1 (bottom of screen) for where Lakitu wants to be. Takes into account viewport size with zoom.
* `XAcceleration`: How fast Lakitu accelerates horizontally.
* `YAcceleration`: How fast Lakitu accelerates vertically.
* `ThrownAdditionalXVelocity`: How much additional X velocity thrown sprites have.
* `ThrownAdditionalYVelocity`: How much additional Y velocity thrown sprites have.
* `MaximumOnscreenSprites`: How many sprites can be onscreen at once.
* `ThrowDelay`: How many seconds between each time Lakitu throws a sprite.
* `RespawnDelay`: How many seconds between each time Lakitu can respawn after being killed

### Editor Properties

* `SpawnBandStart`: The X-position of the line that starts the vertical band of space. Must be less than `SpawnBandEnd`.
* `SpawnBandEnd`: The X-position of the line that ends the vertical band of space. Must be more than `SpawnBandStart`.
* `ThrownSprite`: The `Sprite` instance that Lakitu throws.