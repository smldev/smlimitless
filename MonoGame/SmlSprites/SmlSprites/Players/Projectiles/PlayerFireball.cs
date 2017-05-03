using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSprites.Players.Projectiles
{
	public sealed class PlayerFireball : Sprite
	{
		private static PhysicsSetting<float> XVelocity = new PhysicsSetting<float>(
			"Player Fireball X Velocity (px/sec)", 0.1f, 250f, 150f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> GroundBounceImpulse = new PhysicsSetting<float>(
			"Player Fireball Ground Bounce Impulse", 0.1f, 250f, 100f, PhysicsSettingType.FloatingPoint);

		private ComplexGraphicsObject graphics;
		private float xVelocity;

		public event EventHandler FireballDestroyed;

		public override string EditorCategory => "Projectiles";

		public PlayerFireball(Direction moveDirection)
		{
			Size = new Vector2(8f);
			Velocity = new Vector2((moveDirection == SMLimitless.Direction.Left) ? -XVelocity.Value :
				XVelocity.Value, 0f);
			xVelocity = Velocity.X;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void Initialize(Section owner)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("PlayerMarioFireball");
			Components.Add(new DamageComponent());
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void Draw()
		{
			Vector2 drawPoint = new Vector2(Position.X, Position.Y - 1f);
			graphics.Draw(drawPoint, Color.White);
		}

		public override void Draw(Rectangle cropping)
		{
			Vector2 drawPoint = new Vector2(Position.X, Position.Y - 1f);
			graphics.Draw(drawPoint, cropping, Color.White, SpriteEffects.None);
		}

		public override object GetCustomSerializableObjects()
		{
			return new { };
		}

		public override void Update()
		{
			// Ensure that the fireball is always moving forward. Some collisions may not destroy
			// the fireball but will do a horizontal collision resolution.
			Velocity = new Vector2(xVelocity, Velocity.Y);
			graphics.Update();
			base.Update();
		}

		public override void HandleTileCollision(Tile tile, Vector2 resolutionDistance)
		{
			bool shouldBounce = false;
		
			if (tile.Hitbox is BoundingRectangle)
			{
				var collidingEdges = tile.Hitbox.Bounds.WhichEdgeIsWithin(Hitbox);

				if (collidingEdges.HasFlag(FlaggedDirection.Up))
				{
					// If the fireball hits the top edge, even if it hits any other edge, it should
					// bounce off the top.
					shouldBounce = true;
				}
				else
				{
					// If the fireball is not colliding with the top edge, it must be colliding with
					// some other edge, since we're in the tile collision handler method.
					shouldBounce = false;
				}
			}
			else
			{
				if (resolutionDistance.Y < 0f)
				{
					shouldBounce = true;
				}
			}

			if (shouldBounce)
			{
				Velocity = new Vector2(Velocity.X, -GroundBounceImpulse.Value);
			}
			else
			{
				OnFireballDestroyed();
				Owner.RemoveSpriteOnNextFrame(this);
			}
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer) { return; }
			var damageComponent = GetComponent<DamageComponent>();
			damageComponent.PerformDamage(sprite, SpriteDamageTypes.PlayerFireball, 1);
			OnFireballDestroyed();
			Owner.RemoveSpriteOnNextFrame(this);
		}

		private void OnFireballDestroyed()
		{
			FireballDestroyed?.Invoke(this, new EventArgs());
		}
	}
}
