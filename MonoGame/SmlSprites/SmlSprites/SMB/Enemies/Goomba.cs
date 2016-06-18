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
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSprites.SMB.Enemies
{
	public sealed class Goomba : Sprite
	{
		public static PhysicsSetting<float> WalkingSpeed;

		private ComplexGraphicsObject graphics;
		private bool isFlippedOver = false;
		private GoombaPalette palette;

		[DefaultValue(GoombaPalette.Overworld), Description("The palette used by this Goomba.")]
		public GoombaPalette Palette
		{
			get
			{
				return palette;
			}
			set
			{
				palette = value;
				if (graphics != null)
				{
					graphics.CurrentObjectName = AppendPaletteNameSuffix(graphics.CurrentObjectName.Split('_')[0]);
				}
			}
		}

		public override string EditorCategory
		{
			get
			{
				return "Enemies";
			}
		}

		static Goomba()
		{
			WalkingSpeed = new PhysicsSetting<float>("SMB Goomba: Walking Speed (p/s)", 0.01f, 256f, 32f, PhysicsSettingType.FloatingPoint);
		}

		public Goomba()
		{
			Size = new Vector2(16f);
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			Palette = (GoombaPalette)customObjects.GetInt("palette");
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBGoomba");

			graphics.CurrentObjectName = AppendPaletteNameSuffix("walking");

			HealthComponent healthComponent = new HealthComponent(1, 1, new string[] { });
			healthComponent.SpriteKilled += HealthComponent_SpriteKilled;

			Components.Add(new WalkerComponent(this, ResolveDirection(this, Direction, owner), 32f));
			Components.Add(healthComponent);
			Components.Add(new DamageComponent());
		}

		private void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Vector2 flipVelocity = new Vector2(40f, -100f);

			if (e.DamageType == SpriteDamageTypes.PlayerStomp)
			{
				Velocity = Vector2.Zero;
				graphics.CurrentObjectName = AppendPaletteNameSuffix("flattened");
				SpriteCollisionMode = SpriteCollisionMode.NoCollision;
				Components.RemoveAll(c => c is WalkerComponent);
				SMLimitless.Components.ActionScheduler.Instance.ScheduleAction(() => Owner.RemoveSprite(this), 120);
			}
			else
			{
				isFlippedOver = true;
				Velocity = flipVelocity;
				TileCollisionMode = SpriteCollisionMode.NoCollision;
				SpriteCollisionMode = SpriteCollisionMode.NoCollision;
				Components.RemoveAll(c => c is WalkerComponent);
			}
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White, (!isFlippedOver) ? SpriteEffects.None : SpriteEffects.FlipVertically);
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
				palette = (int)Palette
			};
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite is Players.PlayerMario)
			{
				Players.PlayerMario playerMario = (Players.PlayerMario)sprite;
				if (!(playerMario.Hitbox.Bottom < Hitbox.Center.Y && playerMario.Velocity.Y >= 0f))
				{
					// The player has contacted the goomba, but hasn't stomped on it
					var damageComponent = GetComponent<DamageComponent>();
					damageComponent.PerformDamage(playerMario, SpriteDamageTypes.General, 1);
				}
			}

			base.HandleSpriteCollision(sprite, resolutionDistance);
		}

		private string AppendPaletteNameSuffix(string graphicsObjectName)
		{
			switch (Palette)
			{
				case GoombaPalette.Overworld:
					return graphicsObjectName + "_OW";
				case GoombaPalette.Cave:
					return graphicsObjectName + "_cave";
				case GoombaPalette.Castle:
					return graphicsObjectName + "_castle";
				default:
					throw new InvalidOperationException();
			}
		}
	}

	public enum GoombaPalette
	{
		Overworld,
		Cave,
		Castle
	}
}
