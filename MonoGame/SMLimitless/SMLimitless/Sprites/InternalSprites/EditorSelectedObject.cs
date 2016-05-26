﻿using System;
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

		public override string EditorCategory
		{
			get
			{
				return "Internal Sprites";
			}
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
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
			selectedTile.Initialize(Owner);
			selectedTile.LoadContent();
			OnSelectedObjectChanged();
		}

		public void SelectTileFromEditor(TileDefaultState defaultState)
		{
			Tile tile = AssemblyManager.GetTileByFullName(defaultState.TypeName);

			tile.SolidSides = defaultState.SolidSides;
			tile.GraphicsResourceName = defaultState.GraphicsResource;
			tile.State = tile.InitialState = defaultState.State;
			tile.DeserializeCustomObjects(new JsonHelper((JToken)defaultState.CustomData));

			tile.LoadContent();
			SelectedObjectType = EditorSelectedObjectType.Tile;
			selectedTile = tile;

			OnSelectedObjectChanged();
		}

		public void SelectSpriteFromEditor(SpriteData spriteData)
		{
			Sprite sprite = AssemblyManager.GetSpriteByFullName(spriteData.TypeName);

			sprite.State = sprite.InitialState = (SpriteState)spriteData.State;
			sprite.CollisionMode = (SpriteCollisionMode)spriteData.Collision;
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
			Sprite spriteUnderCursor = Owner.Sprites.FirstOrDefault(s => s.Hitbox.Within(Position, true));
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
					if (spriteUnderCursor == null)	// only add a sprite if the LMB click is new
					{
						Sprite sprite = AssemblyManager.GetSpriteByFullName(selectedSprite.GetType().FullName);
						sprite.CollisionMode = selectedSprite.CollisionMode;
						sprite.State = sprite.InitialState = selectedSprite.InitialState;
						sprite.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(selectedSprite.GetCustomSerializableObjects())));
						sprite.Initialize(Owner);
						sprite.LoadContent();
						sprite.Position = Position;
						Owner.AddSpriteOnNextFrame(sprite);
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
