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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A "scratch pad" screen that can be used
    /// to test anything. The contents of the screen
    /// may change without warning.
    /// </summary>
    public class TestScreen : Screen
    {
        //private BoundingRectangle a = new BoundingRectangle(400f, 200f, 100f, 100f);
        private RightTriangle a = new RightTriangle(400f, 200f, 100f, 100f, RtSlopedSides.BottomRight);
        private BoundingRectangle b = new BoundingRectangle(0f, 0f, 20f, 20f);
        /// <summary>
        /// Updates the screen.
        /// </summary>
        public override void Update()
        {
            Effect.Update();

            float speed = 3f;
            if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                b.X -= speed;
            }
            if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                b.X += speed;
            }
            if (InputManager.IsCurrentKeyPress(Keys.Up))
            {
                b.Y -= speed;
            }
            if (InputManager.IsCurrentKeyPress(Keys.Down))
            {
                b.Y += speed;
            }

            Vector2 resolution = a.GetCollisionResolution(b);
            if (resolution.IsNaN()) resolution = Vector2.Zero;
            b.X += resolution.X;
            b.Y += resolution.Y;
        }

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent()
        {
        }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Initialize(Screen owner, string parameters)
        {
            this.Effect = new FadeEffect();
        }

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public override void Draw()
        {
            Effect.Draw();
            //this.a.DrawOutline(Color.White);
            this.a.Draw(false);
            b.DrawOutline(Color.Red);
            a.GetCollisionResolution(b).ToString().DrawStringDefault();
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

// Basic TestScreen implementation - copy when needed
/////// <summary>
/////// A "scratch pad" screen that can be used
/////// to test anything. The contents of the screen
/////// may change without warning.
/////// </summary>
////public class TestScreen : Screen
////{
////    /// <summary>
////    /// Updates the screen.
////    /// </summary>
////    public override void Update()
////    {
////        Effect.Update();
////    }

////    /// <summary>
////    /// Loads the content for this screen.
////    /// </summary>
////    public override void LoadContent()
////    {
////    }

////    /// <summary>
////    /// Initializes this screen.
////    /// </summary>
////    /// <param name="owner">The screen that is creating this one.</param>
////    /// <param name="parameters">Parameters are unused.</param>
////    public override void Initialize(Screen owner, string parameters)
////    {
////        this.Effect = new FadeEffect();
////    }

////    /// <summary>
////    /// Draws this screen.
////    /// </summary>
////    public override void Draw()
////    {
////        Effect.Draw();
////    }

////    /// <summary>
////    /// Starts this screen.
////    /// </summary>
////    /// <param name="parameters">Parameters are unused.</param>
////    public override void Start(string parameters = "")
////    {
////        base.Start();
////        Effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
////        Effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
////    }

////    /// <summary>
////    /// Handles the EffectCompletedEvent.
////    /// </summary>
////    /// <param name="sender">The object sending this event.</param>
////    /// <param name="direction">The direction of the completed effect.</param>
////    private void Effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
////    {
////        if (this.NextScreen != null)
////        {
////            ScreenManager.AddScreen(this, this.NextScreen);
////            ScreenManager.SwitchScreen(this.NextScreen);
////            this.NextScreen = null;
////        }
////    }
////}
