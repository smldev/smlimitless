using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	public sealed class CollisionDebugSelectSprite : Sprite
	{
		public override string EditorCategory => "Internal Sprites";
		public override bool IsPlayer => false;

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public CollisionDebugSelectSprite()
		{
			Size = new Vector2(16f);
			TileCollisionMode = SpriteCollisionMode.NoCollision;
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
			Vector2 mousePosition = Owner.MousePosition;
			if (!mousePosition.IsNaN()) { Position = mousePosition; }
			
			if (InputManager.IsNewMousePress(MouseButtons.LeftButton) && (Owner.CollisionDebugSelectedSprite == null || Owner.CollisionDebugSelectedSprite == this))
			{
				Sprite sprite = Owner.SpritesGrid.FirstOrDefault(s => s.Hitbox.Within(Position, true));
				if (sprite != null) { Owner.CollisionDebugSelectSprite(sprite); }
			}

			if (InputManager.IsNewMousePress(MouseButtons.RightButton))
			{
				Sprite sprite = Owner.SpritesGrid.FirstOrDefault(s => s.Hitbox.Within(Position, true));
				if (sprite != null && !sprite.GetType().FullName.Contains("Debug")) { sprite.BreakOnCollision = !sprite.BreakOnCollision; }
				else
				{
					Tile tile = Owner.GetTileAtPosition(Position);
					if (tile != null) { tile.BreakOnCollision = !tile.BreakOnCollision; }
				}
			}

			base.Update();
		}
	}
}
