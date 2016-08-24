using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.IO.LevelSerializers;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;
using SmlSprites.Helpers;

namespace SmlSprites.SMB.Enemies
{
	public sealed class Lakitu : Sprite
	{
		private const int GraphicsFrameTransitionFrames = 10;

		public static PhysicsSetting<float> DesiredXPositionPercentage;
		public static PhysicsSetting<float> DesiredYPositionPercentage;
		public static PhysicsSetting<float> XAcceleration;
		public static PhysicsSetting<float> YAcceleration;
		public static PhysicsSetting<float> MaxXVelocity;
		public static PhysicsSetting<float> ThrownAdditionalXVelocity;
		public static PhysicsSetting<float> ThrownAdditionalYVelocity;
		public static PhysicsSetting<int> MaximumOnscreenSprites;
		public static PhysicsSetting<float> ThrowDelay;
		public static PhysicsSetting<float> RespawnDelay;

		private ComplexGraphicsObject graphics;
		private bool isFlippedOver = false;
		private bool isActive = false;
		private PlayerSensingColumn column;
		private List<Sprite> thrownSprites = new List<Sprite>();

		private int throwTimer;
		private int respawnTimer;
		private int frameTransitionTimer = GraphicsFrameTransitionFrames;
		private Direction desiredXPositionDirection = SMLimitless.Direction.Left;

		public override string EditorCategory => "Enemies";

		[Description("If the player is to the right of this X-coordinate, Lakitu will spawn.")]
		public float SpawnBandStart { get; set; }

		[Description("If the player is to the left of this X-coordinate, Lakitu will spawn.")]
		public float SpawnBandEnd { get; set; }

		[Browsable(false)]
		public Sprite ThrownSprite { get; set; }

		[Description("A value indicating whether a sprite has been dropped onto this Lakitu.")]
		public bool HasSpriteDropped
		{
			get { return ThrownSprite != null; }
			set
			{
				if (!value)
				{
					ThrownSprite = null;
				}
			}
		}

		[Browsable(false)]
		public float DesiredYPosition
		{
			get
			{
				float cameraTop = Owner.Camera.Viewport.Top;
				return cameraTop + (Owner.Camera.Viewport.Height * DesiredYPositionPercentage.Value);
			}
		}

		private float DesiredXPositionLeft
		{
			get
			{
				return Owner.Camera.Viewport.Left + (Owner.Camera.Viewport.Width * DesiredXPositionPercentage.Value);
			}
		}

		private float DesiredXPositionRight
		{
			get
			{
				return Owner.Camera.Viewport.Right - (Owner.Camera.Viewport.Width * DesiredXPositionPercentage.Value);
			}
		}

		private bool ShouldSwitchDirections
		{
			get
			{
				if (desiredXPositionDirection == SMLimitless.Direction.Left)
				{
					return Position.X <= DesiredXPositionLeft;
				}
				else if (desiredXPositionDirection == SMLimitless.Direction.Right)
				{
					return Position.X >= DesiredXPositionRight;
				}
				throw new InvalidOperationException($"The desired X position direction has an invalid value of {desiredXPositionDirection}");
			}
		}

		static Lakitu()
		{
			DesiredXPositionPercentage = new PhysicsSetting<float>("SMB Lakitu: Desired X Position Screen %", 0f, 1f, 0.1f, PhysicsSettingType.FloatingPoint);
			DesiredYPositionPercentage = new PhysicsSetting<float>("SMB Lakitu: Desired Y Position Screen %", 0f, 1f, 0.05f, PhysicsSettingType.FloatingPoint);
			XAcceleration = new PhysicsSetting<float>("SMB Lakitu: X Acceleration (px/sec²)", 0f, 100f, 40f, PhysicsSettingType.FloatingPoint);
			YAcceleration = new PhysicsSetting<float>("SMB Lakitu: Y Acceleration (px/sec²)", 0f, 100f, 40f, PhysicsSettingType.FloatingPoint);
			MaxXVelocity = new PhysicsSetting<float>("SMB Lakitu: Max X Velocity (px/sec)", 0f, 500f, 80f, PhysicsSettingType.FloatingPoint);
			ThrownAdditionalXVelocity = new PhysicsSetting<float>("SMB Lakitu: Thrown Additional X Velocity (px/sec)", 0f, 200f, 20f, PhysicsSettingType.FloatingPoint);
			ThrownAdditionalYVelocity = new PhysicsSetting<float>("SMB Lakitu: Thrown Additional Y Velocity (px/sec)", 0f, 200f, 20f, PhysicsSettingType.FloatingPoint);
			MaximumOnscreenSprites = new PhysicsSetting<int>("SMB Lakitu: Maximum Onscreen Sprites", 0, 100, 5, PhysicsSettingType.Integer);
			ThrowDelay = new PhysicsSetting<float>("SMB Lakitu: Throw Delay (sec)", 0.5f, 50f, 4f, PhysicsSettingType.FloatingPoint);
			RespawnDelay = new PhysicsSetting<float>("SMB Lakitu: Respawn Delay (sec)", 0.5f, 50f, 10f, PhysicsSettingType.FloatingPoint);
		}

