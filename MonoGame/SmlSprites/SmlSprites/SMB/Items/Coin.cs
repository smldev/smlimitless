using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Sounds;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.SMB.Items
{
	public sealed class Coin : Sprite
	{
		private AnimatedGraphicsObject graphics;
		private CachedSound collectSound;

		public override string EditorCategory => "Items";

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			
		}

		public Coin()
		{
			Size = new Vector2(16f);
		}

		public override void Initialize(Section owner)
		{
			graphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBCoin");
			IsMoving = false;
			collectSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiCoin"));

			base.Initialize(owner);
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White);
		}

		public override void Draw(Vector2 cropping)
		{
			graphics.Draw(Position, cropping, Color.White, Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
		}

		public override object GetCustomSerializableObjects()
		{
			return new { };
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void Update()
		{
			graphics.Update();

			if (IsMoving) { base.Update(); }
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer)
			{
				Owner.HUDInfo.AddCoins(1);
				Owner.RemoveSpriteOnNextFrame(this);
				AudioPlaybackEngine.Instance.PlaySound(collectSound, (sender, e) => { });
			}
			base.HandleSpriteCollision(sprite, resolutionDistance);
		}
	}
}
