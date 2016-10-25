using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sounds;
using SMLimitless.Sprites.Components;

namespace SmlSample
{
	public sealed class SimplePlayer : Sprite
	{
		private StaticGraphicsObject graphics;
		private int jumpTimeout = 5;
		private CachedSound jumpSound;

		public override string EditorCategory
		{
			get
			{
				return "Test Item";
			}
		}

		public SimplePlayer()
		{
			Size = new Vector2(16f);

			jumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiJump"));
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);

			Components.Add(new HealthComponent(1, 1,  new string[] { }));
			((HealthComponent)Components[0]).SpriteKilled += (sender, e) => { RemoveOnNextFrame = true; };
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void Draw()
		{
			SpriteEffects effects = (Direction == SpriteDirection.Right) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			graphics.Draw(new Vector2(Position.X, Position.Y - 16f), Color.White, effects);
		}

		public override void LoadContent()
		{
			graphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("simple_player");
			graphics.LoadContent();

			
		}

		public override void Update()
		{
			const float Friction = 0.08f;
			const float AccelerationImpulse = 5f;
			const float JumpImpulse = 50f;
			const float MaxJumpVelocity = -150f;
			const int MaxJumpTimeout = 25;

			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if (Velocity.X != 0f && (!isLeftDown && !isRightDown))
			{
				if (Velocity.X > 0.5f)
				{
					Velocity = new Vector2(Velocity.X - (Velocity.X * Friction), Velocity.Y);
				}
				else
				{
					Velocity = new Vector2(0f, Velocity.Y);
				}
			}

			if (isLeftDown)
			{
				Direction = SpriteDirection.Left;
				AdjustVelocity(-AccelerationImpulse);
			}
			else if (isRightDown)
			{
				Direction = SpriteDirection.Right;
				AdjustVelocity(AccelerationImpulse);
			}

			if (InputManager.IsCurrentActionPress(InputAction.Jump) && jumpTimeout > 0)
			{
				Velocity = new Vector2(Velocity.X, (Velocity.Y <= MaxJumpVelocity) ? MaxJumpVelocity : Velocity.Y - JumpImpulse);
				jumpTimeout--;
			}

			if (Velocity.Y == 0f && jumpTimeout == 0)
			{
				jumpTimeout = MaxJumpTimeout;
			}

			base.Update();
		}

		private void AdjustVelocity(float amount)
		{
			const float MaxVelocity = 150f;
			if (Math.Abs(Velocity.X + amount) > MaxVelocity)
			{
				Velocity = new Vector2((Velocity.X >= 0) ? MaxVelocity : -MaxVelocity, Velocity.Y);
			}
			else
			{
				Velocity = new Vector2(Velocity.X + amount, Velocity.Y);
			}
		}

		public override object GetCustomSerializableObjects() { return null; }

		public override void Draw(Rectangle cropping)
		{
			Draw();
		}
	}
}
