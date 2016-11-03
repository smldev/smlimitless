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
			// level = new Level();
			//level.Sections.Add(new Section(level));

			// temporary level.ContentFolderPaths = new List<string>() {
			// System.IO.Directory.GetCurrentDirectory() + @"\TestPackage" };
			level = new IO.LevelSerializers.Serializer003().Deserialize(System.IO.File.ReadAllText(parameters));
			level.Path = parameters;
			level.Initialize();
		}

		/// <summary>
		///   Loads the content for this screen.
		/// </summary>
		public override void LoadContent()
		{
			level.LoadContent();
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
