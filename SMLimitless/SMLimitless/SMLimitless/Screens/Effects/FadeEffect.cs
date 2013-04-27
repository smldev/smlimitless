using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using SMLimitless.Interfaces;
using SMLimitless.Extensions;

namespace SMLimitless.Screens.Effects
{
    /// <summary>
    /// A simple fade effect that can fade in and out.
    /// </summary>
    public sealed class FadeEffect : IEffect
    {
        public event EffectCompletedEventHandler effectCompletedEvent;

        // The amount to increment the transparency level by per frame.
        private float fadeDelta;
        private float currentFadeLevel;

        private bool isRunning;
        private bool isInitialized;
        private EffectDirection dir; // If the direction is forward, then fade to black.  If backward, fade from black.
        private Color color;

        public FadeEffect()
        {
            isInitialized = true;
        }

        public void LoadContent() { }


        public void Start(int length, EffectDirection direction, Vector2 position, Color color)
        {
            if (direction == EffectDirection.Forward && currentFadeLevel == 1f || direction == EffectDirection.Backward && currentFadeLevel == 0f) return;

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

        public void Stop()
        {
            isRunning = false;
            currentFadeLevel = 0f;
            fadeDelta = 0f;
        }

        /// <summary>
        /// Instantly changes the fade to fully black or fully transparent.
        /// </summary>
        /// <param name="direction">The direction in which to fade.  Forward fades to black, backward fades from black.</param>
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
                throw new Exception("The fade effect was not properly initialized.  Please set the screen size.");
            }
        }

        public void Draw()
        {
            if (isInitialized)
            {
                GameServices.SpriteBatch.DrawRectangle(Vector2.Zero.ToRectangle(GameServices.ScreenSize), color * currentFadeLevel);
            }
        }

        private void FadeFinished()
        {
            isRunning = false;
            fadeDelta = 0f;
            if (effectCompletedEvent != null)
                effectCompletedEvent(this, dir);
        }
    }
}
