using System;
using System.Collections.Generic;
using System.Drawing;
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
	/// <summary>
	///   A form that can be used to select, place, and modify game objects in levels.
	/// </summary>
	public partial class EditorForm : Form
	{
		private Dictionary<int, SpriteData> buttonSpriteDataMapping = new Dictionary<int, SpriteData>();
		private Dictionary<int, TileDefaultState> buttonTileDataMapping = new Dictionary<int, TileDefaultState>();
		private Level level;
		private PropertyForm propertyForm;
		private Section section;
		private EditorSelectedObject selectedObject;
		private Section selectedSection;

		/// <summary>
		///   Gets the state of the level editor.
		/// </summary>
		public EditorState EditorState { get; private set; } = EditorState.Cursor;

		/// <summary>
		///   Initializes a new instance of the <see cref="EditorForm" /> class.
		/// </summary>
		/// <param name="level">The level being edited.</param>
		/// <param name="section">The section being edited.</param>
		/// <param name="selectedObject">
		///   An <see cref="EditorSelectedObject" /> instance in the section.
		/// </param>
		public EditorForm(Level level, Section section, EditorSelectedObject selectedObject)
		{
			InitializeComponent();

			this.level = level;
			this.section = section;
			this.selectedObject = selectedObject;

			PropertyLevel.SelectedObject = level;
			PropertySection.SelectedObject = section;

			propertyForm = new PropertyForm(GetSelectedObject(selectedObject));
			selectedObject.SelectedObjectChanged += (sender, e) =>
			{
				propertyForm.DisplayedObject = GetSelectedObject(selectedObject);
			};

			PopulateObjectTabs();
			LoadSections();
		}

		private void LoadSections()
		{
			foreach (Section section in level.Sections)
			{
				var item = new ListViewItem(new string[] { section.Index.ToString(), section.Name.ToString() });
				if (section.IsStartSection) { item.Font = new Font(item.Font, FontStyle.Bold); }
				ListSections.Items.Add(item);
			}
		}

		private void ButtonCursor_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Nothing;
		}

		private void ButtonDelete_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Delete;
		}

		private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			propertyForm.Close();
			propertyForm.Dispose();
			propertyForm = null;

			selectedObject.UnsubscribeAllHandlers();
		}

		private void EditorForm_Load(object sender, EventArgs e)
		{
			propertyForm.Show();
		}

		private object GetSelectedObject(EditorSelectedObject selectedObject)
		{
			switch (selectedObject.SelectedObjectType)
			{
				case EditorSelectedObjectType.Nothing:
				case EditorSelectedObjectType.Delete:
					return new object();
				case EditorSelectedObjectType.Tile:
					return selectedObject.SelectedTile;
				case EditorSelectedObjectType.Sprite:
					return selectedObject.SelectedSprite;
				default:
					throw new InvalidOperationException();
			}
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

		private void ListSections_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ListSections.SelectedItems.Count > 0)
			{
				var selectedItem = ListSections.SelectedItems[0];
				Section section = level.GetSectionByIndex(int.Parse(selectedItem.Text));
				TextSectionName.Text = section.Name;
				SetEnabledOnButtons(true, ButtonUpdateName, ButtonSwitchTo, ButtonSetAsStart, ButtonRemoveSection);
				selectedSection = section;
			}
			else
			{
				SetEnabledOnButtons(false, ButtonUpdateName, ButtonSwitchTo, ButtonSetAsStart, ButtonRemoveSection);
				selectedSection = null;
			}
		}

		private void SetEnabledOnButtons(bool enabled, params Button[] buttons)
		{
			foreach (var button in buttons)
			{
				button.Enabled = enabled;
			}
		}

		private void ButtonUpdateName_Click(object sender, EventArgs e)
		{
			Section sectionBeingUpdated = selectedSection;
			sectionBeingUpdated.Name = TextSectionName.Text;
			var oldItem = ListSections.FindItemWithText(sectionBeingUpdated.Index.ToString());
			int sectionIndexInListBox = ListSections.Items.IndexOf(oldItem);
			ListSections.Items.RemoveAt(sectionIndexInListBox);

			ListViewItem newItem = new ListViewItem(new string[] { sectionBeingUpdated.Index.ToString(), sectionBeingUpdated.Name });
			ListSections.Items.Insert(sectionIndexInListBox, newItem);
			newItem.Selected = true;
		}

		private void ButtonSwitchTo_Click(object sender, EventArgs e)
		{
			level.TransferEditorControlToSection(level.ActiveSection, selectedSection);
		}

		internal void SwitchToSection(Section newSection)
		{
			section = newSection;
			PropertySection.SelectedObject = newSection;
		}
	}
}
