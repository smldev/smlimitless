//-----------------------------------------------------------------------
// <copyright file="BlankScreen.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Interfaces;
using SMLimitless.Screens.Effects;

namespace SMLimitless.Screens
{
	/// <summary>
	///   A screen that has nothing.
	/// </summary>
	public class BlankScreen : Screen
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
		/// <param name="owner">The screen that has created this one.</param>
		/// <param name="parameters">
		///   This screen accepts no parameters, use String.Empty.
		/// </param>
		public override void Initialize(Screen owner, string parameters)
		{
			Owner = owner;
			Effect = new FadeEffect();
			Effect.Set(EffectDirection.Forward, Color.BlueViolet);
		}

		/// <summary>
		///   Loads the (lack of) content for this screen.
		/// </summary>
		public override void LoadContent()
		{
		}

		/// <summary>
		///   Updates this screen. Press X to exit back to the owner screen.
		/// </summary>
		public override void Update()
		{
			Effect.Update();

			if (Keyboard.GetState().IsKeyDown(Keys.X))
			{
				Exit();
			}
		}
	}
}
