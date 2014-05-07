using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Sprites;

namespace SmlSample.Components
{
    public sealed class BasicWalkerComponent : SpriteComponent
    {
        public float Speed { get; private set; }
        private Direction direction;
        
        public BasicWalkerComponent(float speed, Direction initialDirection)
        {
            this.Speed = speed;
            this.direction = initialDirection;
        }

        public override void Initialize(Sprite owner)
        {
            base.Initialize(owner);

            // Initialize the velocity of our owner
            this.Owner.Velocity = (this.direction == Direction.Left) ? new Vector2(-this.Speed, this.Owner.Velocity.Y) : new Vector2(this.Speed, this.Owner.Velocity.Y);
        }

        public override void Update()
        {
            // Check if the X-velocity of our owner is zero
            // If so, we've hit a wall
            if (this.Owner.Velocity.X == 0f)
            {
                // Flip our direction and velocity to go the other way.
                if (this.direction == Direction.Left)
                {
                    this.direction = Direction.Right;
                    this.Owner.Velocity = new Vector2(this.Speed, this.Owner.Velocity.Y);
                }
                else if (this.direction == Direction.Right)
                {
                    this.direction = Direction.Left;
                    this.Owner.Velocity = new Vector2(-this.Speed, this.Owner.Velocity.Y);
                }
            }
        }
    }
}
