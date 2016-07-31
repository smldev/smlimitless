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
using SmlSprites.Helpers;

namespace SmlSprites.SMB.Enemies
{
	public sealed class Lakitu : Sprite
	{
		private const int GraphicsFrameTransitionFrames = 10;
	
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

		static Lakitu()
		{
			DesiredYPositionPercentage = new PhysicsSetting<float>("SMB Lakitu: Desired Y Position Screen %", 0f, 1f, 0.3f, PhysicsSettingType.FloatingPoint);
			XAcceleration = new PhysicsSetting<float>("SMB Lakitu: X Acceleration (px/sec²)", 0f, 400f, 40f, PhysicsSettingType.FloatingPoint);
			YAcceleration = new PhysicsSetting<float>("SMB Lakitu: Y Acceleration (px/sec²)", 0f, 400f, 40f, PhysicsSettingType.FloatingPoint);
			MaxXVelocity = new PhysicsSetting<float>("SMB Lakitu: Max X Velocity (px/sec)", 0f, 4000f, 1200f, PhysicsSettingType.FloatingPoint);
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

			base.Initialize(owner);
		}

		public override void Draw()
		{
			if (isActive || Owner.EditorActive)
			{
				SpriteEffects effects = SpriteEffects.None;
				if (Velocity.X < 0f) { effects |= SpriteEffects.FlipHorizontally; }
				if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

				graphics.Draw(Position, Color.White, effects);
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
			Vector2 center = Hitbox.Center;
			float top = Hitbox.Top;
			Vector2 screenCenter = Owner.Camera.Viewport.Center;
			float desiredY = DesiredYPosition;
			column.Update();

			if (column.HasPlayers)
			{
				if (!isActive)
				{
					ActivateLakitu();
				}

				Components.ForEach(c => c.Update());
				PreviousPosition = Position;

				if (throwTimer == 0)
				{
					ThrowSprite();
					throwTimer = (int)(ThrowDelay.Value * 60);
				}

				Acceleration = new Vector2((center.X < screenCenter.X) ? XAcceleration.Value : -XAcceleration.Value, 0f);

				if (Math.Abs(Position.Y - desiredY) < 1f)
				{
					Acceleration = new Vector2(Acceleration.X, 0f);
					Velocity = new Vector2(Acceleration.X, 0f);	// TODO: make this so that Lakitu slows down before reaching the desired Y coordinate
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
					Acceleration = new Vector2((center.X < screenCenter.X) ? -XAcceleration.Value : XAcceleration.Value, 0f);
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
			Position = new Vector2(xPosition, DesiredYPosition);
			Acceleration = new Vector2(XAcceleration.Value * (facingDirection * -1), 0f);

			isActive = true;
		}

		private void ThrowSprite()
		{
			if (thrownSprites.Count >= MaximumOnscreenSprites.Value) { return; }
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}
	}
}
