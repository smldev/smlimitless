using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public sealed class Spiny : Sprite
	{
		private static PhysicsSetting<float> WalkingSpeed;

		private ComplexGraphicsObject graphics;
		private bool isFlippedOver = false;
		private SMLimitless.Direction FacingDirection => (Velocity.X <= 0f) ? SMLimitless.Direction.Left : SMLimitless.Direction.Right;
		private SpinyState state;

		public override string EditorCategory => "Enemies";

		[DefaultValue(SpinyState.Spiny), Description("The initial state of this Spiny.")]
		public SpinyState SpinyState
		{
			get { return state; }
			set
			{
				state = value;
				UpdateGraphicsAndComponents();
			}
		}

		private void UpdateGraphicsAndComponents()
		{
			if (graphics == null) { return; }

			var walker = GetComponent<WalkerComponent>();

			if (state == SpinyState.Spiny)
			{
				graphics.CurrentObjectName = "walking";
				if (walker != null) { walker.IsActive = true;}
			}
			else if (state == SpinyState.Egg)
			{
				graphics.CurrentObjectName = "egg";
				if (walker != null) { walker.IsActive = false; }
			}
			else { throw new InvalidOperationException(); }
		}

		static Spiny()
		{
			WalkingSpeed = new PhysicsSetting<float>("SMB Spiny: Walking Speed", 0.01f, 256f, 32f, PhysicsSettingType.FloatingPoint);
		}

		public Spiny()
		{
			Size = new Vector2(16f);
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBSpiny");

			graphics.CurrentObjectName = (SpinyState == SpinyState.Spiny) ? "walking" : "egg";

			HealthComponent health = new HealthComponent(1, 1, new string[] { SpriteDamageTypes.PlayerStomp });
			health.SpriteKilled += Health_SpriteKilled;

			Components.Add(new WalkerComponent(this, ResolveDirection(this, Direction, Owner), WalkingSpeed.Value));
			Components.Add(health);
			Components.Add(new DamageComponent());
		}

		private void Health_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Vector2 flipVelocity = new Vector2(40f, -100f);

			isFlippedOver = true;
			Velocity = flipVelocity;
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			SpriteCollisionMode = SpriteCollisionMode.NoCollision;

			var walker = GetComponent<WalkerComponent>();
			if (walker != null) { walker.IsActive = false; }
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			SpinyState = (SpinyState)customObjects.GetInt("spinyState");
		}

		public override void Draw()
		{
			SpriteEffects effects = SpriteEffects.None;
			if (FacingDirection == SMLimitless.Direction.Right) { effects |= SpriteEffects.FlipHorizontally; }
			if (isFlippedOver) { effects |= SpriteEffects.FlipVertically; }

			graphics.Draw(Position, Color.White, effects);
		}

		public override void Update()
		{
			base.Update();
			graphics.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				spinyState = SpinyState
			};
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer)
			{
				var damage = GetComponent<DamageComponent>();
				damage.PerformDamage(sprite, SpriteDamageTypes.General, 1);
			}

			base.HandleSpriteCollision(sprite, resolutionDistance);
		}

		public override void HandleTileCollision(Tile tile, Vector2 resolutionDistance)
		{
			if (SpinyState == SpinyState.Egg && resolutionDistance.Y < 0f)
			{
				SpinyState = SpinyState.Spiny;
			}
		}
	}

	public enum SpinyState
	{
		Egg,
		Spiny
	}
}
