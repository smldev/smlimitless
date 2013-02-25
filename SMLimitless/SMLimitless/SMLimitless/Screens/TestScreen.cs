using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Screens.Effects;
using SMLimitless.Graphics;

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

        public override void Initialize(string parameters)
        {
            this.effect = new FadeEffect(GameServices.ScreenSize);
            this.effect.Set(Interfaces.EffectDirection.Forward, Color.Black);

            // Initalize the testing objects.
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile.png");
            string absolute2 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_tile_anim.png");
            string absolute3 = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphicsObject.LoadFromMetadata(@"static-single>""" + absolute + @"""");
            animGraphicsObject.LoadFromMetadata(@"anim-single>""" + absolute2 + @""",16,10");

            sheetObject.LoadFromMetadata(@"static-spritesheet>""" + absolute3 + @""",16,16,0");
            sheetRectObject.LoadFromMetadata(@"static-spritesheet_r>""" + absolute3 + @""",16,16,[16:0:16:16]");

            animSheetObject.LoadFromMetadata(@"anim-spritesheet>""" + absolute3 + @""",16,16,8,5,6,7,8");
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
            }
        }

        public override void Start()
        {
            base.Start();
            this.effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
        }


        public override string Exit()
        {
            return "";
        }
    }
}
