//-----------------------------------------------------------------------
// <copyright file="FadeEffect.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
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
            this.isInitialized = true;
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
            if ((direction == EffectDirection.Forward && this.currentFadeLevel == 1f) || (direction == EffectDirection.Backward && this.currentFadeLevel == 0f)) 
            {
                return;
            }

            this.isRunning = true;
            this.fadeDelta = 1.0f / length;
            this.dir = direction;
            this.color = color;
            if (this.dir == EffectDirection.Backward)
            {
                // fade in from black
                this.currentFadeLevel = 1.0f;
            }
        }

        /// <summary>
        /// Stops this effect and removes any fade.
        /// </summary>
        public void Stop()
        {
            this.isRunning = false;
            this.currentFadeLevel = 0f;
            this.fadeDelta = 0f;
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
                this.currentFadeLevel = 1f;
            }
            else
            {
                this.currentFadeLevel = 0f;
            }
        }

        /// <summary>
        /// Updates this effect.
        /// </summary>
        public void Update()
        {
            if (this.isRunning && this.isInitialized)
            {
                switch (this.dir)
                {
                    case EffectDirection.Forward:
                        // fade to black
                        if (this.currentFadeLevel < 1.0f)
                        {
                            this.currentFadeLevel += this.fadeDelta;
                        }
                        else FadeFinished();
                        break;
                    case EffectDirection.Backward:
                        if (this.currentFadeLevel > 0f)
                        {
                            this.currentFadeLevel -= this.fadeDelta;
                        }
                        else FadeFinished();
                        break;
                }
            }
            else if (!this.isInitialized)
            {
                throw new Exception("The fade effect was not properly initialized.  Please set the screen size.");
            }
        }

        /// <summary>
        /// Draws this effect.
        /// </summary>
        public void Draw()
        {
            if (this.isInitialized)
            {
                GameServices.SpriteBatch.DrawRectangle(Vector2.Zero.ToRectangle(GameServices.ScreenSize), this.color * this.currentFadeLevel);
            }
        }

        /// <summary>
        /// Called when the effect is finished.
        /// Fires the EffectCompletedEvent.
        /// </summary>
        private void FadeFinished()
        {
            this.isRunning = false;
            this.fadeDelta = 0f;
            if (this.EffectCompletedEvent != null)
            {
                this.EffectCompletedEvent(this, this.dir);
            }
        }
    }
}
