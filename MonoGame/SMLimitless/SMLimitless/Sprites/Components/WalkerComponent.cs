using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	/// A component which moves the sprite horizontally along a surface of tiles.
	/// </summary>
	public sealed class WalkerComponent : SpriteComponent
	{
		private bool active;
		private Direction direction;

		private float currentVelocity;

		/// <summary>
		/// Gets or sets the current velocity of this component.
		/// </summary>
		public float CurrentVelocity
		{
			get
			{
				return (Direction == Direction.Right) ? currentVelocity : -currentVelocity;
			}
			set
			{
				UpdateOwnerVelocity(value);
			}
		}

		private void UpdateOwnerVelocity(float value)
		{
			if (Direction == Direction.Left)
			{
				currentVelocity = (value > 0f) ? -value : value;
			}
			else if (Direction == Direction.Right)
			{
				currentVelocity = (value < 0f) ? -value : value;
			}

			Owner.Velocity = new Vector2(currentVelocity, Owner.Velocity.Y);
		}

		/// <summary>
		/// Gets or sets the direction that the component has its owner walk in.
		/// </summary>
		public Direction Direction
		{
			get { return direction; }
			set
			{
				direction = value;
				UpdateOwnerVelocity(CurrentVelocity);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this component is active.
		/// </summary>
		public override bool IsActive
		{
			get { return active; }
			set
			{
				active = value;
				if (value)
				{
					Owner.Velocity = new Vector2(currentVelocity, Owner.Velocity.Y);
				}
				else
				{
					Owner.Velocity = new Vector2(0f, Owner.Velocity.Y);
				}
			}
		}

		/// <summary>
		/// The initial direction (Left, Right, or FacePlayer) that the sprites starts out facing.
		/// </summary>
		public SpriteDirection StartingDirection { get; }

		/// <summary>
		/// The initial velocity that the sprite has. 
		/// </summary>
		public float InitialHorizontalVelocity { get; }

		/// <summary>
		/// Gets a flag indicating whether this sprite turns when it crosses an edge.
		/// </summary>
		public bool TurnOnCliffs { get; set; }

		/// <summary>
		/// Gets a flag indicating whether this sprite turns when it collides with another sprite.
		/// </summary>
		public bool TurnOnSpriteCollisions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WalkerComponent"/> class.
		/// </summary>
		/// <param name="owner">The sprite that owns this component.</param>
		/// <param name="startingDirection">The initial direction (Left, Right, or FacePlayer) that the sprites starts out facing.</param>
		/// <param name="initialHorizontalVelocity">The initial velocity that the sprite has. Provide a positive value; the sign is automatically determined based on initial direction.</param>
		/// <param name="turnOnEdges">A flag indicating whether this sprite turns when it crosses an edge.</param>
		public WalkerComponent(Sprite owner, SpriteDirection startingDirection, float initialHorizontalVelocity, bool turnOnEdges = false, bool turnOnSpriteCollisions = true)
		{
			Owner = owner;

			StartingDirection = startingDirection;
			InitialHorizontalVelocity = initialHorizontalVelocity;

			Direction = (StartingDirection == SpriteDirection.FacePlayer || StartingDirection == SpriteDirection.Left) ? Direction.Left : Direction.Right;
			CurrentVelocity = initialHorizontalVelocity;
			TurnOnCliffs = turnOnEdges;
			TurnOnSpriteCollisions = turnOnSpriteCollisions;
		}

		/// <summary>
		/// Update this component.
		/// </summary>
		public override void Update()
		{
			if (TurnOnCliffs)
			{
				Section ownerSection = Owner.Owner;

				Tile tileBeneathOwner = Owner.TileBeneathSprite;
				if (tileBeneathOwner != null && (tileBeneathOwner.RectSolidSides & TileRectSolidSides.Top) != 0)
				{
					// There's a tile beneath this sprite. Check if we're near the edge.
					int tileLeft = (int)tileBeneathOwner.Hitbox.Bounds.Left;
					int tileRight = (int)tileBeneathOwner.Hitbox.Bounds.Right;

					// Check to see if there's any other top-solid tile next to this tile
					// in the direction we're travelling. If not, turn.
					float checkPointX = (Direction == Direction.Left) ? (tileLeft - 1f) : (tileRight + 1f);
					Vector2 checkPoint = new Vector2(checkPointX, tileBeneathOwner.Hitbox.Bounds.Center.Y);
					Tile tileBesideTile = ownerSection.GetTileAtPosition(checkPoint);
					if (tileBesideTile == null || (tileBesideTile.RectSolidSides & TileRectSolidSides.Top) == 0)
					{
						int spriteCenter = (int)Owner.Hitbox.Center.X;
						if ((Direction == Direction.Left && tileLeft == spriteCenter) || 
						(Direction == Direction.Right && tileRight == spriteCenter))
						{
							if (Direction == Direction.Left) { Direction = Direction.Right; CurrentVelocity = -CurrentVelocity; }
							else if (Direction == Direction.Right) { Direction = Direction.Left; CurrentVelocity = -CurrentVelocity; }
						}
					}
				}
			}
		}

		/// <summary>
		/// Handle a collision between this component's owner sprite and a tile.
		/// </summary>
		/// <param name="collidingTile">The tile that the owner sprite has collided with.</param>
		/// <param name="resolutionDistance">The distance by which the owner sprite was moved to resolve the collision.</param>
		public override void HandleTileCollision(Tile collidingTile, Vector2 resolutionDistance)
		{
			if (resolutionDistance.GetResolutionDirection() == Direction.Right && Direction == Direction.Left)
			{
				Direction = Direction.Right;
				CurrentVelocity = -CurrentVelocity;
			}
			else if (resolutionDistance.GetResolutionDirection() == Direction.Left && Direction == Direction.Right)
			{
				Direction = Direction.Left;
				CurrentVelocity = -CurrentVelocity;
			}
		}

		public override void HandleSpriteCollision(Sprite collidingSprite, Vector2 resolutionDistance)
		{
			if (!TurnOnSpriteCollisions) { return; }

			if (Owner.SpriteCollisionMode == SpriteCollisionMode.NoCollision || collidingSprite.SpriteCollisionMode == SpriteCollisionMode.NoCollision) { return; }
			if (collidingSprite.Hitbox.Right > Owner.Hitbox.Left && collidingSprite.Hitbox.Left <= Owner.Hitbox.Left)
			{
				Direction = Direction.Right;
				if (IsActive) { CurrentVelocity = (CurrentVelocity > 0f) ? CurrentVelocity : -CurrentVelocity; }
			}
			else if (collidingSprite.Hitbox.Left < Owner.Hitbox.Right && collidingSprite.Hitbox.Right >= Owner.Hitbox.Right)
			{
				Direction = Direction.Left;
				if (IsActive) { CurrentVelocity = (CurrentVelocity < 0f) ? CurrentVelocity : -CurrentVelocity; }
			}
		}
	}
}
