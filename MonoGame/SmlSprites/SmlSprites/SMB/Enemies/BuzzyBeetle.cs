using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSprites.SMB.Enemies
{
	public sealed class BuzzyBeetle : Sprite
	{
		public static PhysicsSetting<float> WalkingSpeed;
		public static PhysicsSetting<float> ShellSpinningSpeed;
		public static PhysicsSetting<int> FramesUntilBeginEmerge;
		public static PhysicsSetting<int> FramesUntilEmerge;

		private ComplexGraphicsObject graphics;
		private bool isFlippedOver = false;
		private SMLimitless.Direction facingDirection = SMLimitless.Direction.Left;

		public override string EditorCategory => "Enemies";
		public override bool IsPlayer => false;

		static BuzzyBeetle()
		{
			WalkingSpeed = new PhysicsSetting<float>("SMB Buzzy Beetle: Walking Speed (p/s)", 0.01f, 256f, 32f, PhysicsSettingType.FloatingPoint);
			ShellSpinningSpeed = new PhysicsSetting<float>("SMB Buzzy Beetle: Shell Spinning Speed (px/sec)", 0.01f, 256f, 150f, PhysicsSettingType.FloatingPoint);
			FramesUntilBeginEmerge = new PhysicsSetting<int>("SMB Buzzy Beetle: Frames Until Begin Emerge", 1, 1000, 600, PhysicsSettingType.Integer);
			FramesUntilEmerge = new PhysicsSetting<int>("SMB Buzzy Beetle: Frames Until Emerge", 1, 1000, 120, PhysicsSettingType.Integer);
		}

		public BuzzyBeetle()
		{
			Size = new Vector2(16f);
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBBuzzyBeetle");

			HealthComponent health = new HealthComponent(1, 1, new string[] { SpriteDamageTypes.PlayerFireball, SpriteDamageTypes.PlayerStomp });
			health.SpriteKilled += Health_SpriteKilled;

			SpriteDirection initialDirection = ResolveDirection(this, Direction, Owner);
			facingDirection = (initialDirection == SpriteDirection.Left) ? SMLimitless.Direction.Left : SMLimitless.Direction.Right;
			Components.Add(health);
			Components.Add(new WalkerComponent(this, initialDirection, WalkingSpeed.Value));
			Components.Add(new DamageComponent());
			Components.Add(new ChasePlayerComponent(this, 60));

			ShelledEnemyComponent shelledEnemy = new ShelledEnemyComponent(this, WalkingSpeed.Value, ShellSpinningSpeed.Value, FramesUntilBeginEmerge.Value, FramesUntilEmerge.Value);
			shelledEnemy.StateChanged += ShelledEnemy_StateChanged;
			Components.Add(shelledEnemy);
		}

		private void ShelledEnemy_StateChanged(object sender, EventArgs e)
		{
			SetGraphicsObject();
		}

		private void Health_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Vector2 flipVelocity = new Vector2(40f, -100f);

			isFlippedOver = true;
			Velocity = flipVelocity;
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			SpriteCollisionMode = SpriteCollisionMode.NoCollision;
			Components.ForEach(c => c.IsActive = false);
			graphics.CurrentObjectName = "shell";
		}

		public override void Draw()
		{
			SpriteEffects effects = SpriteEffects.None;
			if (facingDirection == SMLimitless.Direction.Right) { effects |= SpriteEffects.FlipHorizontally; }
			if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

			graphics.Draw(Position, Color.White, effects);
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
			return new { };
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
					graphics.CurrentObjectName = "walking";
					break;
				case ShelledEnemyComponent.ShelledEnemyState.Shell:
				case ShelledEnemyComponent.ShelledEnemyState.Emerging:
					graphics.CurrentObjectName = "shell";
					break;
				case ShelledEnemyComponent.ShelledEnemyState.ShellSpinning:
					graphics.CurrentObjectName = "shellSpinning";
					break;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
