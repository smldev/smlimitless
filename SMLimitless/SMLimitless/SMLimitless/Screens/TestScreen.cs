//-----------------------------------------------------------------------
// <copyright file="TestScreen.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;
using SMLimitless.Sprites;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A "scratch pad" screen that can be used
    /// to test anything. The contents of the screen
    /// may change without warning.
    /// </summary>
    public class TestScreen : Screen
    {
        /// <summary>
        /// A ComplexGraphicsObject.
        /// </summary>
        private ComplexGraphicsObject graphics;

        /// <summary>
        /// An Interpolator.
        /// </summary>
        private Interpolator interpolator;

        /// <summary>
        /// Represents the position of the graphic.
        /// </summary>
        private Vector2 position = Vector2.Zero;

        /// <summary>
        /// Updates the screen.
        /// Press W, L, B, D, R, or I.
        /// </summary>
        public override void Update() 
        {
            Effect.Update();
            this.graphics.Update();
            this.interpolator.Update();
            if (InputManager.IsCurrentKeyPress(Keys.W))
            {
                this.graphics.CurrentObjectName = "water";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.L))
            {
                this.graphics.CurrentObjectName = "lava";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.B))
            {
                this.graphics.CurrentObjectName = "block_break";
                this.graphics.Reset(true);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.D))
            {
                this.graphics.CurrentObjectName = "diamond_block";
            }
            else if (InputManager.IsCurrentKeyPress(Keys.R))
            {
                if (this.graphics.CurrentObjectName != "diamond_block")
                {
                    this.graphics.Reset(true);
                }
            }
            else if (InputManager.IsCurrentKeyPress(Keys.I))
            {
                if (this.position.X == 400f)
                {
                    this.interpolator.Reset(400f, 0f, 1.0f, i => this.position.X = i.Value, i => { }, InterpolatorScales.SmoothStep);
                }
                else if (this.position.X == 0f)
                {
                    this.interpolator.Reset(0f, 400f, 12.0f, i => this.position.X = i.Value, i => { }, InterpolatorScales.SmoothStep);
                }
            }
        }

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent() 
        {
            this.graphics.LoadContent();
        }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Initialize(Screen owner, string parameters)
        {
            this.Effect = new FadeEffect();
            this.graphics = (ComplexGraphicsObject)GraphicsManager.LoadGraphicsObject(System.IO.Directory.GetCurrentDirectory() + @"..\\..\\..\\..\\gfx\\complex_spritesheet.png");
            this.interpolator = new Interpolator(0f, 400f, 4.0f, i => this.position.X = i.Value, i => { }, InterpolatorScales.SmoothStep);
        }

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public override void Draw()
        {
            this.graphics.Draw(this.position, Color.White);
            GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, this.interpolator.Value.ToString(), new Vector2(16, 16), Color.White);
            GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, Math.Min((this.interpolator.Progress + 1) / this.interpolator.Length * GameServices.GameTime.GetElapsedSeconds(), 1f).ToString(), new Vector2(16, 36), Color.White);
            Effect.Draw();
        }

        /// <summary>
        /// Starts this screen.
        /// </summary>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Start(string parameters = "")
        {
            base.Start();
            Effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
            Effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
        }

        /// <summary>
        /// Handles the EffectCompletedEvent.
        /// </summary>
        /// <param name="sender">The object sending this event.</param>
        /// <param name="direction">The direction of the completed effect.</param>
        private void Effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
        {
            if (this.NextScreen != null)
            {
                ScreenManager.AddScreen(this, this.NextScreen);
                ScreenManager.SwitchScreen(this.NextScreen);
                this.NextScreen = null;
            }
        }
    }
}
