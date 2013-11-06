//-----------------------------------------------------------------------
// <copyright file="TestScreen.cs" company="The Limitless Development Team">
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
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A "scratch pad" screen that can be used
    /// to test anything. The contents of the screen
    /// may change without warning.
    /// </summary>
    public class TestScreen : Screen
    {
        private Section section;

        /// <summary>
        /// Updates the screen.
        /// </summary>
        public override void Update()
        {
            Effect.Update();

            if (InputManager.IsCurrentActionPress(InputAction.Left))
            {
                GameServices.Camera.Position = new Vector2(GameServices.Camera.Position.X - 10f, GameServices.Camera.Position.Y);
            }
            if (InputManager.IsCurrentActionPress(InputAction.Right))
            {
                GameServices.Camera.Position = new Vector2(GameServices.Camera.Position.X + 10f, GameServices.Camera.Position.Y);
            }
            if (InputManager.IsCurrentActionPress(InputAction.Up))
            {
                GameServices.Camera.Position = new Vector2(GameServices.Camera.Position.X, GameServices.Camera.Position.Y - 10f);
            }
            if (InputManager.IsCurrentActionPress(InputAction.Down))
            {
                GameServices.Camera.Position = new Vector2(GameServices.Camera.Position.X, GameServices.Camera.Position.Y + 10f);
            }

            this.section.Update();
        }

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            this.section.LoadContent();
        }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Initialize(Screen owner, string parameters)
        {
            this.Effect = new FadeEffect();
            Content.ContentPackageManager.AddPackage(System.IO.Directory.GetCurrentDirectory() + @"\TestPackage\settings.txt");

            BackgroundData data = new BackgroundData();
            BackgroundData.BackgroundLayerData layer = new BackgroundData.BackgroundLayerData();
            BackgroundData.BackgroundLayerData layer2 = new BackgroundData.BackgroundLayerData();
            layer.SetManually("Notepad", Sprites.BackgroundScrollDirection.Horizontal, 1f);
            layer2.SetManually("eudcedit", Sprites.BackgroundScrollDirection.Vertical, 0.5f);
            data.SetManually(Color.LightBlue, Color.Blue, new List<BackgroundData.BackgroundLayerData>() { layer, layer2 });

            section = new Section(new BoundingRectangle(0f, 0f, 4096f, 4096f));
            section.Initialize(data);

            GameServices.Camera = section.Camera;
        }

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public override void Draw()
        {
            Effect.Draw();
            section.Draw();
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
