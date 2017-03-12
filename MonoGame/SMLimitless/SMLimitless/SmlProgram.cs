//-----------------------------------------------------------------------
// <copyright file="SmlProgram.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Debug;
using SMLimitless.Input;
using SMLimitless.Screens;
using SMLimitless.Sounds;

namespace SMLimitless
{
	/// <summary>
	///   The main type for the game.
	/// </summary>
	public sealed class SmlProgram : Game
	{
		/// <summary>
		///   Handles the configuration and management of the graphics device.
		/// </summary>
		private GraphicsDeviceManager graphics;

		/// <summary>
		///   Enables a group of sprites to be drawn using the same settings.
		/// </summary>
		private SpriteBatch spriteBatch;

		/// <summary>
		///   Sets a path for the level to load when the game starts.
		/// </summary>
		internal string InitialLevelFilePath { private get; set; } = null;

		/// <summary>
		///   Initializes a new instance of the <see cref="SmlProgram" /> class.
		/// </summary>
		public SmlProgram()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "InternalContent";

#if ALTGFX
			Window.Title = "SMLimitless";
#else
			Window.Title = "Super Mario Limitless";
#endif

			IsMouseVisible = true;
		}

		/// <summary>
		///   Initializes the game's resources and managers.
		/// </summary>
		protected override void Initialize()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			GameSettings.Initialize();
			InputManager.Initialize();
			ScreenManager.Initialize();
			GameServices.ScreenSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

			string contentPackageSettingsPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"TestPackage\settings.txt");
			// SMLimitless.Content.ContentPackageManager.AddPackage(contentPackageSettingsPath);
			// GameServices.Camera = new Physics.Camera2D(); // NOTE: comment out
			// this line and the above if loading a LevelScreen.
			ScreenManager.SetRootScreen(new LevelScreen(), GetInitialLevelFilePath());

			base.Initialize();

			stopwatch.Stop();
			Logger.LogInfo(string.Format("Game initialization completed (took {0} ms)", stopwatch.ElapsedMilliseconds));
		}

		/// <summary>
		///   Gets the path to the level to load when the game starts.
		/// </summary>
		/// <returns>
		///   The value of the <see cref="InitialLevelFilePath" /> property, or
		///   if it's null, the selected file from an <see cref="OpenFileDialog"
		///   /> instance.
		/// </returns>
		private string GetInitialLevelFilePath()
		{
			if (InitialLevelFilePath != null) { return InitialLevelFilePath; }

			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Title = "Super Mario Limitless";
			fileDialog.Filter = "SML Level File (*.lvl)|*.lvl|All files (*.*)|*.*";
			fileDialog.Multiselect = false;

			string selectedFilePath = null;
			while (selectedFilePath == null)
			{
				DialogResult result = fileDialog.ShowDialog();
				if (result == DialogResult.Cancel)
				{
					Exit();
					Environment.Exit(1223); // ERR_CANCELLED
				}
				if (File.Exists(fileDialog.FileName)) { selectedFilePath = fileDialog.FileName; }
				else
				{
					string message = $"The file at that path doesn't exist.\r\n\r\nPath:{fileDialog.FileName}";
					MessageBox.Show(message, "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			return selectedFilePath;
		}

		/// <summary>
		///   Loads the content for this game and all its components.
		/// </summary>
		protected override void LoadContent()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			spriteBatch = new SpriteBatch(GraphicsDevice);
			GameServices.InitializeServices(GraphicsDevice, spriteBatch, Content);
			GameServices.InitializeFont("font");

			ScreenManager.LoadContent();
			GameServices.Effects.Add("TestShader", Content.Load<Effect>("TestShader"));

			stopwatch.Stop();
			Logger.LogInfo(string.Format("Load content completed (took {0} ms)", stopwatch.ElapsedMilliseconds));
		}

		/// <summary>
		///   Updates the game. All calls to the Update() method of every other
		///   type should be done here.
		/// </summary>
		/// <param name="gameTime">
		///   Snapshot of the game timing state expressed in values that can be
		///   used by variable-step (real time) or fixed-step (game time) games.
		/// </param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				Exit();
			}

			if (GameServices.GetService<GameTime>() == null)
			{
				GameServices.AddService<GameTime>(gameTime);
			}

            Application.DoEvents();

			InputManager.Update();
			ScreenManager.Update();
			SMLimitless.Components.ActionScheduler.Instance.Update();

			base.Update(gameTime);
		}

		/// <summary>
		///   Draws the game.
		/// </summary>
		/// <param name="gameTime">
		///   Snapshot of the game timing state expressed in values that can be
		///   used by variable-step (real time) or fixed-step (game time) games.
		/// </param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, GameServices.Camera.GetTransformation());
			ScreenManager.Draw();
			spriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		///   Unloads the content for this game and all its components.
		/// </summary>
		protected override void UnloadContent()
		{
			SoundManager.UnloadContent();
			Content.Unload();
		}
	}
}
