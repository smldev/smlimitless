using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites.Components
{
	public sealed class WalkerComponent : SpriteComponent
	{
		private Direction direction;

		private float currentVelocity;
		private float CurrentVelocity
		{
			get
			{
				return (direction == Direction.Right) ? currentVelocity : -currentVelocity;
			}
			set
			{
				if (direction == Direction.Left)
				{
					currentVelocity = (value > 0f) ? -value : value;
				}
				else if (direction == Direction.Right)
				{
					currentVelocity = (value < 0f) ? -value : value;
				}

				Owner.Velocity = new Vector2(currentVelocity, Owner.Velocity.Y);
			}
		}

		public SpriteDirection StartingDirection { get; }
		public float InitialHorizontalVelocity { get; }

		public WalkerComponent(Sprite owner, SpriteDirection startingDirection, float initialHorizontalVelocity)
		{
			Owner = owner;

			StartingDirection = startingDirection;
			InitialHorizontalVelocity = initialHorizontalVelocity;

			direction = (StartingDirection == SpriteDirection.FacePlayer || StartingDirection == SpriteDirection.Left) ? Direction.Left : Direction.Right;
			CurrentVelocity = initialHorizontalVelocity;
		}

		public override void Update()
		{
			
		}

		public override void HandleTileCollision(Tile collidingTile, Vector2 resolutionDistance)
		{
			if (resolutionDistance.GetResolutionDirection() == Direction.Right && direction == Direction.Left)
			{
				direction = Direction.Right;
				CurrentVelocity = -CurrentVelocity;
			}
			else if (resolutionDistance.GetResolutionDirection() == Direction.Left && direction == Direction.Right)
			{
				direction = Direction.Left;
				CurrentVelocity = -CurrentVelocity;
			}
		}
	}
}
