//-----------------------------------------------------------------------
// <copyright file="LevelScreen.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Screens
{
	/// <summary>
	///   A screen that contains a <see cref="Level" /> instance.
	/// </summary>
	public class LevelScreen : Screen
	{
		/// <summary>
		///   The level contained within this screen.
		/// </summary>
		private Level level;

		// The screens that created this screen.
		private Screen owner;

		/// <summary>
		///   Draws this screen.
		/// </summary>
		public override void Draw()
		{
			level.Draw();
		}

		/// <summary>
		///   Initializes this screen.
		/// </summary>
		/// <param name="owner">The screen that is creating this one.</param>
		/// <param name="parameters">
		///   Parameters to specify how the screen should be initialized.
		///   Contains a path to the level file to load.
		/// </param>
		public override void Initialize(Screen owner, string parameters)
		{
			this.owner = owner;

			// temporary level.ContentFolderPaths = new List<string>() {
			// System.IO.Directory.GetCurrentDirectory() + @"\TestPackage" };
			level = new IO.LevelSerializers.Serializer003().Deserialize(System.IO.File.ReadAllText(parameters));
			level.Path = parameters;
			level.Initialize();
			level.LevelExitTriggered += Level_LevelExitTriggered;
		}

		private void Level_LevelExitTriggered(object sender, EventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog()
			{
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = ".lvl",
				Filter = "SML Levels (*.lvl)|*.lvl|All files|*.*",
				Multiselect = false,
				Title = "Super Mario Limitless"
			};

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string path = ofd.FileName;

				Level newLevel = null;
				try
				{
					newLevel = new IO.LevelSerializers.Serializer003().Deserialize(System.IO.File.ReadAllText(path));
					newLevel.Path = path;
					newLevel.Initialize();
					newLevel.LoadContent();
				}
				catch (Exception ex)
				{
					string message = string.Concat("A problem occurred while trying to load the level.",
						Environment.NewLine, ex.GetType().Name,
						Environment.NewLine, ex.Message ?? "");
					System.Windows.Forms.MessageBox.Show(message, "Super Mario Limitless", 
						System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
					System.Diagnostics.Debugger.Break();
					return;
				}

				level = newLevel;
				level.LevelExitTriggered += Level_LevelExitTriggered;
			}
		}

		/// <summary>
		///   Loads the content for this screen.
		/// </summary>
		public override void LoadContent()
		{
			level.LoadContent();
		}

        /// <summary>
        /// Unloads the content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            level.UnloadContent();
        }

        /// <summary>
        ///   Updates this screen.
        /// </summary>
        public override void Update()
		{
			level.Update();
		}
	}
}
