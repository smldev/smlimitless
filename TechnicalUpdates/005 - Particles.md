# Super Mario Limitless

## Technical Update #005 - Particles

### Description

A particle is a simple game object that draws a graphic to the screen. Particles have position, velocity, and acceleration, and can be affected by gravity, but do not collide with the world.

Unlike Sprites and Tiles, there is no inheritance hierarchy of particles - instead, particles are created with a graphics resource, position, velocity, acceleration, and lifespan.

Particles despawn when their lifespan expires or when they leave the active bounds of the camera system.

### Implementation

* Class `Particle`
  * Property `Vector2 Position`
  * Property `Vector2 Velocity`
  * Property `Vector2 Acceleration`
  * Property `float RemainingLife`: measured in seconds. Negative values mean that the particle will never expire.
  * Property `IGraphicsObject Graphics`
  * Property `bool AffectedByGravity`
  * Constructor `Particle(string graphicsResourceName, Vector2 position, Vector2 velocity, Vector2 acceleration, bool affectedByGravity, float lifespan = -1f)