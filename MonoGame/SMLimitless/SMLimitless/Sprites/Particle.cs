using System;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
	/// <summary>
	///   A small visual effect.
	/// </summary>
	public sealed class Particle
	{
		private const float MaxGravitationalVelocity = 350f;

		private Section owner;

		/// <summary>
		///   Gets the current acceleration of this particle, in pixels per
		///   second squared.
		/// </summary>
		public Vector2 Acceleration { get; private set; }

		/// <summary>
		///   Gets a value indicating whether this particle will fall.
		/// </summary>
		public bool AffectedByGravity { get; private set; }

		/// <summary>
		///   Gets the <see cref="IGraphicsObject" /> of this particle.
		/// </summary>
		public IGraphicsObject Graphics { get; private set; }

		/// <summary>
		///   Gets the lifespan of this particle, in seconds.
		/// </summary>
		public float Life { get; private set; }

		/// <summary>
		///   Gets the current position of this particle, in pixels.
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		///   Gets the current velocity of this particle, in pixels per second.
		/// </summary>
		public Vector2 Velocity { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="Particle" /> class.
		/// </summary>
		/// <param name="owner">The section this particle appears in.</param>
		/// <param name="graphicsResourceName">
		///   The name of the graphics resource to use for this particle.
		/// </param>
		/// <param name="position">The position of this particle onscreen.</param>
		/// <param name="velocity">The velocity of this particle.</param>
		/// <param name="affectedByGravity">
		///   A value indicating whether this particle will fall.
		/// </param>
		/// <param name="lifespan">The lifespan of this particle.</param>
		public Particle(Section owner, string graphicsResourceName, Vector2 position, Vector2 velocity, bool affectedByGravity = false, float lifespan = -1f)
		{
			// TODO: particles should be able to be used in more than just sections
			if (string.IsNullOrEmpty(graphicsResourceName)) { throw new ArgumentException("The graphics resource name for this particle was invalid."); }

			this.owner = owner;
			Graphics = ContentPackageManager.GetGraphicsResource(graphicsResourceName);
			Position = position;
			Velocity = velocity;
			AffectedByGravity = affectedByGravity;
			Life = lifespan;
		}

		/// <summary>
		///   Draws this particle to the screen.
		/// </summary>
		public void Draw()
		{
			Graphics.Draw(Position, Color.White);
		}

		/// <summary>
		///   Loads the content for the <see cref="Graphics" /> property.
		/// </summary>
		public void LoadContent()
		{
			Graphics.LoadContent();
		}

		/// <summary>
		///   Updates this particle.
		/// </summary>
		public void Update()
		{
			float delta = GameServices.GameTime.GetElapsedSeconds();
			if (Life >= 0f)
			{
				Life -= delta;

				if (Life <= 0f)
				{
					owner.RemoveParticleOnNextFrame(this);
				}
			}

			Graphics.Update();

			if (AffectedByGravity)
			{
				if (Velocity.Y < MaxGravitationalVelocity) { Acceleration = new Vector2(0f, Level.GravityAcceleration.Value); }
				else if (Velocity.Y >= MaxGravitationalVelocity)
				{
					Velocity = new Vector2(Velocity.X, MaxGravitationalVelocity);
					Acceleration = Vector2.Zero;
				}
			}

			Velocity += Acceleration * delta;
			Position += Velocity * delta;
		}
	}
}