		public Lakitu()
		{
			Size = new Vector2(16f, 24f);
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			isActive = false;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			SpawnBandStart = customObjects.GetFloat("spawnBandStart");
			SpawnBandEnd = customObjects.GetFloat("spawnBandEnd");
			ThrownSprite = TypeSerializer.DeserializeSprite(customObjects, "thrownSprite");
		}

		public override void Initialize(Section owner)
		{
			column = new PlayerSensingColumn(owner, SpawnBandStart, SpawnBandEnd);
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBLakitu");

			ActiveState = SpriteActiveState.AlwaysActive;

			var health = new HealthComponent(1, 1, new string[] { });
			health.SpriteKilled += Health_SpriteKilled;

			Components.Add(health);
			Components.Add(new DamageComponent());

			base.Initialize(owner);
		}

		private void Health_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			if (e.DamageType == SpriteDamageTypes.PlayerStomp)
			{
				Velocity = Vector2.Zero;
				isFlippedOver = true;
				SpriteCollisionMode = TileCollisionMode = SpriteCollisionMode.NoCollision;
				respawnTimer = (int)(RespawnDelay.Value * 60f);

				Owner.HUDInfo.AddScore(200);
			}
		}

		public override void Draw()
		{
			if (isActive)
			{
				SpriteEffects effects = SpriteEffects.None;
				if (Velocity.X < 0f) { effects |= SpriteEffects.FlipHorizontally; }
				if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

				graphics.Draw(Position, Color.White, effects);
			}
			else if (Owner.EditorActive)
			{
				graphics.Draw(Position, Color.White);
				if (HasSpriteDropped) { GameServices.SpriteBatch.DrawRectangle((int)Position.X, (int)Position.Y, 4, 4, Color.Lime); }
			}
		}

