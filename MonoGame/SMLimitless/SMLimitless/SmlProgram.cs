//-----------------------------------------------------------------------
// <copyright file="SmlProgram.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Screens;
using SMLimitless.Sounds;

namespace SMLimitless
{
    /// <summary>
    /// The main type for the game.
    /// </summary>
    public sealed class SmlProgram : Game
    {
        /// <summary>
        /// Handles the configuration and management of the graphics device.
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Enables a group of sprites to be drawn using the same settings.
        /// </summary>
        private SpriteBatch spriteBatch;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SmlProgram"/> class.
        /// </summary>
        public SmlProgram()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the game's resources and managers.
        /// </summary>
        protected override void Initialize()
        {
            GameSettings.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize();
            GameServices.ScreenSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            string contentPackageSettingsPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"TestPackage\settings.txt");
            ////SMLimitless.Content.ContentPackageManager.AddPackage(contentPackageSettingsPath);
            ////GameServices.Camera = new Physics.Camera2D(); // NOTE: comment out this line and the above if loading a LevelScreen.
            ScreenManager.SetRootScreen(new LevelScreen(), System.IO.Directory.GetCurrentDirectory() + @"\testLevel.txt");
            base.Initialize();
        }

        /// <summary>
        /// Loads the content for this game and all its components.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            GameServices.InitializeServices(this.GraphicsDevice, this.spriteBatch, this.Content);
            GameServices.InitializeFont("font");

            ScreenManager.LoadContent();
        }

        /// <summary>
        /// Unloads the content for this game and all its components.
        /// </summary>
        protected override void UnloadContent()
        {
            SoundManager.UnloadContent();
        }

        /// <summary>
        /// Updates the game.
        /// All calls to the Update() method of every other type should be done here.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state expressed in values that can be used by 
        /// variable-step (real time) or fixed-step (game time) games.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (GameServices.GetService<GameTime>() == null)
            {
                GameServices.AddService<GameTime>(gameTime);
            }

            InputManager.Update();
            ScreenManager.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game.
        /// </summary>
        /// <param name="gameTime">Snapshot of the game timing state expressed in values that can be used by
        /// variable-step (real time) or fixed-step (game time) games.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, GameServices.Camera.GetTransformation());
            ScreenManager.Draw();
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
