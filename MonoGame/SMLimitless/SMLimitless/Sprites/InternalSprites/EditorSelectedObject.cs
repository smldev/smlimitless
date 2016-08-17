using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Editor;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	public sealed class EditorSelectedObject : Sprite
	{
		internal EditorSelectedObjectType SelectedObjectType{ get; set; } = EditorSelectedObjectType.Nothing;
		private Tile selectedTile = null;
		private Sprite selectedSprite = null;

		public Tile SelectedTile
		{
			get
			{
				if (SelectedObjectType == EditorSelectedObjectType.Tile) { return selectedTile; }
				throw new InvalidOperationException("The currently selected object is not a tile.");
			}
		}

		public Sprite SelectedSprite
		{
			get
			{
				if (SelectedObjectType == EditorSelectedObjectType.Sprite) { return selectedSprite; }
				throw new InvalidOperationException("The currently selected object is not a sprite.");
			}
		}

		public event EventHandler SelectedObjectChanged;

		public override string EditorCategory => "Internal Sprites";
		public override bool IsPlayer => false;

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public EditorSelectedObject()
		{
			Size = new Vector2(16f);
		}

		public override void Draw()
		{
			switch (SelectedObjectType)
			{
				case EditorSelectedObjectType.Nothing:
				case EditorSelectedObjectType.Delete:
					return;
				case EditorSelectedObjectType.Tile:
					selectedTile.Draw();
					break;
				case EditorSelectedObjectType.Sprite:
					selectedSprite.Draw();
					break;
				default:
					break;
			}
		}

		public override void Draw(Vector2 cropping)
		{
			// Does cropping the editor selected object make sense?
			Draw();
			// Not right now, no.
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
			Vector2 newPosition = Vector2.Zero;

			if (mousePosition.IsNaN()) { mousePosition = Position; }

			newPosition.X = (float)Math.Floor(mousePosition.X / 16f) * 16f;
			newPosition.Y = (float)Math.Floor(mousePosition.Y / 16f) * 16f;

			Position = newPosition;
			if (selectedTile != null) { selectedTile.Position = newPosition; }
			else if (selectedSprite != null) { selectedSprite.Position = newPosition; }

			if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
			{
				OnLeftClick();
			}
		}

		internal void UnsubscribeAllHandlers()
		{
			SelectedObjectChanged = null;
		}

		public void SelectExistingTile(Tile tile)
		{
			SelectedObjectType = EditorSelectedObjectType.Tile;
			selectedTile = tile.Clone();
			selectedTile.Initialize(Owner);
			selectedTile.LoadContent();
			OnSelectedObjectChanged();
		}

		public void SelectExistingSprite(Sprite sprite)
		{
			SelectedObjectType = EditorSelectedObjectType.Sprite;
			selectedSprite = AssemblyManager.GetSpriteByFullName(sprite.GetType().FullName);
			selectedSprite.Initialize(Owner);
			selectedSprite.LoadContent();
			OnSelectedObjectChanged();
		}

		public void SelectTileFromEditor(TileDefaultState defaultState)
		{
			Tile tile = AssemblyManager.GetTileByFullName(defaultState.TypeName);

			tile.SolidSides = defaultState.SolidSides;
			tile.GraphicsResourceName = defaultState.GraphicsResource;
			tile.State = tile.InitialState = defaultState.State;
			tile.DeserializeCustomObjects(new JsonHelper((JToken)defaultState.CustomData));

			tile.Initialize(Owner);
			tile.LoadContent();
			SelectedObjectType = EditorSelectedObjectType.Tile;
			selectedTile = tile;

			OnSelectedObjectChanged();
		}

		public void SelectSpriteFromEditor(SpriteData spriteData)
		{
			Sprite sprite = AssemblyManager.GetSpriteByFullName(spriteData.TypeName);

			sprite.State = sprite.InitialState = (SpriteState)spriteData.State;
			sprite.TileCollisionMode = (SpriteCollisionMode)spriteData.Collision;
			sprite.DeserializeCustomObjects(new JsonHelper(spriteData.CustomData));

			sprite.Initialize(Owner);
			sprite.LoadContent();
			SelectedObjectType = EditorSelectedObjectType.Sprite;
			selectedSprite = sprite;

			OnSelectedObjectChanged();
		}

		private void OnLeftClick()
		{
			Tile tileUnderCursor = Owner.GetTileAtPosition(Position);
			Sprite spriteUnderCursor = Owner.SpritesGrid.FirstOrDefault(s => s.Hitbox.Within(Hitbox.Center, adjacentPointsAreWithin: true));
			switch (SelectedObjectType)
			{
				case EditorSelectedObjectType.Nothing:
					if (tileUnderCursor != null)
					{
						SelectExistingTile(tileUnderCursor);
					}
					else if (spriteUnderCursor != null)
					{
						SelectExistingSprite(spriteUnderCursor);
					}
					break;
				case EditorSelectedObjectType.Delete:
					if (tileUnderCursor != null) { Owner.RemoveTile(tileUnderCursor); }
					else if (spriteUnderCursor != null) { spriteUnderCursor.RemoveOnNextFrame = true; } // yay consistency
					break;
				case EditorSelectedObjectType.Tile:
					Tile tile = selectedTile.Clone();
					tile.Position = Position;
					tile.Initialize(Owner);
					tile.LoadContent();
					Owner.AddTile(tile);
					break;
				case EditorSelectedObjectType.Sprite:
					if (spriteUnderCursor == null && tileUnderCursor == null) // only add a sprite if the LMB click is new
					{
						Sprite sprite = AssemblyManager.GetSpriteByFullName(selectedSprite.GetType().FullName);
						sprite.TileCollisionMode = selectedSprite.TileCollisionMode;
						sprite.State = sprite.InitialState = selectedSprite.InitialState;
						sprite.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(selectedSprite.GetCustomSerializableObjects())));
						sprite.Initialize(Owner);
						sprite.LoadContent();
						sprite.Position = Position;
						sprite.InitialPosition = Position;
						Owner.AddSpriteOnNextFrame(sprite);
					}
					else if (spriteUnderCursor != null && tileUnderCursor == null) // otherwise see if we can drop this sprite inside this object
					{
						Sprite sprite = AssemblyManager.GetSpriteByFullName(selectedSprite.GetType().FullName);
						sprite.TileCollisionMode = selectedSprite.TileCollisionMode;
						sprite.State = sprite.InitialState = selectedSprite.InitialState;
						sprite.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(selectedSprite.GetCustomSerializableObjects())));
						spriteUnderCursor.OnEditorDrop(sprite);
					}
					else if (tileUnderCursor != null && spriteUnderCursor == null)
					{
						Sprite sprite = AssemblyManager.GetSpriteByFullName(selectedSprite.GetType().FullName);
						sprite.TileCollisionMode = selectedSprite.TileCollisionMode;
						sprite.State = sprite.InitialState = selectedSprite.InitialState;
						sprite.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(selectedSprite.GetCustomSerializableObjects())));
						tileUnderCursor.OnEditorDrop(sprite);
					}
					break;
				default:
					break;
			}
		}

		private void OnSelectedObjectChanged()
		{
			if (SelectedObjectChanged != null)
			{
				SelectedObjectChanged(this, new EventArgs());
			}
		}
	}

	public enum EditorSelectedObjectType
	{
		Nothing,
		Delete,
		Tile,
		Sprite
	}
}
