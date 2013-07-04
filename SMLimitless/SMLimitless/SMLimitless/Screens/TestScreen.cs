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
        /// A rectangle.
        /// </summary>
        private BoundingRectangle rectangle;

        /// <summary>
        /// A triangle.
        /// </summary>
        private RightTriangle triangle;

        /// <summary>
        /// Another rectangle.
        /// </summary>
        private BoundingRectangle other;

        /// <summary>
        /// A resolution.
        /// </summary>
        private Vector2 resolution;

        /// <summary>
        /// Another resolution.
        /// </summary>
        private Vector2 otherResolution;

        /// <summary>
        /// A flag.
        /// </summary>
        private bool doResolve;

        /// <summary>
        /// A string.
        /// </summary>
        private string debugText = "";

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Initialize(Screen owner, string parameters)
        {
            this.Effect = new FadeEffect();
            this.triangle = new RightTriangle(new BoundingRectangle(400f, 200f, 100f, 100f), RtSlopedSides.TopLeft);
            this.rectangle = new BoundingRectangle(0f, 0f, 100f, 100f);
            this.other = new BoundingRectangle(98f, 96f, 100f, 100f);
        }

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent() 
        {
        }

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public override void Draw()
        {
            Effect.Draw();
            this.triangle.Draw(false);
            this.rectangle.DrawOutline(Color.White);
            this.other.DrawOutline(Color.White);

            this.resolution.ToString().DrawString(new Vector2(16f, 16f), Color.White);
            this.otherResolution.ToString().DrawString(new Vector2(16f, 36f), Color.White);
        }

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public override void Update()
        {
            Effect.Update();
            if (Input.InputManager.IsCurrentKeyPress(Keys.Left))
            {
                this.rectangle.X -= 2f;
            }

            if (Input.InputManager.IsCurrentKeyPress(Keys.Right))
            {
                this.rectangle.X += 2f;
            }

            if (Input.InputManager.IsCurrentKeyPress(Keys.Up))
            {
                this.rectangle.Y -= 2f;
            }

            if (Input.InputManager.IsCurrentKeyPress(Keys.Down))
            {
                this.rectangle.Y += 2f;
            }

            if (Input.InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
            {
                this.doResolve = true;
            }

            this.resolution = this.otherResolution = Vector2.Zero;

            this.resolution = this.triangle.GetResolutionDistance(this.rectangle).GetIntersectionResolution();
            Intersection intersect = this.other.GetResolutionDistance(this.rectangle);
            if (intersect.IsIntersecting && this.doResolve)
            {
                this.otherResolution = intersect.GetIntersectionResolution();
                this.debugText = this.otherResolution.ToString();
            }

            if (this.resolution != Vector2.Zero)
            {
                this.rectangle.X += this.resolution.X;
                this.rectangle.Y += this.resolution.Y;
            }
            else if (this.otherResolution != Vector2.Zero)
            {
                this.rectangle.X += this.otherResolution.X;
                this.rectangle.Y += this.otherResolution.Y;
            }
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
