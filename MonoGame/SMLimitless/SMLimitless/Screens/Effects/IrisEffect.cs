using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Interfaces;

namespace SMLimitless.Screens.Effects
{
	public sealed class IrisEffect : IEffect
	{
		private float fadeDelta;
		private float currentFadeLevel;
		private Vector2 irisCenter;
		private float initialIrisRadius;
		private float irisRadius;
		private bool isRunning;
		private bool isInitialized;
		private EffectDirection dir;
		private Color color;
		private QuadRenderer quadRenderer;

		public event EffectCompletedEventHandler EffectCompletedEvent;

		public IrisEffect(Vector2 center)
		{
			irisCenter = center;
			currentFadeLevel = 1f;

			Vector2 screenSize = GameServices.ScreenSize;
			irisRadius = (float)Math.Sqrt((screenSize.X * screenSize.X) + (screenSize.Y + screenSize.Y));
			initialIrisRadius = irisRadius;

			isInitialized = true;
		}
		
		public void Draw()
		{
			if (isInitialized && (isRunning || dir == EffectDirection.Backward))
			{
				//GameServices.SpriteBatch.End();

				//GameServices.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);
				float r = color.R / 255f;
				float g = color.G / 255f;
				float b = color.B/ 255f;
				float a = (color.A * currentFadeLevel) / 255f;

				var irisEffect = GameServices.Effects["IrisEffect"];
				irisEffect.Parameters["irisCenter"].SetValue(irisCenter);
				irisEffect.Parameters["radius"].SetValue(irisRadius);
				irisEffect.Parameters["backColor"].SetValue(new Vector4(r, g, b, a));

				quadRenderer.Render(irisEffect);
			}
		}

		public void LoadContent()
		{
			GameServices.Effects.Add("IrisEffect", GameServices.GetService<ContentManager>().Load<Effect>("IrisEffect"));
			quadRenderer = new QuadRenderer();
			
		}

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

		public void Stop()
		{
			isRunning = false;
			currentFadeLevel = 1f;
			fadeDelta = 0f;
		}

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
			if (EffectCompletedEvent != null)
			{
				EffectCompletedEvent(this, dir);
			}
		}
	}
}
