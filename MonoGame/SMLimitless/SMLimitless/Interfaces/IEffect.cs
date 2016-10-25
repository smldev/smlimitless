//-----------------------------------------------------------------------
// <copyright file="IEffect.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Interfaces
{
    /// <summary>
    /// An event handler delegate for the effect completed event.
    /// </summary>
    /// <param name="sender">The object that raises this event.</param>
    /// <param name="direction">The "forward" or "backward" direction of the effect.</param>
    public delegate void EffectCompletedEventHandler(object sender, EffectCompletedEventArgs direction);

	public sealed class EffectCompletedEventArgs : EventArgs
	{
		public EffectDirection Direction { get; }

		public EffectCompletedEventArgs(EffectDirection direction)
		{
			Direction = direction;
		}
	}

    /// <summary>
    /// Defines a "forward" and "backward" direction for effects.
    /// Every effect may use this differently, but the general definition
    /// is that a forward effect changes something on the screen,
    /// and a backward effect undoes that change.
    /// </summary>
    public enum EffectDirection
    {
        /// <summary>
        /// The forward direction.
        /// </summary>
        Forward,

        /// <summary>
        /// The backward direction.
        /// </summary>
        Backward
    }
    
   /// <summary>
    /// Defines a visual effect.
    /// These can be used, for example, to switch between screens.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// An event fired when the effect is completed.
        /// </summary>
        event EffectCompletedEventHandler EffectCompletedEvent;

        /// <summary>
        /// Starts an effect.
        /// </summary>
        /// <param name="length">The length in frames that this effect will run for.</param>
        /// <param name="direction">An EffectDirection that defines whether the effect works "forward" or "backward.</param>
        /// <param name="position">The position on the screen to draw this effect.</param>
        /// <param name="color">The color of this effect.</param>
        void Start(int length, EffectDirection direction, Vector2 position, Color color);

        /// <summary>
        /// Stops an effect and removes all changes.
        /// </summary>
        void Stop();

        /// <summary>
        /// Loads the content for this effect.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Immediately sets an effect to its final "forward" or "backward" state.
        /// </summary>
        /// <param name="direction">The "forward" or "backward" direction.</param>
        /// <param name="color">The color of this effect.</param>
        void Set(EffectDirection direction, Color color);

        /// <summary>
        /// Updates this effect.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws this effect to the screen.
        /// </summary>
        void Draw();
    }
}
