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

        /* Testing graphics objects */
        StaticGraphicsObject graphicsObject = new StaticGraphicsObject();
        AnimatedGraphicsObject animGraphicsObject = new AnimatedGraphicsObject();

        StaticGraphicsObject sheetObject = new StaticGraphicsObject();
        StaticGraphicsObject sheetRectObject = new StaticGraphicsObject();

        AnimatedGraphicsObject animSheetObject = new AnimatedGraphicsObject();

        public SmlProgram()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SpritesheetManager.Initalize();

            // Initalize the testing objects.
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile.png");
            string absolute2 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile_anim.png");
            string absolute3 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphicsObject.LoadFromMetadata(@"static-single>""" + absolute + @"""");
            animGraphicsObject.LoadFromMetadata(@"anim-single>""" + absolute2 + @""",16,10");

            sheetObject.LoadFromMetadata(@"static-spritesheet>""" + absolute3 + @""",16,16,0");
            sheetRectObject.LoadFromMetadata(@"static-spritesheet_r>""" + absolute3 + @""",16,16,[16:0:16:16]");

            animSheetObject.LoadFromMetadata(@"anim-spritesheet>""" + absolute3 + @""",16,16,8,5,6,7,8");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameServices.InitializeServices(this.GraphicsDevice, spriteBatch, Content);
            SpritesheetManager.LoadContent();

            graphicsObject.LoadContent();
            animGraphicsObject.LoadContent();

            sheetObject.LoadContent();
            sheetRectObject.LoadContent();

            animSheetObject.LoadContent();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();
            graphicsObject.Draw(new Vector2(256, 256), Color.White, SpriteEffects.FlipVertically);
            animGraphicsObject.Draw(new Vector2(256, 224), Color.White, false, SpriteEffects.FlipHorizontally);

            sheetObject.Draw(new Vector2(288, 256), Color.White);
            sheetRectObject.Draw(new Vector2(304, 256), Color.White);

            animSheetObject.Draw(new Vector2(320, 256), Color.White, false, SpriteEffects.None);
            this.spriteBatch.End();

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
