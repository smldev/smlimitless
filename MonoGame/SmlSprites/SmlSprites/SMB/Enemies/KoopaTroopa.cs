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
		private KoopaVariety variety;

		[DefaultValue(KoopaVariety.Green), Description("The color of this Koopa Troopa.")]
		public KoopaVariety Variety
		{
			get
			{
				return variety;
			}
			set
			{
				variety = value;
				if (graphics != null)
				{
					graphics.CurrentObjectName = AppendTypeSuffix(graphics.CurrentObjectName.Split('_')[0]);
				}
				SetBehavior();
			}
		}

		public override string EditorCategory => "Enemies";
		public override bool IsPlayer => false;

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
			Variety = (KoopaVariety)customObjects.GetInt("type");
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

			ChasePlayerComponent chasePlayer = new ChasePlayerComponent(this, 60);
			chasePlayer.NearestPlayerDirectionUpdated += ChasePlayer_NearestPlayerDirectionUpdated;
			Components.Add(chasePlayer);

			ShelledEnemyComponent shelledEnemy = new ShelledEnemyComponent(this, WalkingSpeed.Value, ShellSpinningSpeed.Value, FramesUntilBeginEmerge.Value, FramesUntilEmerge.Value);
			shelledEnemy.StateChanged += ShelledEnemy_StateChanged;
			Components.Add(shelledEnemy);
			SetBehavior();
		}

		private void ChasePlayer_NearestPlayerDirectionUpdated(object sender, EventArgs e)
		{
			ChasePlayerComponent chasePlayer = (ChasePlayerComponent)sender;
			WalkerComponent walker = GetComponent<WalkerComponent>();
			FlaggedDirection nearestPlayerDirection = chasePlayer.NearestPlayerDirection;

			if ((nearestPlayerDirection & FlaggedDirection.Left) != 0)
			{
				facingDirection = walker.Direction = SMLimitless.Direction.Left;
			}
			else if ((nearestPlayerDirection & FlaggedDirection.Right) != 0)
			{
				facingDirection = walker.Direction = SMLimitless.Direction.Right;
			}
		}

		private void ShelledEnemy_StateChanged(object sender, EventArgs e)
		{
			SetGraphicsObject();

			switch (GetComponent<ShelledEnemyComponent>().State)
			{
				case ShelledEnemyComponent.ShelledEnemyState.Shell:
					RemoveAttribute("ShellSpinning");
					break;
				case ShelledEnemyComponent.ShelledEnemyState.ShellSpinning:
					AddAttribute("ShellSpinning");
					break;
				case ShelledEnemyComponent.ShelledEnemyState.Walking:
				case ShelledEnemyComponent.ShelledEnemyState.Emerging:
				default:
					break;
			}
		}

		private void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Vector2 flipVelocity = new Vector2(40f, -100f);

			isFlippedOver = true;
			Velocity = flipVelocity;
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			SpriteCollisionMode = SpriteCollisionMode.NoCollision;
			Components.ForEach(c => c.IsActive = false);
			graphics.CurrentObjectName = AppendTypeSuffix("shell");

			Owner.HUDInfo.AddScore(200);
		}

		private void SetBehavior()
		{
			var shelledEnemyComponent = GetComponent<ShelledEnemyComponent>();
			if (shelledEnemyComponent == null) { return; }

			switch (Variety)
			{
				case KoopaVariety.Green:
				case KoopaVariety.CaveGreen:
					shelledEnemyComponent.Behavior = ShelledEnemyComponent.ShelledEnemyBehavior.DontTurnOnCliffs;
					break;
				case KoopaVariety.Red:
					shelledEnemyComponent.Behavior = ShelledEnemyComponent.ShelledEnemyBehavior.TurnOnCliffs;
					break;
				case KoopaVariety.Yellow:
					shelledEnemyComponent.Behavior = ShelledEnemyComponent.ShelledEnemyBehavior.ChasePlayer;
					break;
				default:
					throw new InvalidOperationException();
			}
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

		public override void Draw(Vector2 cropping)
		{
			SpriteEffects effects = SpriteEffects.None;
			if (facingDirection == SMLimitless.Direction.Right) { effects |= SpriteEffects.FlipHorizontally; }
			if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

			graphics.Draw(new Vector2(Position.X, Position.Y - 8f), cropping, Color.White, effects);
		}

		public override void Update()
		{
			base.Update();

			if (Velocity.X < 0f && facingDirection == SMLimitless.Direction.Right)
			{
				facingDirection = SMLimitless.Direction.Left;

				var walker = GetComponent<WalkerComponent>();
				if (walker != null) { walker.Direction = facingDirection; }
			}
			else if (Velocity.X > 0f && facingDirection == SMLimitless.Direction.Left)
			{
				facingDirection = SMLimitless.Direction.Right;
				var walker = GetComponent<WalkerComponent>();
				if (walker != null) { walker.Direction = facingDirection; }
			}

			graphics.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				type = (int)Variety
			};
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			base.HandleSpriteCollision(sprite, resolutionDistance);
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		private void SetGraphicsObject()
		{
			switch (GetComponent<ShelledEnemyComponent>().State)
			{
				case ShelledEnemyComponent.ShelledEnemyState.Walking:
					graphics.CurrentObjectName = AppendTypeSuffix("walking");
					break;
				case ShelledEnemyComponent.ShelledEnemyState.Shell:
					graphics.CurrentObjectName = AppendTypeSuffix("shell");
					break;
				case ShelledEnemyComponent.ShelledEnemyState.Emerging:
					graphics.CurrentObjectName = AppendTypeSuffix("emerging");
					break;
				case ShelledEnemyComponent.ShelledEnemyState.ShellSpinning:
					graphics.CurrentObjectName = AppendTypeSuffix("shellSpinning");
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private string AppendTypeSuffix(string graphicsName)
		{
			switch (Variety)
			{
				case KoopaVariety.Green:
					return graphicsName + "_green";
				case KoopaVariety.CaveGreen:
					return graphicsName + "_cave";
				case KoopaVariety.Red:
					return graphicsName + "_red";
				case KoopaVariety.Yellow:
					return graphicsName + "_yellow";
				default:
					throw new InvalidOperationException();
			}
		}
	}

	public enum KoopaVariety
	{
		Green = 0,
		CaveGreen = 1,
		Red = 2,
		Yellow = 3
	}
}
