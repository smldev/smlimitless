﻿//-----------------------------------------------------------------------
// <copyright file="TestScreen.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using SMLimitless.Screens.Effects;

namespace SMLimitless.Screens
{
	/// <summary>
	///   A "scratch pad" screen that can be used to test anything. The contents
	///   of the screen may change without warning.
	/// </summary>
	public class TestScreen : Screen
	{
		/// <summary>
		///   Draws this screen.
		/// </summary>
		public override void Draw()
		{
			Effect.Draw();
		}

		/// <summary>
		///   Initializes this screen.
		/// </summary>
		/// <param name="owner">The screen that is creating this one.</param>
		/// <param name="parameters">Parameters are unused.</param>
		public override void Initialize(Screen owner, string parameters)
		{
			Effect = new FadeEffect();
		}

		/// <summary>
		///   Loads the content for this screen.
		/// </summary>
		public override void LoadContent()
		{
		}

		/// <summary>
		///   Starts this screen.
		/// </summary>
		/// <param name="parameters">Parameters are unused.</param>
		public override void Start(string parameters = "")
		{
			base.Start();
			Effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
			Effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		public override void Update()
		{
			Effect.Update();
		}

		/// <summary>
		///   Handles the EffectCompletedEvent.
		/// </summary>
		/// <param name="sender">The object sending this event.</param>
		/// <param name="direction">The direction of the completed effect.</param>
		private void Effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
		{
			if (NextScreen != null)
			{
				ScreenManager.AddScreen(this, NextScreen);
				ScreenManager.SwitchScreen(NextScreen);
				NextScreen = null;
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
