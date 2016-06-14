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

		/// <summary>
		/// Gets the state of the level editor.
		/// </summary>
		public EditorState EditorState { get; private set; } = EditorState.Cursor;
		private EditorSelectedObject selectedObject;

		private PropertyForm propertyForm;

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorForm"/> class.
		/// </summary>
		/// <param name="level">The level being edited.</param>
		/// <param name="section">The section being edited.</param>
		/// <param name="selectedObject">An <see cref="EditorSelectedObject"/> instance in the section.</param>
		public EditorForm(Level level, Section section, EditorSelectedObject selectedObject)
		{
			InitializeComponent();

			this.level = level;
			this.section = section;
			this.selectedObject = selectedObject;

			PropertyLevel.SelectedObject = level;
			PropertySection.SelectedObject = section;

			propertyForm = new PropertyForm(selectedObject);

			PopulateObjectTabs();
		}

		private void PopulateObjectTabs()
		{
			const int DefaultControlPadding = 4;
			const int DefaultButtonSize = 24;

			// Set up some local fields.
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

		private void EditorForm_Load(object sender, EventArgs e)
		{
			propertyForm.Show();
		}

		private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			propertyForm.Close();
			propertyForm.Dispose();
			propertyForm = null;
		}
	}

	/// <summary>
	/// An enumeration of states that the level editor can be in.
	/// </summary>
	public enum EditorState
	{
		/// <summary>
		/// An object is selected and can be placed in the section.
		/// </summary>
		ObjectSelected,

		/// <summary>
		/// No object is selected, but an object can be selected by clicking it.
		/// </summary>
		Cursor,

		/// <summary>
		/// Any object clicked on will be removed from the section.
		/// </summary>
		Delete
	}
}
