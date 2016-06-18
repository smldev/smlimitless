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
		public bool TurnOnEdges { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WalkerComponent"/> class.
		/// </summary>
		/// <param name="owner">The sprite that owns this component.</param>
		/// <param name="startingDirection">The initial direction (Left, Right, or FacePlayer) that the sprites starts out facing.</param>
		/// <param name="initialHorizontalVelocity">The initial velocity that the sprite has. Provide a positive value; the sign is automatically determined based on initial direction.</param>
		/// <param name="turnOnEdges">A flag indicating whether this sprite turns when it crosses an edge.</param>
		public WalkerComponent(Sprite owner, SpriteDirection startingDirection, float initialHorizontalVelocity, bool turnOnEdges = false)
		{
			Owner = owner;

			StartingDirection = startingDirection;
			InitialHorizontalVelocity = initialHorizontalVelocity;

			direction = (StartingDirection == SpriteDirection.FacePlayer || StartingDirection == SpriteDirection.Left) ? Direction.Left : Direction.Right;
			CurrentVelocity = initialHorizontalVelocity;
			TurnOnEdges = turnOnEdges;
		}

		/// <summary>
		/// Update this component.
		/// </summary>
		public override void Update()
		{
			if (TurnOnEdges)
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
					float checkPointX = (direction == Direction.Left) ? (tileLeft - 1f) : (tileRight + 1f);
					Vector2 checkPoint = new Vector2(checkPointX, tileBeneathOwner.Hitbox.Bounds.Center.Y);
					Tile tileBesideTile = ownerSection.GetTileAtPosition(checkPoint);
					if (tileBesideTile == null || (tileBesideTile.RectSolidSides & TileRectSolidSides.Top) == 0)
					{
						int spriteCenter = (int)Owner.Hitbox.Center.X;
						if ((direction == Direction.Left && tileLeft == spriteCenter) || 
						(direction == Direction.Right && tileRight == spriteCenter))
						{
							if (direction == Direction.Left) { direction = Direction.Right; CurrentVelocity = -CurrentVelocity; }
							else if (direction == Direction.Right) { direction = Direction.Left; CurrentVelocity = -CurrentVelocity; }
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

		public override void HandleSpriteCollision(Sprite collidingSprite, Vector2 resolutionDistance)
		{
			if (Owner.SpriteCollisionMode == SpriteCollisionMode.NoCollision || collidingSprite.SpriteCollisionMode == SpriteCollisionMode.NoCollision) { return; }
			if (collidingSprite.Hitbox.Right > Owner.Hitbox.Left && collidingSprite.Hitbox.Left <= Owner.Hitbox.Left)
			{
				direction = Direction.Right;
				CurrentVelocity = (CurrentVelocity > 0f) ? CurrentVelocity : -CurrentVelocity;
			}
			else if (collidingSprite.Hitbox.Left < Owner.Hitbox.Right && collidingSprite.Hitbox.Right >= Owner.Hitbox.Right)
			{
				direction = Direction.Left;
				CurrentVelocity = (CurrentVelocity < 0f) ? CurrentVelocity : -CurrentVelocity;
			}
		}
	}
}
