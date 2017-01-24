﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.IO;
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
		private const int DefaultButtonSize = 24;
		private const int DefaultControlPadding = 4;
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

			DynamicPropertyControlGenerator.GenerateControls(PanelLevelSettings, level);
			DynamicPropertyControlGenerator.GenerateControls(PanelSectionSettings, section);

			propertyForm = new PropertyForm(GetSelectedObject(selectedObject));
			selectedObject.SelectedObjectChanged += (sender, e) =>
			{
				propertyForm.DisplayedObject = GetSelectedObject(selectedObject);
			};

			PopulateObjectTabs();
			LoadSections();
		}

		internal void SwitchToSection(Section newSection)
		{
			section = newSection;
			DynamicPropertyControlGenerator.GenerateControls(PanelSectionSettings, newSection);
		}

		private void ButtonCursor_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Nothing;
		}

		private void ButtonDelete_Click(object sender, EventArgs e)
		{
			selectedObject.SelectedObjectType = EditorSelectedObjectType.Delete;
		}

		private void ButtonSetAsStart_Click(object sender, EventArgs e)
		{
			if (selectedSection != null)
			{
				var selectedItem = ListSections.SelectedItems[0];
				var oldStartSection = level.Sections.First(s => s.IsStartSection);
				var oldStartSectionItem = GetItemBySection(oldStartSection);

				selectedSection.IsStartSection = true;
				oldStartSection.IsStartSection = false;

				selectedItem.Font = new Font(selectedItem.Font, FontStyle.Bold);
				oldStartSectionItem.Font = new Font(oldStartSectionItem.Font, FontStyle.Regular);
			}
		}

		private void ButtonSwitchTo_Click(object sender, EventArgs e)
		{
			level.TransferEditorControlToSection(level.ActiveSection, selectedSection);
		}

		private void ButtonUpdate_Click(object sender, EventArgs e)
		{
		}

		private void ButtonUpdateName_Click(object sender, EventArgs e)
		{
			Section sectionBeingUpdated = selectedSection;
			sectionBeingUpdated.Name = TextSectionName.Text;
			var oldItem = ListSections.FindItemWithText(sectionBeingUpdated.Index.ToString());
			int sectionIndexInListBox = ListSections.Items.IndexOf(oldItem);
			ListSections.Items.RemoveAt(sectionIndexInListBox);

			ListViewItem newItem = new ListViewItem(new string[]
			{
				sectionBeingUpdated.Index.ToString(),
				sectionBeingUpdated.Name
			});
			ListSections.Items.Insert(sectionIndexInListBox, newItem);
			newItem.Selected = true;
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

		private void GenerateSpriteButtons(EditorObjectData objectData)
		{
			int spriteButtonIndex = 0;
			int spriteButtonX = DefaultControlPadding;
			int spriteButtonY = DefaultControlPadding;

			foreach (var spriteObject in objectData.SpriteData)
			{
				Button button = new Button();
				button.Size = new Size(DefaultButtonSize, DefaultButtonSize);
				button.Location = new Point(spriteButtonX, spriteButtonY);
				button.TabIndex = spriteButtonIndex++;

				IGraphicsObject graphicsObject =
					ContentPackageManager.GetGraphicsResource(spriteObject.EditorResourceName);
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

		private void GenerateTileButtons(EditorObjectData objectData)
		{
			int tileButtonX = DefaultControlPadding;
			int tileButtonY = DefaultControlPadding;
			int tileButtonIndex = 0;

			foreach (var tileObject in objectData.TileData)
			{
				foreach (var defaultState in tileObject.DefaultStates)
				{
					Button button = new Button();
					button.Size = new Size(DefaultButtonSize, DefaultButtonSize);
					button.Location = new Point(tileButtonX, tileButtonY);
					button.TabIndex = tileButtonIndex;

					IGraphicsObject graphicsObject =
						ContentPackageManager.GetGraphicsResource(defaultState.GraphicsResource);
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
		}

		private ListViewItem GetItemBySection(Section section)
		{
			string index = section.Index.ToString();
			for (int i = 0; i < ListSections.Items.Count; i++)
			{
				var item = ListSections.Items[i];
				if (item.Text == index) { return item; }
			}

			throw new ArgumentException($"Couldn't find section with index {index} in the list.");
		}

		private object GetSelectedObject(EditorSelectedObject selectedObject)
		{
			switch (selectedObject.SelectedObjectType)
			{
				case EditorSelectedObjectType.Nothing:
				case EditorSelectedObjectType.Delete:
					return null;
				case EditorSelectedObjectType.Tile:
					return selectedObject.SelectedTile;
				case EditorSelectedObjectType.Sprite:
					return selectedObject.SelectedSprite;
				default:
					throw new InvalidOperationException();
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

		private void LoadSections()
		{
			foreach (Section section in level.Sections)
			{
				var item = new ListViewItem(new[] { section.Index.ToString(), section.Name.ToString() });
				if (section.IsStartSection) { item.Font = new Font(item.Font, FontStyle.Bold); }
				ListSections.Items.Add(item);
			}
		}

		private void PopulateObjectTabs()
		{
			var objectDataLists = AssemblyManager.GetAllObjectData();

			foreach (var objectData in objectDataLists)
			{
				GenerateTileButtons(objectData);
				GenerateSpriteButtons(objectData);
			}
		}

		private void SetEnabledOnButtons(bool enabled, params Button[] buttons)
		{
			foreach (var button in buttons)
			{
				button.Enabled = enabled;
			}
		}
	}
}
