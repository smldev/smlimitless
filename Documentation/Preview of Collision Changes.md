## Preview of Collision Changes Post-Alpha v0.02

One thing missing from the current collision resolution method is knowledge about the direction of travel. Each time a rectangle collides with another rectangle or right triangle, its resolution distance (the distance by which it has to be moved to no longer be in the other rectangle) is calculated only using the positions of the two rectangles. This is sufficient in most cases, but does lead to edge cases where the proper resolution distance cannot be determined using positions alone.

Consider that two stationary rectangles that are not already colliding will not collide unless one or both of them begins moving. Thus, we can determine that collision happens only in one way: through motion*; through one or both rectangles moving into each other. Ordinarily, a moving rectangle (for instance, a sprite) will move into a stationary rectangle (for instance, a tile). Tiles can move as part of layers; those collisions will be covered below.

If, when calculating collision resolution between a moving and stationary rectangle, we provide both the rectangles involved *and* the velocity of the moving rectangle, we can determine the proper resolution distance between the rectangles, regardless of the moving rectangle's speed or size. This also helps solve a small subset of the "bullet-through-paper" problems where a fast-moving object skips far enough into another rectangle's hitbox that it's pushed out the other side (or just skips the hitbox entirely).

Essentially, when a rectangle moves into another, the direction of motion (left/right for the X axis, up/down for the Y axis) can be reversed to find the resolution direction - the direction to push the rectangle out is always the opposite of the direction the rectangle entered in.

Consider a rectangle A that's moving to the right (velocity of `X = 2, Y = 0`). If it moves into a rectangle B with width `10`, and with a left edge at `X = 10`, and the right edge of A ends up at `X = 11`, since A's X velocity is to the right, the direction to push A out is always left. This works even if A moves substantially into the B - for instance, if the right edge of A is at `X = 19`, since the motion is to the right, the direction is still toward the left.

The below table considers two rectangles: a moving rectangle A and a stationary rectangle named B. It shows the direction to push A out given a collision with B.

| If A is moving... | And A hits this edge of B... | Then this edge of A... | Should be pushed to this edge of B |
|-------------------|-|------------------------|------------------------------------|
| Left              | Right edge | Left edge              | Right edge                         |
| Right             | Left edge | Right edge             | Left edge                          |
| Up                | Bottom edge | Top edge               | Bottom edge                        |
| Down              | Top edge | Bottom edge            | Top edge                           |

Sometimes, both rectangles can be moving (for instance, a sprite colliding with a moving layer of tiles). The collision engine will perform the movement of all tiles before the movement of any sprites, so no collision will occur with two objects moving into each other at the same point of processing. However, sprites that are motionless can suddenly be colliding with a tile despite not having moved. Sprites are always the objects that should be offset on collisions, and this can be achieved by taking the tile's velocity and inverting both components. This works because if a tile moves to the right, the sprite can be considered moving to the left from the tile's perspective. The inverted velocity can be used in place of the sprite's velocity and the above collision resolution can be done.

After all tiles move and all resulting collisions resolved, sprites are then moved and any new collisions can be handled using the above method.


* There is one exception: if the rectangles were created such that they were colliding in the first place. This is resolved by considering `no X motion` to mean `moving leftward` and `no Y motion` to mean `moving downward`. Then the collision can be handled with the above method.