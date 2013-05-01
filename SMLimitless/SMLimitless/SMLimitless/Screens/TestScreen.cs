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
        public StaticGraphicsObject graphics;
        public Interpolator interpolator;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;

        public override void Update() 
        {
            effect.Update();
            graphics.Update();
            interpolator.Update();
        }

        public override void LoadContent() 
        {
            graphics.LoadContent();
        }

        public override void Initialize(Screen owner, string parameters)
        {
            effect = new FadeEffect();
            graphics = (StaticGraphicsObject)GraphicsManager.LoadGraphicsObject(System.IO.Directory.GetCurrentDirectory() +  @"..\\..\\..\\..\\gfx\\smw_concrete_block.png");
            interpolator = new Interpolator(0f, 400f, 4.0f, i => this.position.X = i.Value, i => this.velocity = Vector2.Zero, InterpolatorScales.SmoothStep);
        }

        public override void Draw()
        {
            graphics.Draw(position, Color.White);
            effect.Draw();
        }

        public override void Start(string parameters = "")
        {
            base.Start();
            effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
            effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
            
        }

        private void AdjustSpeed(float addend)
        {
            this.velocity.X += addend;
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
