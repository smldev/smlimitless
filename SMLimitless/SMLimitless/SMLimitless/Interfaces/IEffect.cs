using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace SMLimitless.Interfaces
{
    /// <summary>
    /// Defines a visual effect.
    /// These can be used, for example, to switch between screens.
    /// </summary>
    public interface IEffect
    {
        event EffectCompletedEventHandler effectCompletedEvent;
        void Start(int length, EffectDirection direction, Vector2 position, Color color);
        void Stop();
        void LoadContent();
        void Set(EffectDirection direction, Color color);
        void Update();
        void Draw();
    }

    public delegate void EffectCompletedEventHandler(object sender, EffectDirection direction);

    public enum EffectDirection
    {
        Forward,
        Backward
    }
}
