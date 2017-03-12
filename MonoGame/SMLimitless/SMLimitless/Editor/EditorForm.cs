using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
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

                if (selectedObject.SelectedObjectType == EditorSelectedObjectType.SectionExit)
                {
                    LoadSectionExitInformation(selectedObject.SelectedExit);
                }
			};

			PopulateObjectTabs();
			LoadSections();
		}

        private void LoadSectionExitInformation(SectionExit exit)
        {
            var otherExit = level.GetSectionExitByID(exit.OtherID);

            var source = (exit.SourceBehavior == ExitSourceBehavior.NotASource) ? otherExit : exit;
            var destination = (exit == source) ? otherExit : exit;

            Section sourceSection = level.GetSectionWithExit(source);
            Section destinationSection = level.GetSectionWithExit(destination);

            TextSourceSectionID.Text = sourceSection.Index.ToString();
            TextDestinationSectionID.Text = destinationSection.Index.ToString();
            LabelSourceSectionName.Text = sourceSection.Name;
            LabelDestinationSectionName.Text = destinationSection.Name;

            TextSourceX.Text = ((int)source.Position.X).ToString();
            TextSourceY.Text = ((int)source.Position.Y).ToString();
            TextDestinationX.Text = ((int)destination.Position.X).ToString();
            TextDestinationY.Text = ((int)destination.Position.Y).ToString();

            switch (source.SourceBehavior)
            {
                case ExitSourceBehavior.PipeDown:
                    RadioSourcePipeDown.Enabled = true; break;
                case ExitSourceBehavior.PipeUp:
                    RadioSourcePipeUp.Enabled = true; break;
                case ExitSourceBehavior.PipeLeft:
                    RadioSourcePipeLeft.Enabled = true; break;
                case ExitSourceBehavior.PipeRight:
                    RadioSourcePipeRight.Enabled = true; break;
                case ExitSourceBehavior.Door:
                    RadioSourceDoor.Enabled = true; break;
                case ExitSourceBehavior.Immediate:
                    throw new NotImplementedException();
                case ExitSourceBehavior.Default:
                case ExitSourceBehavior.NotASource:
                default:
                    throw new InvalidOperationException($"Invalid source behavior {source.SourceBehavior}");
            }

            switch (destination.DestinationBehavior)
            {
                case ExitDestinationBehavior.PipeUp:
                    RadioDestinationPipeUp.Enabled = true; break;
                case ExitDestinationBehavior.PipeDown:
                    RadioDestinationPipeDown.Enabled = true; break;
                case ExitDestinationBehavior.PipeRight:
                    RadioDestinationPipeRight.Enabled = true; break;
                case ExitDestinationBehavior.PipeLeft:
                    RadioDestinationPipeLeft.Enabled = true; break;
                case ExitDestinationBehavior.None:
                    RadioDestinationDoor.Enabled = true; break;
                case ExitDestinationBehavior.Default:
                case ExitDestinationBehavior.NotADestination:
                default:
                    throw new InvalidOperationException($"Invalid destination behavior {destination.DestinationBehavior}");
            }
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

				selectedItem.Font = new System.Drawing.Font(selectedItem.Font, System.Drawing.FontStyle.Bold);
				oldStartSectionItem.Font = new System.Drawing.Font(oldStartSectionItem.Font, System.Drawing.FontStyle.Regular);
			}
		}

		private void ButtonSwitchTo_Click(object sender, EventArgs e)
		{
			level.TransferEditorControlToSection(level.ActiveSection, selectedSection);
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
				button.Size = new System.Drawing.Size(DefaultButtonSize, DefaultButtonSize);
				button.Location = new System.Drawing.Point(spriteButtonX, spriteButtonY);
				button.TabIndex = spriteButtonIndex++;

				IGraphicsObject graphicsObject =
					ContentPackageManager.GetGraphicsResource(spriteObject.EditorResourceName);
				graphicsObject.LoadContent();
				Texture2D editorGraphic = graphicsObject.GetEditorGraphics();
                System.Drawing.Image editorGraphicImage = editorGraphic.ToImage();
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
					button.Size = new System.Drawing.Size(DefaultButtonSize, DefaultButtonSize);
					button.Location = new System.Drawing.Point(tileButtonX, tileButtonY);
					button.TabIndex = tileButtonIndex;

					IGraphicsObject graphicsObject =
						ContentPackageManager.GetGraphicsResource(defaultState.GraphicsResource);
					graphicsObject.LoadContent();
					Texture2D editorGraphic = graphicsObject.GetEditorGraphics();
                    System.Drawing.Image editorGraphicImage = editorGraphic.ToImage();
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
                case EditorSelectedObjectType.SectionExit:
                    return selectedObject.SelectedExit;
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
				if (section.IsStartSection) { item.Font = new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Bold); }
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

		private void ButtonNew_Click(object sender, EventArgs e)
		{
            // Create a new exit pair.
            SectionExitFormData exitData = new SectionExitFormData(this, level);

            if (!exitData.ParseSucceded) { return; }

            if (!level.HasSection(exitData.SourceSectionID))
            {
                MessageBox.Show($"There is no section with the ID {exitData.SourceSectionID}.",
                    "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!level.HasSection(exitData.DestinationSectionID))
            {
                MessageBox.Show($"There is no section with the ID {exitData.DestinationSectionID}.",
                    "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LabelSourceSectionName.Text = $"Name: {level.GetSectionByIndex(exitData.SourceSectionID).Name}";
            LabelDestinationSectionName.Text = $"Name: {level.GetSectionByIndex(exitData.DestinationSectionID).Name}";

			SectionExit source = new SectionExit(level.GetSectionByIndex(exitData.SourceSectionID));
			source.ExitType = SectionExitType.Source;
			source.ID = exitData.SourceID;
			source.OtherID = exitData.DestinationID;
			source.SourceBehavior = exitData.SourceBehavior;
			source.Position = new Vector2(exitData.SourceX, exitData.SourceY);
			source.Size = GameServices.GameObjectSize;

			SectionExit destination = new SectionExit(level.GetSectionByIndex(exitData.DestinationSectionID));
			destination.ExitType = SectionExitType.Destination;
			destination.ID = exitData.DestinationID;
			destination.OtherID = exitData.SourceID;
			destination.DestinationBehavior = exitData.DestinationBehavior;
			destination.Position = new Vector2(exitData.DestinationX, exitData.DestinationY);
			destination.Size = GameServices.GameObjectSize;

			level.GetSectionByIndex(exitData.SourceSectionID).SectionExits.Add(source);
			level.GetSectionByIndex(exitData.DestinationSectionID).SectionExits.Add(destination);
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (selectedObject.SelectedObjectType != EditorSelectedObjectType.SectionExit) { return; }

            var selectedExit = selectedObject.SelectedExit;
            SectionExit otherExit = level.GetSectionExitByID(selectedExit.OtherID);

            SectionExitFormData exitData = new SectionExitFormData(this, level);
            if (!exitData.ParseSucceded) { return; }

            var source = (selectedExit.SourceBehavior != ExitSourceBehavior.NotASource)
                ? selectedExit : otherExit;
            var destination = (source == selectedExit) ? otherExit : selectedExit;

            source.ExitType = SectionExitType.Source;
            source.ID = exitData.SourceID;
            source.OtherID = exitData.DestinationID;
            source.SourceBehavior = exitData.SourceBehavior;
            source.Position = new Vector2(exitData.SourceX, exitData.SourceY);

            destination.ExitType = SectionExitType.Destination;
            destination.ID = exitData.DestinationID;
            destination.OtherID = exitData.SourceID;
            destination.DestinationBehavior = exitData.DestinationBehavior;
            destination.Position = new Vector2(exitData.DestinationX, exitData.DestinationY);
        }
	}
}
