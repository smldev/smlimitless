using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Editor;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.InternalSprites
{
	/// <summary>
	///   Enumerates the kinds of objects editors can select.
	/// </summary>
	public enum EditorSelectedObjectType
	{
		/// <summary>
		///   Nothing has been selected.
		/// </summary>
		Nothing,

		/// <summary>
		///   The editor will delete any object under the cursor on a left-click.
		/// </summary>
		Delete,

		/// <summary>
		///   A tile has been selected.
		/// </summary>
		Tile,

		/// <summary>
		///   A sprite has been selected.
		/// </summary>
		Sprite,

		/// <summary>
		/// A section exit has been selected.
		/// </summary>
		SectionExit
	}

	/// <summary>
	///   A <see cref="Sprite" /> that is used to place tiles and sprites in a section.
	/// </summary>
	public sealed class EditorSelectedObject : Sprite
	{
		private Sprite selectedSprite = null;
		private Tile selectedTile = null;
		private SectionExit selectedExit = null;

		/// <summary>
		///   Gets the category to place this sprite in the editor's object selector.
		/// </summary>
		public override string EditorCategory => "Internal Sprites";

		/// <summary>
		///   Gets a value indicating whether this sprite is a player.
		/// </summary>
		public override bool IsPlayer => false;

		/// <summary>
		///   Gets the sprite that will be placed on left-click, or throws an
		///   <see cref="InvalidOperationException" /> if a sprite is not selected.
		/// </summary>
		public Sprite SelectedSprite
		{
			get
			{
				if (SelectedObjectType == EditorSelectedObjectType.Sprite) { return selectedSprite; }
				throw new InvalidOperationException("The currently selected object is not a sprite.");
			}
		}

		/// <summary>
		///   Gets the tile that will be placed on left-click, or throws an <see
		///   cref="InvalidOperationException" /> if a tile is not selected.
		/// </summary>
		public Tile SelectedTile
		{
			get
			{
				if (SelectedObjectType == EditorSelectedObjectType.Tile) { return selectedTile; }
				throw new InvalidOperationException("The currently selected object is not a tile.");
			}
		}

		public SectionExit SelectedExit
		{
			get
			{
				if (SelectedObjectType == EditorSelectedObjectType.SectionExit) { return selectedExit; }
				throw new InvalidOperationException("The currently selected object is not a section exit.");
			}
		}

		internal EditorSelectedObjectType SelectedObjectType { get; set; } = EditorSelectedObjectType.Nothing;
		/// <summary>
		///   An event fired any time the selected object changes.
		/// </summary>
		/// <remarks>
		///   This event will fire even if the same kind of object (i.e. tile to
		///   tile) is selected.
		/// </remarks>
		public event EventHandler SelectedObjectChanged;

		/// <summary>
		///   Initializes a new instance of the <see cref="EditorSelectedObject"
		///   /> class.
		/// </summary>
		public EditorSelectedObject()
		{
			Size = new Vector2(16f);
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
		///   Draws the selected object to screen.
		/// </summary>
		public override void Draw()
		{
			if (!Owner.Owner.EditorActive) { return; }

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
				case EditorSelectedObjectType.SectionExit:
					selectedExit.DebugDraw();
					break;
				default:
					break;
			}
		}

		/// <summary>
		///   Draws a portion of this selected object to screen.
		/// </summary>
		/// <param name="cropping">The portion to draw.</param>
		public override void Draw(Rectangle cropping)
		{
			// Does cropping the editor selected object make sense?
			Draw();
			// Not right now, no.
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
		///   Selects a sprite already in a section.
		/// </summary>
		/// <param name="sprite">The sprite being selected.</param>
		public void SelectExistingSprite(Sprite sprite)
		{
			SelectedObjectType = EditorSelectedObjectType.Sprite;
			selectedSprite = AssemblyManager.GetSpriteByFullName(sprite.GetType().FullName);
			selectedSprite.Initialize(Owner);
			selectedSprite.LoadContent();
			OnSelectedObjectChanged();
		}

		/// <summary>
		///   Selects a tile already in a section.
		/// </summary>
		/// <param name="tile">The tile being selected.</param>
		public void SelectExistingTile(Tile tile)
		{
			SelectedObjectType = EditorSelectedObjectType.Tile;
			selectedTile = tile.Clone();
			selectedTile.Initialize(Owner);
			selectedTile.LoadContent();
			OnSelectedObjectChanged();
		}

		public void SelectExistingSectionExit(SectionExit exit)
		{
			SelectedObjectType = EditorSelectedObjectType.SectionExit;
			selectedExit = exit.Clone();
			OnSelectedObjectChanged();
		}

		/// <summary>
		///   Selects a sprite from the level editor.
		/// </summary>
		/// <param name="spriteData">The default properties for this sprite.</param>
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
			selectedTile = null;

			OnSelectedObjectChanged();
		}

		/// <summary>
		///   Selects a tile from the level editor.
		/// </summary>
		/// <param name="defaultState">The default properties for this tile.</param>
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
			selectedSprite = null;

			OnSelectedObjectChanged();
		}

		/// <summary>
		///   Updates this sprite.
		/// </summary>
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
			else if (selectedExit != null) { selectedExit.Position = newPosition; }

			if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
			{
				OnLeftClick();
			}
		}

		internal void UnsubscribeAllHandlers()
		{
			SelectedObjectChanged = null;
		}

		private void OnLeftClick()
		{
			Tile tileUnderCursor = Owner.GetTileAtPosition(Position);

			Func<Sprite, bool> spriteSelector = s =>
			{
				bool within = s.Hitbox.Within(Hitbox.Center, adjacentPointsAreWithin: true);
				bool isNotInternal = !s.GetType().Namespace.Contains("Internal");
				return within && isNotInternal;
			};
			Sprite spriteUnderCursor = Owner.SpritesGrid.FirstOrDefault(spriteSelector);

			SectionExit exitUnderCursor = Owner.SectionExits.FirstOrDefault(e => 
				e.Hitbox.IntersectsIncludingEdges(Position));

			switch (SelectedObjectType)
			{
				case EditorSelectedObjectType.Nothing:
					if (exitUnderCursor != null)
					{
						SelectExistingSectionExit(exitUnderCursor);
						Owner.SectionExits.Remove(exitUnderCursor);
					}
					else if (tileUnderCursor != null)
					{
						SelectExistingTile(tileUnderCursor);
					}
					else if (spriteUnderCursor != null && !(spriteUnderCursor is EditorSelectedObject) && !(spriteUnderCursor is EditorCameraTrackingObject))
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
					tile.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(selectedTile.GetCustomSerializableObjects())));
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
				case EditorSelectedObjectType.SectionExit:
					if (exitUnderCursor != null) { return; }
					SectionExit exit = selectedExit.Clone();
					Owner.SectionExits.Add(exit);
					break;
				default:
					break;
			}
		}

		private void OnSelectedObjectChanged()
		{
			SelectedObjectChanged?.Invoke(this, new EventArgs());
		}
	}
}
