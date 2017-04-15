using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Players
{
	public sealed class PlayerMarioSuper : PlayerMario
	{
		private const int TextureWidth = 29;

		private ComplexGraphicsObject graphics;

		public PlayerMarioSuper() : base()
		{
			Size = new Vector2(16f, 32f);
		}

		public override void Initialize(Section owner)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3PlayerMarioSuper");
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void Draw()
		{
			Vector2 drawPosition = new Vector2((int)(Position.X - ((TextureWidth / 2f) - (Hitbox.Width / 2f))), Position.Y);
			graphics.Draw(drawPosition.Floor(), Color.White,
			(FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		public override void Draw(Rectangle cropping)
		{
			graphics.Draw(Position.Floor(), cropping, Color.White,
			(FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		protected override void BaseUpdate()
		{
			base.BaseUpdate();
			graphics.Update();
		}

		protected override void SetPlayerGraphicsObject(string objectName)
		{
			graphics.CurrentObjectName = objectName;
		}
	}
}
