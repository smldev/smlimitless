using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Sounds;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;
using SmlSprites.Players.Projectiles;

namespace SmlSprites.Players
{
	public sealed class PlayerMarioFire : PlayerMarioSuper
	{
		private const int MaximumFireballs = 2;

		private const float FireballSpawnY = 16f;

		private static PhysicsSetting<int> FireballthrowAnimationTimer = new PhysicsSetting<int>(
			"SMB3 Fire Mario: Fireball throw Animation Timer", 0, 60, 5, PhysicsSettingType.Integer);
		private static PhysicsSetting<int> FramesBetweenFireballThrowsWhenSpinJumping = new PhysicsSetting<int>(
			"SMB3 Fire Mario: Frames between Fireball Throws When Spin Jumping", 1, 60, 5, PhysicsSettingType.Integer);
		
		private ComplexGraphicsObject graphics;
		private CachedSound throwFireballSound;

		private bool isThrowingFireball;
		private int fireballThrowTimer;
		private int spinJumpFireballThrowTimer;
		private int fireballsThrown;

		public PlayerMarioFire() : base()
		{
			Size = new Vector2(16f, 32f);
		}

		public override void Initialize(Section owner)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3PlayerMarioFire");
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
			throwFireballSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiFireBall"));
		}

		public override void Draw()
		{
			Vector2 drawPosition = new Vector2((int)(Position.X - ((TextureWidth / 2f) - (Hitbox.Width / 2f))), Position.Y);
			graphics.Draw(drawPosition.Floor(), Color.White,
				(FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		protected override void BaseUpdate()
		{
			base.BaseUpdate();
			graphics.Update();

			if (fireballThrowTimer > 0)
			{
				fireballThrowTimer--;
			}

			if (IsSpinJumping)
			{
				if (spinJumpFireballThrowTimer == 0)
				{
					var direction = (GameServices.Random.Next(0, 2) == 0) ?
						SMLimitless.Direction.Left : SMLimitless.Direction.Right;
					ThrowFireball(direction);
					spinJumpFireballThrowTimer = FramesBetweenFireballThrowsWhenSpinJumping.Value;
				}
				else
				{
					spinJumpFireballThrowTimer--;
				}
			}
		}

		protected override void SetPlayerGraphicsObject(string objectName)
		{
			graphics.CurrentObjectName = objectName;
		}

		internal override void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Owner.PerformPowerupStateChange(this, "SmlSprites.Players.PlayerMarioSuper",
				"SMB3PlayerMarioSuperToFire", poweringUp: false);
		}

		private void ThrowFireball(SMLimitless.Direction direction)
		{
			if (fireballsThrown >= MaximumFireballs || IsDucking || IsGroundPounding
				|| IsDead || IsSlidingDownWall) { return; }

			// Set the animation timer and fireball throwing flag.
			fireballThrowTimer = FireballthrowAnimationTimer.Value;
			isThrowingFireball = true;
			fireballsThrown++;

			// Create a new fireball.
			var fireball = new PlayerFireball(direction);
			fireball.Initialize(Owner);
			fireball.LoadContent();
			
			// Set the fireball's position.
			fireball.Position = new Vector2(
				(FacingDirection == SMLimitless.Direction.Left) ? Hitbox.Left : Hitbox.Right,
				Hitbox.Top + FireballSpawnY);

			fireball.FireballDestroyed += (sender, e) => { fireballsThrown--; };
			AudioPlaybackEngine.Instance.PlaySound(throwFireballSound, (sender, e) => { });
			Owner.AddSpriteOnNextFrame(fireball);
		}

		protected override void CheckForWalkRunInput()
		{
			base.CheckForWalkRunInput();

			bool isRunDown = InputManager.IsNewActionPress(InputAction.Run) ||
				InputManager.IsNewActionPress(InputAction.AltRun);
			
			if (isRunDown)
			{
				ThrowFireball(FacingDirection);
			}
		}

		protected override void DeterminePlayerGraphicsObject()
		{
			base.DeterminePlayerGraphicsObject();

			if (fireballThrowTimer > 0 && !OnLevelExit)
			{
				SetPlayerGraphicsObject("throwFireball");
			}
		}
	}
}
