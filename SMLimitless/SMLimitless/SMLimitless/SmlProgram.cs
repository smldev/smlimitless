#region Using Statements
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
using SMLimitless.Screens;
#endregion

namespace SMLimitless
{
    /// <summary>
    /// The main type for the game.
    /// </summary>
    public sealed class SmlProgram : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public SmlProgram()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SpritesheetManager.Initalize();
            ScreenManager.Initalize();
            ScreenManager.SetRootScreen(new TestScreen(), "");
            GameServices.ScreenSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameServices.InitializeServices(GraphicsDevice, spriteBatch, Content);
            SpritesheetManager.LoadContent();
            ScreenManager.LoadContent();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            if (GameServices.GetService<GameTime>() == null)
            {
                GameServices.AddService<GameTime>(gameTime);
            }

            // TODO: Add your update logic here
            InputManager.Update();
            ScreenManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            ScreenManager.Draw();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    #region Program
    #if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SmlProgram game = new SmlProgram())
            {
                game.Run();
            }
        }
    }
    #endif
    #endregion
}
