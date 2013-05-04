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
        public ComplexGraphicsObject graphics;
        public Interpolator interpolator;
        public Vector2 position = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;

        public override void Update() 
        {
            effect.Update();
            graphics.Update();
            interpolator.Update();
            if (InputManager.IsCurrentKeyPress(Keys.W))
            {
                graphics.CurrentObjectName = "water";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.L))
            {
                graphics.CurrentObjectName = "lava";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.B))
            {
                graphics.CurrentObjectName = "block_break";
                graphics.Reset(true);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.D))
            {
                graphics.CurrentObjectName = "diamond_block";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.R))
            {
                if (graphics.CurrentObjectName != "diamond_block")
                {
                    graphics.Reset(true);
                }
            }
            else if (InputManager.IsCurrentKeyPress(Keys.I))
            {
                if (position.X == 400f)
                {
                    interpolator.Reset(400f, 0f, 4.0f, i => this.position.X = i.Value, i => this.velocity = Vector2.Zero, InterpolatorScales.SmoothStep);
                }
                else if (position.X == 0f)
                {
                    interpolator.Reset(0f, 400f, 4.0f, i => this.position.X = i.Value, i => this.velocity = Vector2.Zero, InterpolatorScales.SmoothStep);
                }
            }
        }

        public override void LoadContent() 
        {
            graphics.LoadContent();
        }

        public override void Initialize(Screen owner, string parameters)
        {
            effect = new FadeEffect();
            graphics = (ComplexGraphicsObject)GraphicsManager.LoadGraphicsObject(System.IO.Directory.GetCurrentDirectory() +  @"..\\..\\..\\..\\gfx\\complex_spritesheet.png");
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
