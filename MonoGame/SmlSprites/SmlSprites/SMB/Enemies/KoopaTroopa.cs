using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless;
using SMLimitless.Components;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSprites.SMB.Enemies
{
	public sealed class KoopaTroopa : Sprite
	{
		public static PhysicsSetting<float> WalkingSpeed;
		public static PhysicsSetting<float> ShellSpinningSpeed;
		public static PhysicsSetting<int> FramesUntilBeginEmerge;
		public static PhysicsSetting<int> FramesUntilEmerge;

		private ComplexGraphicsObject graphics;
		private bool isFlippedOver = false;
		private Direction facingDirection = SMLimitless.Direction.Left;
		private KoopaType type;
		private KoopaStatus status;
		private ActionScheduler.ScheduledAction emergeAction = null;

		private KoopaStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (value == KoopaStatus.Walking)
				{
					Components.Add(new WalkerComponent(this, (facingDirection == SMLimitless.Direction.Left) ? SpriteDirection.Left : SpriteDirection.Right, WalkingSpeed.Value, (Type == KoopaType.Red)));
				}
				else if (value == KoopaStatus.ShellSpinning)
				{
					Components.RemoveAll(c => c is WalkerComponent);
					Components.Add(new WalkerComponent(this, (facingDirection == SMLimitless.Direction.Left) ? SpriteDirection.Left : SpriteDirection.Right, ShellSpinningSpeed.Value, false));
				}
				else
				{
					Components.RemoveAll(c => c is WalkerComponent);
					Velocity = new Vector2(0f, Velocity.Y);
				}
				status = value;
				SetGraphicsObject();
			}
		}

		[DefaultValue(KoopaType.Green), Description("The color of this Koopa Troopa.")]
		public KoopaType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
				if (graphics != null)
				{
					graphics.CurrentObjectName = AppendTypeSuffix(graphics.CurrentObjectName.Split('_')[0]);
				}

				if (type == KoopaType.Red)
				{
					Components.RemoveAll(c => c is WalkerComponent);
					Components.Add(new WalkerComponent(this, (facingDirection == SMLimitless.Direction.Left) ? SpriteDirection.Left : SpriteDirection.Right, WalkingSpeed.Value, true));
				}
				else
				{
					Components.RemoveAll(c => c is WalkerComponent); ;
					Components.Add(new WalkerComponent(this, (facingDirection == SMLimitless.Direction.Left) ? SpriteDirection.Left : SpriteDirection.Right, WalkingSpeed.Value, false));
				}
			}
		}

		public override string EditorCategory => "Enemies";

		static KoopaTroopa()
		{
			WalkingSpeed = new PhysicsSetting<float>("SMB Koopa: Walking Speed (p/s)", 0.01f, 256f, 32f, PhysicsSettingType.FloatingPoint);
			ShellSpinningSpeed = new PhysicsSetting<float>("SMB Koopa: Shell Spinning Speed (px/sec)", 0.01f, 256f, 150f, PhysicsSettingType.FloatingPoint);
			FramesUntilBeginEmerge = new PhysicsSetting<int>("SMB Koopa: Frames Until Begin Emerge", 1, 1000, 600, PhysicsSettingType.Integer);
			FramesUntilEmerge = new PhysicsSetting<int>("SMB Koopa: Frames Until Emerge", 1, 1000, 120, PhysicsSettingType.Integer);
		}

		public KoopaTroopa()
		{
			Size = new Vector2(16f);
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			Type = (KoopaType)customObjects.GetInt("type");
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBKoopaTroopa");

			graphics.CurrentObjectName = AppendTypeSuffix("walking");

			HealthComponent healthComponent = new HealthComponent(1, 1, new string[] { SpriteDamageTypes.PlayerStomp });
			healthComponent.SpriteKilled += HealthComponent_SpriteKilled;

			SpriteDirection initialDirection = ResolveDirection(this, Direction, Owner);
			facingDirection = (initialDirection == SpriteDirection.Left) ? SMLimitless.Direction.Left : SMLimitless.Direction.Right;
			Components.Add(healthComponent);
			Components.Add(new WalkerComponent(this, initialDirection, WalkingSpeed.Value));
			Components.Add(new DamageComponent());
		}

		private void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Vector2 flipVelocity = new Vector2(40f, -100f);

			isFlippedOver = true;
			Velocity = flipVelocity;
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			SpriteCollisionMode = SpriteCollisionMode.NoCollision;
			Components.Clear();
			graphics.CurrentObjectName = AppendTypeSuffix("shell");
		}

		public override void Draw()
		{
			// Each graphic is 24px high, but the hitbox is only 16px high
			// Therefore we want to draw sprites 8px higher than they actually are
			// Walking sprites are actually 24px tall, and the 8px above shell sprites are empty
			SpriteEffects effects = SpriteEffects.None;
			if (facingDirection == SMLimitless.Direction.Right) { effects |= SpriteEffects.FlipHorizontally; }
			if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

			graphics.Draw(new Vector2(Position.X, Position.Y - 8f), Color.White, effects);
		}

		public override void Update()
		{
			base.Update();

			if (Velocity.X < 0f && facingDirection == SMLimitless.Direction.Right)
			{
				facingDirection = SMLimitless.Direction.Left;
			}
			else if (Velocity.X > 0f && facingDirection == SMLimitless.Direction.Left)
			{
				facingDirection = SMLimitless.Direction.Right;
			}

			graphics.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				type = (int)Type
			};
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (Status == KoopaStatus.Walking)
			{
				HandleSpriteCollisionWhileWalking(sprite, resolutionDistance);
			}
			else if (Status == KoopaStatus.Shell || Status == KoopaStatus.Emerging)
			{
				HandleSpriteCollisionWhileShell(sprite, resolutionDistance);
			}
			else if (Status == KoopaStatus.ShellSpinning)
			{
				HandleSpriteCollisionWhileShellSpinning(sprite, resolutionDistance);
			}

			base.HandleSpriteCollision(sprite, resolutionDistance);
		}

		private void HandleSpriteCollisionWhileWalking(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite is Players.PlayerMario)
			{
				Players.PlayerMario playerMario = (Players.PlayerMario)sprite;
				if (playerMario.Hitbox.Bottom < Hitbox.Center.Y && playerMario.Velocity.Y <= 0f)
				{
					GoToShell();
				}
			}
		}

		private void GoToShell()
		{
			// Player has stomped this koopa, so transition to Shell status
			Status = KoopaStatus.Shell;
			emergeAction = ActionScheduler.Instance.ScheduleAction(() =>
			{
				Status = KoopaStatus.Emerging;
				emergeAction = ActionScheduler.Instance.ScheduleActionOnNextFrame(() =>
				{
					Status = KoopaStatus.Walking;
					isFlippedOver = false;
				}, FramesUntilEmerge.Value);
			}, FramesUntilBeginEmerge.Value);
		}

		private void HandleSpriteCollisionWhileShell(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite is Players.PlayerMario)
			{
				Players.PlayerMario playerMario = (Players.PlayerMario)sprite;

				// If the player's center is to the right (or equal to) our center, go left
				// Otherwise, go right
				facingDirection = (playerMario.Hitbox.Center.X >= Hitbox.X) ? SMLimitless.Direction.Left : SMLimitless.Direction.Right;
				Status = KoopaStatus.ShellSpinning;
				ActionScheduler.Instance.CancelScheduledAction(emergeAction);
			}
		}

		private void HandleSpriteCollisionWhileShellSpinning(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite is Players.PlayerMario)
			{
				Players.PlayerMario playerMario = (Players.PlayerMario)sprite;
				if (playerMario.Hitbox.Bottom < Hitbox.Center.Y && playerMario.Velocity.Y <= 0f)
				{
					GoToShell();
				}
				else
				{
					var damageComponent = GetComponent<DamageComponent>();
					if (damageComponent != null) { damageComponent.PerformDamage(playerMario, SpriteDamageTypes.ShellSpinning, 1); }
				}
			}
			else
			{
				var damageComponent = GetComponent<DamageComponent>();
				if (damageComponent != null) { damageComponent.PerformDamage(sprite, SpriteDamageTypes.ShellSpinning, 1); }
			}
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		private void SetGraphicsObject()
		{
			switch (status)
			{
				case KoopaStatus.Walking:
					graphics.CurrentObjectName = AppendTypeSuffix("walking");
					break;
				case KoopaStatus.Shell:
					graphics.CurrentObjectName = AppendTypeSuffix("shell");
					break;
				case KoopaStatus.Emerging:
					graphics.CurrentObjectName = AppendTypeSuffix("emerging");
					break;
				case KoopaStatus.ShellSpinning:
					graphics.CurrentObjectName = AppendTypeSuffix("shellSpinning");
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private string AppendTypeSuffix(string graphicsName)
		{
			switch (Type)
			{
				case KoopaType.Green:
					return graphicsName + "_green";
				case KoopaType.CaveGreen:
					return graphicsName + "_cave";
				case KoopaType.Red:
					return graphicsName + "_red";
				case KoopaType.Yellow:
					return graphicsName + "_yellow";
				default:
					throw new InvalidOperationException();
			}
		}
	}

	public enum KoopaType
	{
		Green = 0,
		CaveGreen = 1,
		Red = 2,
		Yellow = 3
	}

	public enum KoopaStatus
	{ 
		Walking,
		Shell,
		Emerging,
		ShellSpinning
	}
}
