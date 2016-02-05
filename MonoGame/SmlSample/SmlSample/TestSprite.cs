using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSample
{
	public sealed class TestSprite : Sprite
	{
		private AnimatedGraphicsObject graphics;	

		public override string EditorCategory
		{
			get
			{
				return "Test Items";
			}
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
			graphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("smb3_goomba");

			Size = new Vector2(16f);
			Components.Add(new WalkerComponent(this, SpriteDirection.Left, 32f));
			Components.Add(new HealthComponent(1, new string[] { }));

			((HealthComponent)Components[1]).SpriteDeath += (sender, e) => { RemoveOnNextFrame = true; };
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White);
		}

		public override void Update()
		{
			base.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite is PainterSprite) return;

			if ((resolutionDistance.GetIntersectionDirection() & SMLimitless.Physics.FlaggedDirection.Up) == SMLimitless.Physics.FlaggedDirection.Up && sprite.Velocity.Y > 0f)
			{
				((HealthComponent)Components[1]).Damage(1, "playerStomp");
			}
			else
			{
				((HealthComponent)sprite.Components[0]).Damage(1, "contact");
			}
		}
	}
}
