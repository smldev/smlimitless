//-----------------------------------------------------------------------
// <copyright file="Level.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using SMLimitless.Components;
using SMLimitless.Content;
using SMLimitless.Editor;
using SMLimitless.Editor.Attributes;
using SMLimitless.Physics;
using SMLimitless.Sprites.InternalSprites;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   The main unit of gameplay.
	/// </summary>
	[HasUserEditableProperties]
	public sealed class Level
	{
		/// <summary>
		///   The acceleration caused by gravity, measured in pixels per second
		///   per second.
		/// </summary>
		public static PhysicsSetting<float> GravityAcceleration =
			new PhysicsSetting<float>("Gravity Acceleration (px/sec²)", 0f, 10000f, 700f,
				PhysicsSettingType.FloatingPoint);

		private Section activeSection;

		private EditorSelectedObject selectedObject = new EditorSelectedObject();

		private EditorCameraTrackingObject trackingObject = null;

		/// <summary>
		///   Gets a string placed in all level files indicating the version of
		///   the serializer used to create it.
		/// </summary>
		[Obsolete]
		public static string SerializerVersion
		{
			get
			{
				return "Version 0.01";
			}
		}

		/// <summary>
		///   Gets the name of the author who created this level.
		/// </summary>
		public string Author { get; internal set; }

		/// <summary>
		///   Gets the name of the level, which is presented on menu screens.
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		///   Gets the absolute path to the file this level was loaded from.
		/// </summary>
		public string Path { get; internal set; }

		[PointProperty("Test", "Test_")]
		public Point TestProperty { get; set; }

		[PointProperty("Test2", "Test2_")]
		public Point TestReadOnlyProperty { get; private set; }

		/// <summary>
		///   Gets or sets the section that the player is currently in.
		/// </summary>
		internal Section ActiveSection
		{
			get
			{
				return activeSection;
			}
			set
			{
				activeSection = value;
				GameServices.Camera = value.Camera;
			}
		}

		/// <summary>
		///   Gets or sets a collection of all the paths to the content package
		///   folders used in this level.
		/// </summary>
		internal List<string> ContentFolderPaths { get; set; }

		internal bool EditorActive { get; private set; }
		internal EditorForm EditorForm { get; set; }

		/// <summary>
		///   Gets or sets the event script of this level.
		/// </summary>
		internal EventScript EventScript { get; set; }

		/// <summary>
		///   Gets or sets a collection of all the level exits in this level.
		/// </summary>
		internal List<LevelExit> LevelExits { get; set; }

		internal IDGenerator SectionExitIDGenerator { get; private set; }

		/// <summary>
		///   Gets or sets a collection of all the sections in this level.
		/// </summary>
		internal List<Section> Sections { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="Level" /> class.
		/// </summary>
		public Level()
		{
			LevelExits = new List<LevelExit>();
			Sections = new List<Section>();
			EventScript = new EventScript();
		}

		/// <summary>
		///   Draws this level.
		/// </summary>
		public void Draw()
		{
			if (EditorActive) { selectedObject.Draw(); }

			ActiveSection.Draw();
		}

		/// <summary>
		///   Initializes this level.
		/// </summary>
		public void Initialize()
		{
			LoadOverrides();
			Sections.ForEach(s => s.Initialize());
			selectedObject.Initialize(ActiveSection);
			// ContentFolderPaths.ForEach(f => Content.ContentPackageManager.AddPackageFromFolder(f));
		}

		/// <summary>
		///   Notifies this level that a level exit has been cleared.
		/// </summary>
		/// <param name="exitSpriteName">
		///   The name of the sprite that served as the level exit.
		/// </param>
		public void LevelExitCleared(string exitSpriteName)
		{
			// Look in this.levelExits for an exit with the sprite name. Notify
			// the owner (world/levelpack/whatever) that this exit has been
			// cleared Give the owner the LevelExit tied to the exitSpriteName
		}

		/// <summary>
		///   Loads the content of this level.
		/// </summary>
		public void LoadContent()
		{
			Sections.ForEach(s => s.LoadContent());
			selectedObject.LoadContent();
		}

		/// <summary>
		///   Updates this level.
		/// </summary>
		public void Update()
		{
			ActiveSection.Update();

			if (EditorActive)
			{
				trackingObject.Update();
				selectedObject.Update();
			}
		}

		internal void ChangeSection(int sectionIndex)
		{
			Section sectionToSwitchTo = GetSectionByIndex(sectionIndex);
			ActiveSection = sectionToSwitchTo;
		}

		internal Section GetSectionByIndex(int sectionIndex)
		{
			Section sectionWithIndex = Sections.FirstOrDefault(s => s.Index == sectionIndex);
			if (sectionWithIndex == null)
			{
				throw new ArgumentException($"No section in this level has an index of {sectionIndex}.");
			}
			return sectionWithIndex;
		}

		internal SectionExit GetSectionExitByID(int id)
		{
			foreach (Section section in Sections)
			{
				if (section.SectionExits.Any(e => e.ID == id))
				{
					return section.SectionExits.First(e => e.ID == id);
				}
			}

			return null;
		}

		internal void OnSectionExit(SectionExit source)
		{
			SectionExit destination = GetSectionExitByID(source.OtherID);

			Section sourceSection = source.Owner;
			Section destinationSection = destination.Owner;

			foreach (Section section in Sections)
			{
				section.CameraSystem.IsFrozen = false;
				section.ExitLock = null;
			}

			ChangeActiveSectionForExit(destinationSection, destination.IrisPoint, source, destination);
		}

		internal void ToggleEditor()
		{
			if (!EditorActive)
			{
				EditorActive = true;

				trackingObject = new EditorCameraTrackingObject();
				trackingObject.Initialize(ActiveSection);
				trackingObject.Position = ActiveSection.Camera.Viewport.Center;

				ActiveSection.ActivateEditor(trackingObject, selectedObject);

				EditorForm = new EditorForm(this, ActiveSection, selectedObject);
				EditorForm.Show();
			}
			else
			{
				EditorActive = false;

				trackingObject = null;
				ActiveSection.DeactivateEditor();

				EditorForm.Close();
				EditorForm.Dispose();
				EditorForm = null;
			}
		}

		internal void TransferEditorControlToSection(Section oldSection, Section newSection)
		{
			if (!EditorActive) { return; }

			oldSection.RemoveSpriteOnNextFrame(trackingObject);
			oldSection.RemoveSpriteOnNextFrame(selectedObject);

			trackingObject.Owner = newSection;
			selectedObject.Owner = newSection;

			oldSection.DeactivateEditor();
			newSection.ActivateEditor(trackingObject, selectedObject);

			EditorForm.SwitchToSection(newSection);

			ActiveSection = newSection;
		}

		private void ChangeActiveSectionForExit(Section section, Vector2 irisOutPosition, SectionExit source,
							SectionExit destination)
		{
			float halfScreenWidth = GameServices.ScreenSize.X / 2f;
			float halfScreenHeight = GameServices.ScreenSize.Y / 2f;
			Vector2 newCameraPosition = new Vector2(destination.Hitbox.Center.X - halfScreenWidth,
			destination.Hitbox.Center.Y - halfScreenHeight);

			ActiveSection = section;
			ActiveSection.SetIrisState(closed: true);
			ActiveSection.CameraSystem.IsFrozen = true;
			ActiveSection.CameraSystem.MoveIfFrozen(newCameraPosition);
			ActiveSection.IrisOut(90, irisOutPosition, (sender, e) =>
			{
				ActiveSection.IsActive = true;
				destination.Emerge(source.PlayersInExit);
				source.PlayersInExit.Clear();
				ActiveSection.CameraSystem.IsFrozen = false;
			});
		}

		private void LoadOverrides()
		{
			string levelFolderAbsolutePath = System.IO.Path.GetDirectoryName(Path);
			string levelFileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(Path);
			string pathToOverridesFile = Directory.GetFiles(levelFolderAbsolutePath, "overrides.txt",
				SearchOption.TopDirectoryOnly).FirstOrDefault();
			if (pathToOverridesFile == null) { return; }

			var contentOverrides = OverrideReader.GetOverridesFromFile(pathToOverridesFile);
			if (!contentOverrides.ContainsKey(levelFileNameWithoutExt)) { return; }
			foreach (string overrideFolder in contentOverrides[levelFileNameWithoutExt])
			{
				string absoluteOverrideFolderPath = System.IO.Path.Combine(levelFolderAbsolutePath, overrideFolder);
				ContentPackageManager.AddOverrideFolder(absoluteOverrideFolderPath);
			}
		}
	}
}
