using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
	public sealed class Particle
	{
		private const float MaxGravitationalVelocity = 350f;
		
		private Section owner;	

		public Vector2 Position { get; private set; }
		public Vector2 Velocity { get; private set; }
		public Vector2 Acceleration { get; private set; }
		public bool AffectedByGravity { get; private set; }
		public float Life { get; private set; }
		public IGraphicsObject Graphics { get; private set; }

		public Particle(Section owner, string graphicsResourceName, Vector2 position, Vector2 velocity, bool affectedByGravity = false, float lifespan = -1f)
		{
			if (string.IsNullOrEmpty(graphicsResourceName)) { throw new ArgumentException("The graphics resource name for this particle was invalid."); }

			this.owner = owner;
			Graphics = ContentPackageManager.GetGraphicsResource(graphicsResourceName);
			Position = position;
			Velocity = velocity;
			AffectedByGravity = affectedByGravity;
			Life = lifespan;
		}

		public void LoadContent()
		{
			Graphics.LoadContent();
		}

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

		public void Draw()
		{
			Graphics.Draw(Position, Color.White);
		}
	}
}
