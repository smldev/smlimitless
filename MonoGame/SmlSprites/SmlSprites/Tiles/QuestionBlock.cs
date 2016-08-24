using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.IO.LevelSerializers;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Tiles
{
	public sealed class QuestionBlock : Tile
	{
		private static PhysicsSetting<float> MaximumVisualDisplacement;
		private static PhysicsSetting<float> VisualDisplacementLength;
		private static PhysicsSetting<float> SpriteReleaseLength;

		private bool isEmpty = false;
		private bool isReleasingSprite = false;
		private float releaseSpriteProgress = 0f;
		private VerticalDirection releasingSpriteDirection = VerticalDirection.None;
		private AnimatedGraphicsObject questionGraphics;
		private StaticGraphicsObject emptyGraphics;
		private float visualDisplacementTarget;
		private float visualDisplacement;
		private VerticalDirection visualDisplacementDirection;
		private bool visualDisplacementReturning = false;
		private Sprite releasingSprite;
		private float spriteReleaseAmount;

		public Sprite ContainedSprite { get; set; }
		public string ContainedSpriteGraphicsName { get; private set; }
		public QuestionBlockItemReleaseType ReleaseType { get; private set; }
		public int Quantity { get; set; }
		public float TimeLimit { get; set; }

		public override string EditorCategory => "Interactive Tiles";
		public override float SurfaceFriction => 1f;

		static QuestionBlock()
		{
			MaximumVisualDisplacement = new PhysicsSetting<float>("Question Block: Max Visual Displacement (px)", 1f, 64f, 6f, PhysicsSettingType.FloatingPoint);
			VisualDisplacementLength = new PhysicsSetting<float>("Question Block: Visual Displacement Length (sec)", 0.01f, 10f, 0.75f, PhysicsSettingType.FloatingPoint);
			SpriteReleaseLength = new PhysicsSetting<float>("Question Block: Sprite Release Length (sec)", 0.01f, 10f, 1f, PhysicsSettingType.FloatingPoint);
		}

		public QuestionBlock()
		{
			Size = new Vector2(16f);
			TileShape = CollidableShape.Rectangle;
			RectSolidSides = TileRectSolidSides.Top | TileRectSolidSides.Left | TileRectSolidSides.Right | TileRectSolidSides.Bottom;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			ContainedSprite = TypeSerializer.DeserializeSprite(customObjects, "containedSprite");
			ReleaseType = (QuestionBlockItemReleaseType)customObjects.GetInt("releaseType");
			Quantity = customObjects.GetInt("quantity");
			TimeLimit = customObjects.GetFloat("timeLimit");
			isEmpty = customObjects.GetBool("isEmpty");
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				containedSprite = TypeSerializer.GetSpriteObjects(ContainedSprite),
				releaseType = (int)ReleaseType,
				quantity = Quantity,
				timeLimit = TimeLimit,
				isEmpty = this.isEmpty
			};
		}

		public override void Initialize(Section owner)
		{
			questionGraphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBQuestionBlock");
			emptyGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBEmptyBlock");
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			questionGraphics.LoadContent();
			emptyGraphics.LoadContent();
		}

		public override void Update()
		{
			if (!isEmpty) { questionGraphics.Update(); }

			UpdateVisualDisplacement();

			UpdateReleasingSprite();
		}

		private void UpdateVisualDisplacement()
		{
			if (visualDisplacementDirection == VerticalDirection.None) { return; }

			bool atOrOverTarget = false;

			if (visualDisplacementReturning) { atOrOverTarget = visualDisplacement <= 0f; }
			else { atOrOverTarget = visualDisplacement >= MaximumVisualDisplacement.Value; }

			if (atOrOverTarget)
			{
				if (!visualDisplacementReturning)
				{
					visualDisplacementReturning = true;
					visualDisplacementTarget = 0f;
				}
				else
				{
					// All done. We can start the sprite release now.
					var releaseDirection = visualDisplacementDirection;
					visualDisplacementDirection = VerticalDirection.None;
					visualDisplacement = 0f;
					visualDisplacementTarget = 0f;
					visualDisplacementReturning = false;
					BeginRelease(releaseDirection);
				}
			}
			else
			{
				// Move the visual displacement toward its target.
				float displacementDelta = (MaximumVisualDisplacement.Value * (1f / (VisualDisplacementLength.Value * 60f)));
				if (visualDisplacementReturning) { displacementDelta = -displacementDelta; }
				visualDisplacement += displacementDelta;
			}
		}

		public override void Draw()
		{
			if (!isEmpty)
			{
				float displacement = (visualDisplacementDirection == VerticalDirection.Up) ? -visualDisplacement : visualDisplacement;
				Vector2 displacedPosition = new Vector2(Position.X, Position.Y + displacement);
				questionGraphics.Draw(displacedPosition, Color.White);

				if (isReleasingSprite)
				{
					Rectangle cropping = GetSpriteYCropping();
					if (releasingSpriteDirection == VerticalDirection.Up) { releasingSprite.Draw(cropping); }
					else if (releasingSpriteDirection == VerticalDirection.Down) { releasingSprite.Draw(cropping);  }
					SMLimitless.Debug.Logger.LogInfo($"Crop {cropping}");
				}
			}
			else
			{
				emptyGraphics.Draw(Position, Color.White);
			}
		}

		private Rectangle GetSpriteYCropping()
		{
			Rectangle result = new Rectangle();
			result.X = 0;
			result.Width = (int)releasingSprite.Hitbox.Width;
			
			if (releasingSpriteDirection == VerticalDirection.Up)
			{
				result.Y = 0;
				result.Height = (int)(Hitbox.Bounds.Top - releasingSprite.Hitbox.Top);
			}
			else if (releasingSpriteDirection == VerticalDirection.Down)
			{
				float visibleAmount = releasingSprite.Hitbox.Bottom - Hitbox.Bounds.Bottom;
				result.Y = (int)(releasingSprite.Hitbox.Height - visibleAmount);
				result.Height = (int)(visibleAmount);
			}
			else { throw new InvalidOperationException("There should be no sprite releasing now."); }

			return result;
		}

		private void OnBlockHit(VerticalDirection releaseDirection)
		{
			if (isEmpty || isReleasingSprite) { return; }
			if (ContainedSprite == null) { return; }

			releaseDirection = ChangeReleaseDirectionIfNeeded(releaseDirection);

			// Set the visual displacement. Setting visualDisplacementDirection causes UpdateVisualDisplacement to start automatically.
			visualDisplacementTarget = MaximumVisualDisplacement.Value;
			visualDisplacementDirection = releaseDirection;

			// Clone a sprite and set its properties.
			releasingSprite = CloneSpriteToRelease();
			spriteReleaseAmount = 0f;
		}

		private void BeginRelease(VerticalDirection releaseDirection)
		{
			releasingSprite = CloneSpriteToRelease();
			isReleasingSprite = true;
			releasingSpriteDirection = releaseDirection;
		}

		private void UpdateReleasingSprite()
		{
			if (!isReleasingSprite) { return; }

			float releaseSpriteDelta = 1f / (SpriteReleaseLength.Value * 60f);
			float releaseDistanceDelta = releasingSprite.Hitbox.Height * releaseSpriteDelta;
			float newSpriteYPosition = releasingSprite.Position.Y + (releaseDistanceDelta * ((releasingSpriteDirection == VerticalDirection.Up) ? -1f : 1f));

			releasingSprite.Position = new Vector2(releasingSprite.Position.X, newSpriteYPosition);

			releaseSpriteProgress += releaseSpriteDelta;

			if (releaseSpriteProgress >= 1f)
			{
				EndRelease();
			}
		}

		private void EndRelease()
		{
			isReleasingSprite = false;
			Owner.AddSpriteOnNextFrame(releasingSprite);
			releasingSprite = null;
			releaseSpriteProgress = 0f;
			releasingSpriteDirection = VerticalDirection.None;

			switch (ReleaseType)
			{
				case QuestionBlockItemReleaseType.FixedQuantity:
					Quantity--;
					if (Quantity == 0) { isEmpty = true; }
					break;
				case QuestionBlockItemReleaseType.FixedTime:
					break;
				case QuestionBlockItemReleaseType.FixedTimeWithMaximumQuantity:
					Quantity--;
					if (Quantity == 0) { isEmpty = true; }
					break;
				case QuestionBlockItemReleaseType.FixedTimeWithBonusAction:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// If there's a tile directly above or below this question block, we need to switch the direction we release it.
		/// </summary>
		/// <param name="releaseDirection">The initial release direction.</param>
		/// <returns>
		/// If there's no tile in the release direction, the initial release direction.
		/// If there is a tile in the initial release direction, the opposite direction,
		/// If there's a tile in both direction, VerticalDirection.None.
		/// </returns>
		private VerticalDirection ChangeReleaseDirectionIfNeeded(VerticalDirection releaseDirection)
		{
			Vector2 upCheckpoint = new Vector2(Hitbox.Bounds.Center.X, Hitbox.Bounds.Top - (GameServices.GameObjectSize.Y / 2f));
			Vector2 downCheckpoint = new Vector2(Hitbox.Bounds.Center.X, Hitbox.Bounds.Bottom + (GameServices.GameObjectSize.Y / 2f));

			Tile tileAbove = Owner.GetTileAtPosition(upCheckpoint);
			Tile tileBelow = Owner.GetTileAtPosition(downCheckpoint);

			if (tileAbove != null && tileBelow != null) { return VerticalDirection.None; }
			else if (releaseDirection == VerticalDirection.Up && tileAbove != null) { return VerticalDirection.Down; }
			else if (releaseDirection == VerticalDirection.Down && tileBelow != null) { return VerticalDirection.Up; }
			return releaseDirection;
		}

		private Sprite CloneSpriteToRelease()
		{
			Sprite spriteToRelease = ContainedSprite.Clone();
			spriteToRelease.Initialize(Owner);
			spriteToRelease.LoadContent();
			spriteToRelease.Position = Position;

			return spriteToRelease;
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
			// The question block is triggered if it's not empty,
			// if the sprite is a player that hits the block from below
			//  (player.Center beneath Bottom, player's Velocity upward)
			// or player is ground pounding from above
			//  (player.Center above Top)
			// or if the sprite has attribute ShellSpinning and hits on the side

			if (isEmpty) { return; }
			if (sprite.IsPlayer)
			{
				Vector2 playerCenter = sprite.Hitbox.Center;
				float playerYVelocity = sprite.Velocity.Y;
				float thisBottom = Hitbox.Bounds.Bottom;
				float thisTop = Hitbox.Bounds.Top;
				bool isGroundPounding = sprite.HasAttribute("GroundPounding");

				if (playerCenter.Y > thisBottom)
				{
					OnBlockHit(VerticalDirection.Up);
				}
				else if (playerCenter.Y < thisTop && isGroundPounding)
				{
					OnBlockHit(VerticalDirection.Down);
				}
			}
			else if (sprite.HasAttribute("ShellSpinning"))
			{
				Vector2 spriteCenter = sprite.Hitbox.Center;
				float spriteXVelocity = sprite.Velocity.X;
				float thisLeft = Hitbox.Bounds.Left;
				float thisRight = Hitbox.Bounds.Right;

				if (spriteCenter.X < thisLeft)
				{
					OnBlockHit(VerticalDirection.Up);
				}
				else if (spriteCenter.X > thisRight)
				{
					OnBlockHit(VerticalDirection.Up);
				}
			}
		}

		public override bool OnEditorDrop(Sprite sprite)
		{
			ContainedSprite = sprite;
			return true;
		}
	}

	public enum QuestionBlockItemReleaseType
	{
		FixedQuantity,
		FixedTime,
		FixedTimeWithMaximumQuantity,
		FixedTimeWithBonusAction // to be supported
	}
}
