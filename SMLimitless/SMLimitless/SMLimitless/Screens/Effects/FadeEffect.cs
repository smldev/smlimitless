﻿using System;
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

        private Vector2 screensize;

        public FadeEffect(Vector2 ScreenSize)
        {
            screensize = ScreenSize;
            isInitialized = true;
        }

        public void LoadContent(ContentManager Content) { }

        public void SetScreenSize(Vector2 screenSize)
        {
            screensize = screenSize;
            isInitialized = true;
        }

        public void Start(int length, EffectDirection direction)
        {
            if (direction == EffectDirection.Forward && this.currentFadeLevel == 1f || direction == EffectDirection.Backward && this.currentFadeLevel == 0f) return;

            isRunning = true;
            fadeDelta = 1.0f / length;
            dir = direction;
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
        public void Set(EffectDirection direction)
        {
            if (direction == EffectDirection.Forward)
            {
                currentFadeLevel = 1f;
            }
            else
            {
                currentFadeLevel = 0f;
            }
        }

        public void Update(GameTime gameTime)
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isInitialized)
            {
                spriteBatch.DrawRectangle(Vector2.Zero.ToRectangle(screensize), Color.Black * currentFadeLevel);
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
