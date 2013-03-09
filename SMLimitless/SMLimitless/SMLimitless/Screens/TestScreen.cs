using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Screens.Effects;
using SMLimitless.Graphics;
using SMLimitless.Sprites;

namespace SMLimitless.Screens
{
    public class TestScreen : Screen
    {
        /* Testing graphics objects */
        StaticGraphicsObject graphicsObject = new StaticGraphicsObject();
        AnimatedGraphicsObject animGraphicsObject = new AnimatedGraphicsObject();

        StaticGraphicsObject sheetObject = new StaticGraphicsObject();
        StaticGraphicsObject sheetRectObject = new StaticGraphicsObject();

        AnimatedGraphicsObject animSheetObject = new AnimatedGraphicsObject();

        public override void Initialize(Screen owner, string parameters)
        {
            this.effect = new FadeEffect(GameServices.ScreenSize);
            this.effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
            this.effect.effectCompletedEvent += new Interfaces.EffectCompletedEventHandler(effect_effectCompletedEvent);

            // Initalize the testing objects.
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile.png");
            string absolute2 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile_anim.png");
            string absolute3 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphicsObject.LoadFromMetadata(@"static-single>""" + absolute + @"""");
            animGraphicsObject.LoadFromMetadata(@"anim-single>""" + absolute2 + @""",16,10");

            sheetObject.LoadFromMetadata(@"static-spritesheet>""" + absolute3 + @""",16,16,0");
            sheetRectObject.LoadFromMetadata(@"static-spritesheet_r>""" + absolute3 + @""",16,16,[16:0:16:16]");

            animSheetObject.LoadFromMetadata(@"anim-spritesheet>""" + absolute3 + @""",16,16,8,4,5,6,7");
            this.isInitialized = true;
        }

        public override void LoadContent()
        {
            graphicsObject.LoadContent();
            animGraphicsObject.LoadContent();

            sheetObject.LoadContent();
            sheetRectObject.LoadContent();

            animSheetObject.LoadContent();
            this.isContentLoaded = true;
        }

        public override void Update()
        {
            if (this.IsRunning)
            {
                animGraphicsObject.Update();
                animSheetObject.Update();

                this.effect.Update(null);

                if (InputManager.IsNewKeyPress(Keys.X))
                {
                    BlankScreen blank = new BlankScreen();
                    blank.Initialize(this, "");
                    this.nextScreen = blank;
                    this.effect.Start(60, Interfaces.EffectDirection.Forward, Vector2.Zero, Color.Black);
                }
            }
        }

        public override void Draw()
        {
            if (this.IsRunning)
            {
                graphicsObject.Draw(new Vector2(256, 256), Color.White, SpriteEffects.FlipVertically);
                animGraphicsObject.Draw(new Vector2(256, 224), Color.White, false, SpriteEffects.FlipHorizontally);

                sheetObject.Draw(new Vector2(288, 256), Color.White);
                sheetRectObject.Draw(new Vector2(304, 256), Color.White);

                animSheetObject.Draw(new Vector2(320, 256), Color.White, false, SpriteEffects.None);

                this.effect.Draw(null, GameServices.SpriteBatch);
            }
        }

        public override void Start(string parameters = "")
        {
            base.Start();
            this.effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
        }

        void effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
        {
            if (nextScreen != null)
            {
                ScreenManager.AddScreen(this, nextScreen);
                ScreenManager.SwitchScreen(nextScreen);
                nextScreen = null;
            }
        }
    }
}
