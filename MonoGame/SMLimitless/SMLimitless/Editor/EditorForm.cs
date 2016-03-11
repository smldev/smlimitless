using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.InternalSprites;

namespace SMLimitless.Editor
{
	public partial class EditorForm : Form
	{
		private Level level;
		private Section section;

		private Dictionary<int, TileDefaultState> buttonTileDataMapping = new Dictionary<int, TileDefaultState>();
		private Dictionary<int, SpriteData> buttonSpriteDataMapping = new Dictionary<int, SpriteData>();

		public EditorState EditorState { get; private set; } = EditorState.Cursor;
		private EditorSelectedObject selectedObject;

		public EditorForm(Level level, Section section, EditorSelectedObject selectedObject)
		{
			InitializeComponent();

			this.level = level;
			this.section = section;
			this.selectedObject = selectedObject;

			PropertyLevel.SelectedObject = level;
			PropertySection.SelectedObject = section;

			PopulateObjectTabs();
		}

		private void PopulateObjectTabs()
		{
			const int DefaultControlPadding = 4;
			const int DefaultButtonSize = 24;

			var objectDataLists = AssemblyManager.GetAllObjectData();
			int tileButtonX = DefaultControlPadding;
			int tileButtonY = DefaultControlPadding;
			int tileButtonIndex = 0;
			int spriteButtonIndex = 0;
			int spriteButtonX = DefaultControlPadding;
			int spriteButtonY = DefaultControlPadding;

			foreach (var objectData in objectDataLists)
			{
				foreach (var tileObject in objectData.TileData)
				{
					foreach (var defaultState in tileObject.DefaultStates)
					{
						Button button = new Button();
						button.Size = new Size(DefaultButtonSize, DefaultButtonSize);
						button.Location = new Point(tileButtonX, tileButtonY);
						button.TabIndex = tileButtonIndex;

						IGraphicsObject graphicsObject = ContentPackageManager.GetGraphicsResource(defaultState.GraphicsResource);
						graphicsObject.LoadContent();
						Texture2D editorGraphic = graphicsObject.GetEditorGraphics();
						Image editorGraphicImage = editorGraphic.ToImage();
						button.Click += (sender, e) => { selectedObject.SelectTileFromEditor(defaultState); };
						button.Image = editorGraphicImage;

						PanelTiles.Controls.Add(button);
						buttonTileDataMapping.Add(button.TabIndex, defaultState);

						if (tileButtonX + DefaultControlPadding + DefaultButtonSize > PanelTiles.Size.Width)
						{
							tileButtonX = DefaultControlPadding;
							tileButtonY += DefaultControlPadding + DefaultButtonSize;
						}
						else
						{
							tileButtonX += DefaultControlPadding + DefaultButtonSize;
						}
						tileButtonIndex++;
					}
				}

				foreach (var spriteObject in objectData.SpriteData)
				{
					Button button = new Button();
					button.Size = new Size(DefaultButtonSize, DefaultButtonSize);
					button.Location = new Point(spriteButtonX, spriteButtonY);
					button.TabIndex = spriteButtonIndex++;

					IGraphicsObject graphicsObject = ContentPackageManager.GetGraphicsResource(spriteObject.EditorResourceName);
					graphicsObject.LoadContent();
					Texture2D editorGraphic = graphicsObject.GetEditorGraphics();
					Image editorGraphicImage = editorGraphic.ToImage();
					button.Click += (sender, e) => { selectedObject.SelectSpriteFromEditor(spriteObject); };
					button.Image = editorGraphicImage;

					PanelSprites.Controls.Add(button);
					buttonSpriteDataMapping.Add(button.TabIndex, spriteObject);

					if (spriteButtonX + DefaultControlPadding + DefaultButtonSize > PanelSprites.Size.Width)
					{
						spriteButtonX = DefaultControlPadding;
						spriteButtonY += DefaultControlPadding + DefaultButtonSize;
					}
					else
					{
						spriteButtonX += DefaultButtonSize + DefaultControlPadding;
					}
				}
			}
		}

		private void ButtonDelete_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Delete;
		}

		private void ButtonCursor_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Nothing;
		}
	}

	public enum EditorState
	{
		ObjectSelected,
		Cursor,
		Delete
	}
}
