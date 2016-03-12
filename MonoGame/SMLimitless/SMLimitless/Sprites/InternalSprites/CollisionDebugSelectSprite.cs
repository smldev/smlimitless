using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	public sealed class CollisionDebugSelectSprite : Sprite
	{
		public override string EditorCategory
		{
			get
			{
				return "Internal Sprite";
			}
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public CollisionDebugSelectSprite()
		{
			Size = new Vector2(16f);
			CollisionMode = SpriteCollisionMode.NoCollision;
		}

		public override void Draw()
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
		}

		public override void Update()
		{
			Position = InputManager.MousePosition;
			
			if (InputManager.IsNewMousePress(MouseButtons.LeftButton) && (Owner.CollisionDebugSelectedSprite == null || Owner.CollisionDebugSelectedSprite == this))
			{
				Sprite sprite = Owner.Sprites.FirstOrDefault(s => s.Hitbox.Within(InputManager.MousePosition, true));
				if (sprite != null) { Owner.CollisionDebugSelectSprite(sprite); }
			}

			base.Update();
		}
	}
}
