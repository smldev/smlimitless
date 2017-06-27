using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;

namespace SmlSprites.Players
{
	public class PlayerMarioSuper : PlayerMario
	{
		protected const int TextureWidth = 29;

		protected readonly Vector2 StandingHitboxSize = new Vector2(16f, 32f);
		protected readonly Vector2 DuckingHitboxSize = new Vector2(16f, 16f);

		protected static PhysicsSetting<float> SpinJumpWhileDuckingImpulseMultiplier =
			new PhysicsSetting<float>("Super Mario (SMB3): Spin Jump while Ducking Impulse Multiplier",
				0.0001f, 1f, 0.9f, PhysicsSettingType.FloatingPoint);

		private ComplexGraphicsObject graphics;

		internal virtual bool IsDucking { get; set; }

		protected override bool IsPlayerMoving
		{
			get
			{
				return base.IsPlayerMoving && (!IsDucking || !IsOnGround);
			}
		}

		public PlayerMarioSuper() : base()
		{
			Size = new Vector2(16f, 32f);
		}

		public override void Initialize(Section owner)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3PlayerMarioSuper");
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void Draw()
		{
			Vector2 drawPosition = new Vector2((int)(Position.X - ((TextureWidth / 2f) - (Hitbox.Width / 2f))), Position.Y);
			graphics.Draw(drawPosition.Floor(), Color.White,
			(FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		public override void Draw(Rectangle cropping)
		{
			graphics.Draw(Position.Floor(), cropping, Color.White,
			(FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		protected override void BaseUpdate()
		{
			base.BaseUpdate();
			graphics.Update();
		}

		protected override void SetPlayerGraphicsObject(string objectName)
		{
			graphics.CurrentObjectName = objectName;
		}

		internal override void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Owner.PerformPowerupStateChange(this, "SmlSprites.Players.PlayerMario",
				"SMB3PlayerMarioSmallToSuper", poweringUp: false);
		}

		// PlayerMario Behavior Overrides
		protected override void CheckForWalkRunInput()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if (isLeftDown || isRightDown) { IsSliding = false; }

			bool conflictingInput = (isLeftDown && isRightDown);
			bool playerNotMoving = (!isLeftDown && !isRightDown) && !IsSliding;
			bool duckingOnGround = IsDucking && IsOnGround;
			if (conflictingInput || playerNotMoving || duckingOnGround)
			{
				// If the user is holding both left and right down, we should cancel the acceleration and do nothing.
				CancelHorizontalAcceleration();
			}
			else if (isLeftDown)
			{
				float targetVelocityX = (SprintChargeTimer == FramesToSprintingAllowed.Value) ? -MaximumSprintingSpeed.Value : (IsRunning) ? -MaximumRunningSpeed.Value : -MaximumWalkingSpeed.Value;
				if (IsGroundPounding && GroundPoundSpinTimer >= GroundPoundSpinTimerSetting.Value) { targetVelocityX *= GroundPoundHorizontalMovementMultiplier.Value; }
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
				FacingDirection = SMLimitless.Direction.Left;
			}
			else if (isRightDown)
			{
				float targetVelocityX = (SprintChargeTimer == FramesToSprintingAllowed.Value) ? MaximumSprintingSpeed.Value : (IsRunning) ? MaximumRunningSpeed.Value : MaximumWalkingSpeed.Value;
				if (IsGroundPounding && GroundPoundSpinTimer >= GroundPoundSpinTimerSetting.Value) { targetVelocityX *= GroundPoundHorizontalMovementMultiplier.Value; }
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
				FacingDirection = SMLimitless.Direction.Right;
			}
		}

		protected override void CheckForSpinJumpInput()
		{
			base.CheckForSpinJumpInput();

			// If the player is spin jumping, two things will be true:
			// The IsSpinJumping flag is set, and
			// The player will have the upward spin jump impulse
			// Just multiply it by the impulse multiplier if we're ducking.
			if (IsDucking && IsSpinJumping)
			{
				Velocity = new Vector2(Velocity.X,
					Velocity.Y * SpinJumpWhileDuckingImpulseMultiplier.Value);
			}
		}

		protected override void CheckForSlideInput()
		{
			Vector2 tileBeneathPlayerCheckPoint = Hitbox.BottomCenter;
			tileBeneathPlayerCheckPoint.Y += 1f;
			Tile tileBeneathPlayer = Owner.GetTileAtPosition(tileBeneathPlayerCheckPoint);

			// Conditions for ducking
			bool alreadyDucking = IsDucking;
			bool notMovingVertically = (Velocity.Y == 0);
			bool tileBeneathIsPresent = (tileBeneathPlayer != null);
			bool tileBeneathIsRectangle = tileBeneathIsPresent &&
				(tileBeneathPlayer.TileShape == CollidableShape.Rectangle);
			bool canDuckOnTile = (tileBeneathIsPresent && tileBeneathIsRectangle);
			bool downPressed = InputManager.IsCurrentActionPress(InputAction.Down);

			if ((alreadyDucking && downPressed) ||
				(notMovingVertically || canDuckOnTile) && downPressed)
			{
				IsDucking = true;
				ChangePlayerHitboxOnDuck();
				IsSliding = false;
			}
			else
			{
				IsDucking = false;
				ChangePlayerHitboxOnDuck();
			}

			if (!IsSliding)
			{
				if ((InputManager.IsNewActionPress(InputAction.Down) && IsOnGround) || WasGroundPounding)
				{
					if (tileBeneathPlayer != null && tileBeneathPlayer.TileShape == CollidableShape.RightTriangle)
					{
						float slideDirection = (float)((RightTriangle)tileBeneathPlayer.Hitbox).HorizontalSlopedSide;
						if (WasGroundPounding)
						{
							Velocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
							TargetVelocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
						}
						else
						{
							TargetVelocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
						}
						IsSliding = true;
					}
				}
			}
			else
			{
				if (tileBeneathPlayer != null)
				{
					if (tileBeneathPlayer.TileShape == CollidableShape.Rectangle)
					{
						IsSliding = false;
						if (InputManager.IsCurrentActionPress(InputAction.Down))
						{
							IsDucking = true;
							ChangePlayerHitboxOnDuck();
						}
					}
					else
					{
						TargetVelocity = new Vector2((float)((RightTriangle)tileBeneathPlayer.Hitbox).HorizontalSlopedSide * SlidingVelocity.Value, Velocity.Y);
					}
				}
			}
		}

		private void ChangePlayerHitboxOnDuck()
		{
			if (IsDucking && Size.Y != 16f)
			{
				Size = DuckingHitboxSize;
				Position = new Vector2(Position.X, Position.Y + 16f);
			}
			else if (!IsDucking && Size.Y == 16f)
			{
				Size = StandingHitboxSize;
				Position = new Vector2(Position.X, Position.Y - 16f);
			}
		}

		protected override void DeterminePlayerGraphicsObject()
		{
			base.DeterminePlayerGraphicsObject();

			// We can check for ducking here, instead of modifying the base method.
			if (IsDucking && !OnLevelExit)
			{
				if (IsSpinJumping) { SetPlayerGraphicsObject("spinJump"); }
				else if (IsGroundPounding) { return; }
				else { SetPlayerGraphicsObject("ducking"); }
			}
			// Since this check always occurs last, it takes priority.
		}
	}
}
