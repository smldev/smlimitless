using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using SmlEngine;

namespace SmlEngine.Effects
{
    /// <summary>
    /// Defines a visual effect.
    /// These can be used, for example, to switch between screens.
    /// </summary>
    public interface IEffect
    {
        event EffectCompletedEventHandler effectCompletedEvent;
        void Start(int length, EffectDirection direction);
        void Stop();
        void LoadContent(ContentManager Content);

        /// <summary>
        /// Sets the screen size and visible drawing area for this effect.
        /// </summary>
        /// <param name="screenSize">The size of the screen in pixels.</param>
        void SetScreenSize(Vector2 screenSize);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }

    public delegate void EffectCompletedEventHandler(object sender, EffectDirection direction);

    public enum EffectDirection
    {
        Forward,
        Backward
    }
}
