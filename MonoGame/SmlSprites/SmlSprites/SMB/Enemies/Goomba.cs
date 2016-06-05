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
				ChangePalette();
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

			ChangePalette();

			Components.Add(new WalkerComponent(this, ResolveDirection(this, Direction, owner), 32f));
			Components.Add(new HealthComponent(1, 1, new string[] { }));

			// TODO: Add damage and death handlers
		}

		private void ChangePalette()
		{
			if (graphics == null) { return; }

			if (Palette == GoombaPalette.Overworld) { graphics.CurrentObjectName = "walking_OW"; }
			else if (Palette == GoombaPalette.Cave) { graphics.CurrentObjectName = "walking_cave"; }
			else if (Palette == GoombaPalette.Castle) { graphics.CurrentObjectName = "walking_castle"; }
			else { throw new ArgumentException($"The provided goomba palette number {(int)Palette} doesn't match any palettes."); }
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White, (!isFlippedOver) ? SpriteEffects.None : SpriteEffects.FlipVertically);
		}

		public override void Update()
		{
			base.Update();
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
			// TODO: Add interaction-with-player code here; add sprite attributes
			base.HandleSpriteCollision(sprite, resolutionDistance);
		}
	}

	public enum GoombaPalette
	{
		Overworld,
		Cave,
		Castle
	}
}
