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
		private static PhysicsSetting<float> SpriteSpawnLength;

		private bool isEmpty = false;
		private bool isReleasingSprite = false;
		private AnimatedGraphicsObject questionGraphics;
		private StaticGraphicsObject emptyGraphics;
		private float visualDisplacementTarget;
		private float visualDisplacement;
		private VerticalDirection visualDisplacementDirection;
		private bool visualDisplacementReturning = false;
		private Sprite spawningSprite;
		private float spriteSpawnAmount;

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
			SpriteSpawnLength = new PhysicsSetting<float>("Question Block: Sprite Spawn Length (sec)", 0.01f, 10f, 1f, PhysicsSettingType.FloatingPoint);
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

			// Sprite release update
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
					// All done. We can start the sprite spawn now.
					visualDisplacementDirection = VerticalDirection.None;
					visualDisplacement = 0f;
					visualDisplacementTarget = 0f;
					visualDisplacementReturning = false;
					// isReleasingSprite = true;
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
			}
			else
			{
				emptyGraphics.Draw(Position, Color.White);
			}
		}

		private void ReleaseItem(VerticalDirection releaseDirection)
		{
			if (isEmpty || isReleasingSprite) { return; }
			if (ContainedSprite == null) { return; }

			releaseDirection = ChangeReleaseDirectionIfNeeded(releaseDirection);

			// Set the visual displacement.
			visualDisplacementTarget = MaximumVisualDisplacement.Value;
			visualDisplacementDirection = releaseDirection;

			// Clone a sprite and set its properties.
			spawningSprite = CloneSpriteToSpawn();
			spriteSpawnAmount = 0f;
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

		private Sprite CloneSpriteToSpawn()
		{
			Sprite spriteToSpawn = ContainedSprite.Clone();
			spriteToSpawn.Initialize(Owner);
			spriteToSpawn.LoadContent();
			spriteToSpawn.Position = Position;

			return spriteToSpawn;
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
					ReleaseItem(VerticalDirection.Up);
				}
				else if (playerCenter.Y < thisTop && isGroundPounding)
				{
					ReleaseItem(VerticalDirection.Down);
				}
			}
			else if (sprite.HasAttribute("ShellSpinning"))
			{
				Vector2 spriteCenter = sprite.Hitbox.Center;
				float spriteXVelocity = sprite.Velocity.X;
				float thisLeft = Hitbox.Bounds.Left;
				float thisRight = Hitbox.Bounds.Right;

				if (spriteXVelocity > 0f && spriteCenter.X < thisLeft)
				{
					ReleaseItem(VerticalDirection.Up);
				}
				else if (spriteXVelocity < 0f && spriteCenter.X > thisRight)
				{
					ReleaseItem(VerticalDirection.Up);
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
