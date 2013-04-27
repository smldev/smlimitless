using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;
using SMLimitless.Screens.Effects;
using SMLimitless.Graphics;
using SMLimitless.Sprites;
using SMLimitless.Physics;

namespace SMLimitless.Screens
{
    public class TestScreen : Screen
    {
        // Testing adjustability of AnimatedGraphicsObjects
        AnimatedGraphicsObject graphics = new AnimatedGraphicsObject();

        public override void Update() 
        {
            effect.Update();
            if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                graphics.AdjustSpeed(10f);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                graphics.AdjustSpeed(-10f);
            }
            graphics.Update();
        }

        public override void LoadContent() 
        {
            graphics.LoadContent();
        }

        public override void Initialize(Screen owner, string parameters)
        {
            effect = new FadeEffect();
            graphics = (AnimatedGraphicsObject)GraphicsManager.LoadGraphicsObject(System.IO.Directory.GetCurrentDirectory() +  @"..\\..\\..\\..\\gfx\\smw_player_big.png");
        }

        public override void Draw()
        {
            graphics.Draw(new Vector2(0, 0), Color.White);
            GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, graphics.AnimationCycleLength.ToString(), Vector2.Zero, Color.White);
            effect.Draw();
        }

        public override void Start(string parameters = "")
        {
            base.Start();
            effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
            effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
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