		public override void Draw(Rectangle cropping)
		{
			if (isActive)
			{
				SpriteEffects effects = SpriteEffects.None;
				if (Velocity.X < 0f) { effects |= SpriteEffects.FlipHorizontally; }
				if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

				graphics.Draw(Position, cropping, Color.White, effects);
			}
			else if (Owner.EditorActive)
			{
				graphics.Draw(Position, Color.White);
			}
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				spawnBandStart = SpawnBandStart,
				spawnBandEnd = SpawnBandEnd,
				thrownSprite = TypeSerializer.GetSpriteObjects(ThrownSprite)
			};
		}

		public override void Update()
		{
			float delta = GameServices.GameTime.GetElapsedSeconds();
			float desiredXPosition = (desiredXPositionDirection == SMLimitless.Direction.Left) ? DesiredXPositionLeft : DesiredXPositionRight;
			float top = Hitbox.Top;
			float desiredY = DesiredYPosition;
			column.Update();

			if (respawnTimer > 0)
			{
				if (!Hitbox.IntersectsIncludingEdges(Owner.Camera.Viewport))
				{
					isActive = false;
				}
				respawnTimer--;
				base.Update();	// to apply gravity
				return;
			}

			if (column.HasPlayers)
			{
				if (!isActive)
				{
					ActivateLakitu();
				}

				Components.ForEach(c => c.Update());
				PreviousPosition = Position;

				throwTimer--;
				if (throwTimer <= 0)
				{
					ThrowSprite();
					throwTimer = (int)(ThrowDelay.Value * 60);
				}

				if (ShouldSwitchDirections)
				{
					desiredXPositionDirection = (desiredXPositionDirection == SMLimitless.Direction.Left) ? SMLimitless.Direction.Right : SMLimitless.Direction.Left;
					desiredXPosition = (desiredXPositionDirection == SMLimitless.Direction.Left) ? DesiredXPositionLeft : DesiredXPositionRight;
				}
				Acceleration = new Vector2((Position.X < desiredXPosition) ? XAcceleration.Value : -XAcceleration.Value, Acceleration.Y);

				if (Math.Abs(Position.Y - desiredY) < 1f)
				{
					Acceleration = new Vector2(Acceleration.X, 0f);
					Velocity = new Vector2(Velocity.X, 0f);	// TODO: make this so that Lakitu slows down before reaching the desired Y coordinate
				}
				else
				{
					Acceleration = new Vector2(Acceleration.X, (top < desiredY) ? YAcceleration.Value : -YAcceleration.Value);
				}

				Vector2 velocity = Vector2.Zero;
				velocity.X = MathHelper.Clamp(Velocity.X + (Acceleration.X * delta), -MaxXVelocity.Value, MaxXVelocity.Value);
				velocity.Y = Velocity.Y + (Acceleration.Y * delta);

				PreviousVelocity = Velocity;
				Velocity = velocity;
			}
			else
			{
				bool withinViewport = Hitbox.IntersectsIncludingEdges(Owner.Camera.Viewport);
				if (isActive && withinViewport)
				{
					// Find the nearest screen edge and accelerate toward it.
					Acceleration = new Vector2((Hitbox.Center.X < Owner.Camera.Viewport.Center.X) ? -XAcceleration.Value : XAcceleration.Value, 0f);
				}
				else if (isActive && !withinViewport)
				{
					isActive = false;
				}
			}

			thrownSprites.RemoveAll(s => s.ActiveState == SpriteActiveState.Inactive || s.ActiveState == SpriteActiveState.WaitingToLeaveBounds);
		}

		private void ActivateLakitu()
		{
			// Spawn this sprite, place it in front of the player just offscreen,
			// and put it at the desired Y position with the proper velocity.

			if (respawnTimer > 0)
			{
				respawnTimer--;
				return;
			}

			Sprite player = column.PlayersWithin.First();
			int facingDirection = (player.Velocity.X >= 0f) ? 1 : -1;
			float viewportWidth = Owner.Camera.Viewport.Width;

			// Lakitu spawns 10% past the edge of the viewport on whichever side the player is moving toward
			float xPosition = Owner.Camera.Viewport.X + ((viewportWidth + (viewportWidth * 0.1f)) * facingDirection);
			desiredXPositionDirection = (facingDirection == 1) ? SMLimitless.Direction.Left : SMLimitless.Direction.Right;
			Position = new Vector2(xPosition, DesiredYPosition);
			Acceleration = new Vector2(XAcceleration.Value * (facingDirection * -1), 0f);

			var health = GetComponent<HealthComponent>();
			health.Heal(1);

			isActive = true;
			isFlippedOver = false;
			SpriteCollisionMode = SpriteCollisionMode.OffsetNotify;
		}

		private void ThrowSprite()
		{
			if (thrownSprites.Count >= MaximumOnscreenSprites.Value) { return; }
			if (!Hitbox.IntersectsIncludingEdges(Owner.Camera.Viewport)) { return; }
			if (ThrownSprite == null) { return; }

			Sprite spriteToThrow = ThrownSprite.Clone();
			spriteToThrow.Initialize(Owner);
			spriteToThrow.LoadContent();
			spriteToThrow.Position = Position;
			spriteToThrow.Velocity = new Vector2(Velocity.X + ThrownAdditionalXVelocity.Value, Velocity.Y - ThrownAdditionalYVelocity.Value);
			spriteToThrow.IsMoving = true;
			Owner.AddSpriteOnNextFrame(spriteToThrow);
			thrownSprites.Add(spriteToThrow);

			SMLimitless.Debug.Logger.LogInfo($"Spawned sprite {((spriteToThrow.Hitbox.IntersectsIncludingEdges(Owner.Camera.Viewport) ? "INSIDE" : "OUTSIDE"))} {spriteToThrow.Position - Owner.Camera.Position}");
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override bool OnEditorDrop(Sprite sprite)
		{
			ThrownSprite = sprite;
			return true;
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (!isActive) { return; }

			if (sprite.IsPlayer)
			{
				if (!(sprite.Hitbox.Bottom < Hitbox.Center.Y && sprite.Velocity.Y >= 0f))
				{
					var damageComponent = GetComponent<DamageComponent>();
					damageComponent.PerformDamage(sprite, SpriteDamageTypes.General, 1);
				}
			}
			
			
			base.HandleSpriteCollision(sprite, resolutionDistance);
		}
	}
}
