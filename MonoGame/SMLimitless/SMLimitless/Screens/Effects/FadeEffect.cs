//-----------------------------------------------------------------------
// <copyright file="FadeEffect.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Screens.Effects
{
    /// <summary>
    /// A simple fade effect that can fade in and out.
    /// </summary>
    public sealed class FadeEffect : IEffect
    {
        /// <summary>
        /// The amount to increment the transparency level by per frame.
        /// </summary>
        private float fadeDelta;

        /// <summary>
        /// The current transparency level.
        /// </summary>
        private float currentFadeLevel;
        
        /// <summary>
        /// A value indicating whether the effect is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// A value indicating whether the effect is initialized.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Defines the effect direction for this effect.
        /// If the direction is forward, then fade to black.  If backward, fade from black.
        /// </summary>
        private EffectDirection dir;

        /// <summary>
        /// The color to fade to or from.
        /// </summary>
        private Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeEffect"/> class.
        /// </summary>
        public FadeEffect()
        {
			isInitialized = true;
        }

        /// <summary>
        /// This event is fired when the effect is completed.
        /// </summary>
        public event EffectCompletedEventHandler EffectCompletedEvent;

        /// <summary>
        /// Loads the content for this effect.
        /// </summary>
        public void LoadContent() 
        { 
        }

        /// <summary>
        /// Starts this effect.
        /// </summary>
        /// <param name="length">The number of frames this effect should run for.</param>
        /// <param name="direction">The direction to run this effect.</param>
        /// <param name="position">This parameter is meaningless for this IEffect implementer.</param>
        /// <param name="color">The color to fade to or from.</param>
        public void Start(int length, EffectDirection direction, Vector2 position, Color color)
        {
            if ((direction == EffectDirection.Forward && currentFadeLevel == 1f) || (direction == EffectDirection.Backward && currentFadeLevel == 0f)) 
            {
                return;
            }

			isRunning = true;
			fadeDelta = 1.0f / length;
			dir = direction;
            this.color = color;
            if (dir == EffectDirection.Backward)
            {
				// fade in from black
				currentFadeLevel = 1.0f;
            }
        }

        /// <summary>
        /// Stops this effect and removes any fade.
        /// </summary>
        public void Stop()
        {
			isRunning = false;
			currentFadeLevel = 0f;
			fadeDelta = 0f;
        }

        /// <summary>
        /// Instantly changes the fade to fully black or fully transparent.
        /// </summary>
        /// <param name="direction">The direction in which to fade. Forward fades to black, backward fades from black.</param>
        /// <param name="color">The color to set to.</param>
        public void Set(EffectDirection direction, Color color)
        {
            this.color = color;
            if (direction == EffectDirection.Forward)
            {
				currentFadeLevel = 1f;
            }
            else
            {
				currentFadeLevel = 0f;
            }
        }

        /// <summary>
        /// Updates this effect.
        /// </summary>
        public void Update()
        {
            if (isRunning && isInitialized)
            {
                switch (dir)
                {
                    case EffectDirection.Forward:
                        // fade to black
                        if (currentFadeLevel < 1.0f)
                        {
							currentFadeLevel += fadeDelta;
                        }
                        else FadeFinished();
                        break;
                    case EffectDirection.Backward:
                        if (currentFadeLevel > 0f)
                        {
							currentFadeLevel -= fadeDelta;
                        }
                        else FadeFinished();
                        break;
                }
            }
            else if (!isInitialized)
            {
                throw new InvalidOperationException("The fade effect was not properly initialized.  Please set the screen size.");
            }
        }

        /// <summary>
        /// Draws this effect.
        /// </summary>
        public void Draw()
        {
            if (isInitialized)
            {
                GameServices.SpriteBatch.DrawRectangle(Vector2.Zero.ToRectangle(GameServices.ScreenSize), color * currentFadeLevel);
            }
        }

        /// <summary>
        /// Called when the effect is finished.
        /// Fires the EffectCompletedEvent.
        /// </summary>
        private void FadeFinished()
        {
			isRunning = false;
			fadeDelta = 0f;
            if (EffectCompletedEvent != null)
            {
				EffectCompletedEvent(this, dir);
            }
        }
    }
}
