using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	/// <summary>
	///   A sprite that can select another sprite for collision debugging.
	/// </summary>
	public sealed class CollisionDebugSelectSprite : Sprite
	{
		/// <summary>
		///   Gets the category to place this sprite in the editor's object selector.
		/// </summary>
		public override string EditorCategory => "Internal Sprites";

		/// <summary>
		///   Gets a value indicating whether this sprite is a player.
		/// </summary>
		public override bool IsPlayer => false;

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="CollisionDebugSelectSprite" /> class.
		/// </summary>
		public CollisionDebugSelectSprite()
		{
			Size = new Vector2(16f);
			TileCollisionMode = SpriteCollisionMode.NoCollision;
		}

		/// <summary>
		///   Deserializes any objects that custom sprites have written to the
		///   level file.
		/// </summary>
		/// <param name="customObjects">
		///   An object containing the objects of the custom sprites.
		/// </param>
		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		/// <summary>
		///   This sprite has no graphics.
		/// </summary>
		public override void Draw()
		{
		}

		/// <summary>
		///   This sprite has no graphics.
		/// </summary>
		/// <param name="cropping">Not applicable.</param>
		public override void Draw(Rectangle cropping)
		{
		}

		/// <summary>
		///   Gets an anonymous object containing objects that need to be saved
		///   to the level file.
		/// </summary>
		/// <returns></returns>
		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		/// <summary>
		///   Loads the content for this sprite.
		/// </summary>
		public override void LoadContent()
		{
		}

		/// <summary>
		///   Updates this sprite.
		/// </summary>
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
