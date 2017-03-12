using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Graphics;
using SMLimitless.Interfaces;

namespace SMLimitless.Screens.Effects
{
	/// <summary>
	///   An iris wipe effect.
	/// </summary>
	public sealed class IrisEffect : IEffect, IDisposable
	{
		private Color color;
		private float currentFadeLevel;
		private EffectDirection dir;
		private float fadeDelta;
		private float initialIrisRadius;
		private Vector2 irisCenter;
		private float irisRadius;
		private bool isInitialized;
		private bool isRunning;
		private QuadRenderer quadRenderer;

        /// <summary>
        /// Gets a value indicating whether the resources for this object have been released.
        /// </summary>
        public bool IsDisposed { get; private set; }

		/// <summary>
		///   An event raised when this effect has completed.
		/// </summary>
		public event EffectCompletedEventHandler EffectCompletedEvent;

		/// <summary>
		///   Initializes a new instance of the <see cref="IrisEffect" /> class.
		/// </summary>
		/// <param name="center">
		///   The point at which the iris should close to or open from.
		/// </param>
		public IrisEffect(Vector2 center)
		{
			irisCenter = center;
			currentFadeLevel = 1f;

			Vector2 screenSize = GameServices.ScreenSize;
			irisRadius = (float)Math.Sqrt((screenSize.X * screenSize.X) + (screenSize.Y + screenSize.Y));
			initialIrisRadius = irisRadius;

			isInitialized = true;
		}

		/// <summary>
		///   Draws this effect.
		/// </summary>
		public void Draw()
		{
			if (isInitialized && (isRunning || dir == EffectDirection.Backward))
			{
				//GameServices.SpriteBatch.End();

				//GameServices.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);
				float r = color.R / 255f;
				float g = color.G / 255f;
				float b = color.B / 255f;
				float a = (color.A * currentFadeLevel) / 255f;

				var irisEffect = GameServices.Effects["IrisEffect"];
				irisEffect.Parameters["irisCenter"].SetValue(irisCenter);
				irisEffect.Parameters["radius"].SetValue(irisRadius);
				irisEffect.Parameters["backColor"].SetValue(new Vector4(r, g, b, a));

				quadRenderer.Render(irisEffect, GameServices.Camera.Position);
			}
		}

		/// <summary>
		///   Loads the content for this effect.
		/// </summary>
		public void LoadContent()
		{
			if (!GameServices.Effects.ContainsKey("IrisEffect"))
			{ GameServices.Effects.Add("IrisEffect", GameServices.GetService<ContentManager>().Load<Effect>("IrisEffect")); }
			quadRenderer = new QuadRenderer();
		}

		/// <summary>
		///   Sets this effect to be completed in any direction.
		/// </summary>
		/// <param name="direction">The direction to be set to.</param>
		/// <param name="color">The color of the effect.</param>
		public void Set(EffectDirection direction, Color color)
		{
			this.color = color;
			if (direction == EffectDirection.Forward)
			{
				currentFadeLevel = 0f;
			}
			else
			{
				currentFadeLevel = 1f;
			}
		}

		/// <summary>
		///   Starts this effect.
		/// </summary>
		/// <param name="length">How many frames this effect should last.</param>
		/// <param name="direction">
		///   The direction in which to run the effect (forward is iris in,
		///   backwards is iris out).
		/// </param>
		/// <param name="position">
		///   The position at which the iris should close/open.
		/// </param>
		/// <param name="color">The color of the background after the iris-in.</param>
		public void Start(int length, EffectDirection direction, Vector2 position, Color color)
		{
			if ((direction == EffectDirection.Backward && currentFadeLevel == 1f) || (direction == EffectDirection.Forward && currentFadeLevel == 0f))
			{
				return;
			}

			isRunning = true;
			fadeDelta = 1.0f / length;
			dir = direction;
			this.color = color;

			if (dir == EffectDirection.Forward)
			{
				currentFadeLevel = 1.0f;
			}
		}

		/// <summary>
		///   Stops the effect, filling the screen with the color specified in
		///   the <see cref="Start(int, EffectDirection, Vector2, Color)" /> method.
		/// </summary>
		public void Stop()
		{
			isRunning = false;
			currentFadeLevel = 1f;
			fadeDelta = 0f;
		}

		/// <summary>
		///   Updates this effect.
		/// </summary>
		public void Update()
		{
			if (isRunning && isInitialized)
			{
				switch (dir)
				{
					case EffectDirection.Forward:
						// Fade from black to transparent, iris out
						if (currentFadeLevel > 0f)
						{
							currentFadeLevel -= fadeDelta;
							irisRadius = initialIrisRadius * (1 - currentFadeLevel);
						}
						else FadeFinished();
						break;
					case EffectDirection.Backward:
						// Fade to black from transparent, iris in
						if (currentFadeLevel < 1f)
						{
							currentFadeLevel += fadeDelta;
							irisRadius = initialIrisRadius * (1 - currentFadeLevel);
						}
						else FadeFinished();
						break;
				}
			}
			else if (!isInitialized)
			{
				throw new InvalidOperationException("The iris effect was not properly initialized. Please set the screen size.");
			}
		}

		private void FadeFinished()
		{
			isRunning = false;
			fadeDelta = 0f;
			EffectCompletedEvent?.Invoke(this, new EffectCompletedEventArgs(dir));
		}

        private void Dispose(bool disposing)
        {
            if (IsDisposed) { return; }

            if (disposing)
            {
                if (quadRenderer != null && !quadRenderer.IsDisposed)
                {
                    quadRenderer.Dispose();
                }
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Releases resources used by this <see cref="IrisEffect"/> instance. 
        /// </summary>
        public void Dispose() => Dispose(true);
	}
}
