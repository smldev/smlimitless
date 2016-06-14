using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Components;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sounds;
using SMLimitless.Sprites.Components;

namespace SmlSprites.Players
{
	public class PlayerMario : Sprite
	{
		private ComplexGraphicsObject graphics;
		private int sprintChargeTimer = 0;
		private Direction direction = SMLimitless.Direction.Left;
		private ActionScheduler actionScheduler = new ActionScheduler();

		private bool isDead;
		private bool isGroundPounding;
		private bool wasGroundPounding { get; set; }
		private int groundPoundSpinTimer = 0;
		private bool perfomingInAirSpin;
		private int inAirSpinTimer = 0;
		private int inAirSpinTimeout = 0;
		private bool isSliding;

		private static PhysicsSetting<float> MaximumWalkingSpeed = new PhysicsSetting<float>("Small Mario: Full Walking Speed (px/sec)", 0f, 100f, 50f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumRunningSpeed = new PhysicsSetting<float>("Small Mario: Full Running Speed (px/sec)", 0f, 150f, 75f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumSprintingSpeed = new PhysicsSetting<float>("Small Mario: Max Sprinting Speed (px/sec)", 0f, 200f, 100f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MovementAcceleration = new PhysicsSetting<float>("Small Mario: Acceleration (px/sec²)", 0f, 400f, 200f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<int> FramesToSprintingAllowed = new PhysicsSetting<int>("Small Mario: Frames to Sprinting Allowed", 1, 120, 60, PhysicsSettingType.Integer);

		private static PhysicsSetting<float> JumpImpulse = new PhysicsSetting<float>("Small Mario: Jump Impulse (px/sec²)", 0f, 500f, 245f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumJumpImpulseAddend = new PhysicsSetting<float>("Small Mario: Maximum Additional Jump Impulse (px/sec²)", 0f, 200f, 60f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> JumpGravityMultiplier = new PhysicsSetting<float>("Small Mario: Jump Gravity Multiplier", 0.01f, 1f, 0.65f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> SpinJumpImpulse = new PhysicsSetting<float>("Small Mario: Spin Jump Impulse (px/sec²)", 0f, 500f, 245f, PhysicsSettingType.FloatingPoint);

		private static PhysicsSetting<int> GroundPoundSpinTimer = new PhysicsSetting<int>("Small Mario: Ground Pound Spin Length (frames)", 1, 50, 20, PhysicsSettingType.Integer);
		private static PhysicsSetting<float> GroundPoundVelocity = new PhysicsSetting<float>("Small Mario: Ground Pound Velocity (px/sec)", 0f, 500f, 300f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> GroundPoundHorizontalMovementMultiplier = new PhysicsSetting<float>("Small Mario: Ground Pound Horizontal Speed Multiplier", 0.001f, 1f, 0.25f, PhysicsSettingType.FloatingPoint);

		private static PhysicsSetting<int> InAirSpinDuration = new PhysicsSetting<int>("Small Mario: In-Air Spin Length (frames)", 1, 100, 20, PhysicsSettingType.Integer);
		private static PhysicsSetting<float> InAirSpinGravityMultiplier = new PhysicsSetting<float>("Small Mario: In-Air Spin Gravity Multiplier", 0.001f, 1.0f, 0.6f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<int> InAirSpinTimeout = new PhysicsSetting<int>("Small Mario: In-Air Spin Timeout (frames)", 1, 100, 15, PhysicsSettingType.Integer);

		private static PhysicsSetting<float> SlidingVelocity = new PhysicsSetting<float>("Small Mario: Slide Down Slope Velocity (px/sec)", 10f, 400f, 100f, PhysicsSettingType.FloatingPoint);

		private static PhysicsSetting<float> SlideDownWallVelocity = new PhysicsSetting<float>("Small Mario: Slide Down Wall Velocity (px/sec)", 10f, 150f, 75f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> WallJumpVerticalImpulse = new PhysicsSetting<float>("Small Mario: Wall Jump Vertical Impulse (px/sec²)", 1f, 500f, 200f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> WallJumpHorizontalImpulse = new PhysicsSetting<float>("Small Mario: Wall Jump Horizontal Impulse (px/sec²)", 1f, 500f, 100f, PhysicsSettingType.FloatingPoint);

		public string DebugGraphicsName { get; protected set; } = "";
		protected virtual Vector2 TargetVelocity { get; set; }
		protected virtual bool IsJumping { get; set; }
		protected virtual bool IsSpinJumping { get; set; }

		private CachedSound jumpSound;
		private CachedSound spinJumpSound;
		private CachedSound groundPoundDropSound;
		private CachedSound groundPoundHitSound;
		private CachedSound inAirSpinSound;
		private CachedSound wallJumpSound;

		public override string EditorCategory
		{
			get
			{
				return "Players";
			}
		}

		protected virtual bool IsPlayerMoving
		{
			get
			{
				return InputManager.IsCurrentActionPress(InputAction.Left) ^ InputManager.IsCurrentActionPress(InputAction.Right);
			}
		}

		protected virtual bool IsRunning
		{
			get
			{
				return InputManager.IsCurrentActionPress(InputAction.Run) || InputManager.IsCurrentActionPress(InputAction.AltRun);
			}
		}

		protected virtual Tile WallBesidePlayer
		{
			get
			{
				Vector2 checkPoint = new Vector2(0f, Hitbox.Top);
				if (FacingDirection == SMLimitless.Direction.Left) { checkPoint.X = Hitbox.Left - 1f; }
				else if (FacingDirection == SMLimitless.Direction.Right) { checkPoint.X = Hitbox.Right + 1f; }

				Tile tile = Owner.GetTileAtPosition(checkPoint);
				if (tile == null) { return null; }
				else
				{
					if (tile.TileShape == CollidableShape.Rectangle) { return tile; }
					else
					{
						int expectedDirection = Math.Sign(tile.Position.X - Position.X); // -1 for tiles to our left, +1 for tiles to our tight
						if ((int)((RightTriangle)tile.Hitbox).HorizontalSlopedSide == expectedDirection) { return tile; }
						return null;
					}
				}
			}
		}

		protected virtual bool IsSlidingDownWall
		{
			get
			{
				Tile wallBesidePlayer = WallBesidePlayer;
				if (wallBesidePlayer != null)
				{
					HorizontalDirection tileBesideDirection = (HorizontalDirection)Math.Sign(wallBesidePlayer.Position.X - Position.X);
					if (tileBesideDirection == HorizontalDirection.Left) { return InputManager.IsCurrentActionPress(InputAction.Left); }
					else if (tileBesideDirection == HorizontalDirection.Right) { return InputManager.IsCurrentActionPress(InputAction.Right); }
					else { throw new InvalidOperationException($"{tileBesideDirection}"); }
				}
				return false;
			}
		}

		protected virtual Direction FacingDirection
		{
			get
			{
				//direction = (Velocity.X < 0f) ? Direction.Left : (Velocity.X == 0f) ? direction : Direction.Right;
				return direction;
			}
		}

		public PlayerMario() : base()
		{
			Size = new Vector2(16f);
			IsActive = true;

			InitializeSounds();

			HealthComponent healthComponent = new HealthComponent(1, 1, new string[] { });
			healthComponent.SpriteKilled += HealthComponent_SpriteKilled;

			Components.Add(healthComponent);
			Components.Add(new DamageComponent());
		}

		private void HealthComponent_SpriteKilled(object sender, SpriteDamagedEventArgs e)
		{
			Owner.PlayerKilled(this);
			isDead = true;
			Velocity = new Vector2(0f, -100f);
			TileCollisionMode = SpriteCollisionMode.NoCollision;
			SpriteCollisionMode = SpriteCollisionMode.NoCollision;
		}

		public override void Draw()
		{
			graphics.Draw(Position.Floor(), Color.White, (FacingDirection == SMLimitless.Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		public override void Update()
		{
			if (!isDead)
			{
				CheckForWalkRunInput();
				SprintIfAllowed();

				CheckForJumpInput();
				CheckForSpinJumpInput();
				CheckForGroundPoundInput();
				CheckForInAirSpinInput();
				CheckForSlideInput();
				DetermineHorizontalAcceleration();

				ApplyTileSurfaceFriction();
			}

			DeterminePlayerGraphicsObject();
			BaseUpdate();
		}

		protected virtual void BaseUpdate()
		{
			float delta = GameServices.GameTime.GetElapsedSeconds();

			Components.ForEach(c => c.Update());

			PreviousPosition = Position;

			if (IsEmbedded)
			{
				Acceleration = Vector2.Zero;
				Velocity = new Vector2(-25f, 0f);
			}
			else
			{
				if (InputManager.IsCurrentActionPress(InputAction.Jump) || InputManager.IsCurrentActionPress(InputAction.SpinJump) && IsJumping)
				{
					Acceleration = new Vector2(Acceleration.X, Level.GravityAcceleration.Value * JumpGravityMultiplier.Value);
				}
				else if (isGroundPounding && groundPoundSpinTimer < GroundPoundSpinTimer.Value)
				{
					Acceleration = Vector2.Zero;
				}
				else
				{
					Acceleration = new Vector2(Acceleration.X, Level.GravityAcceleration.Value);
				}

				if (isGroundPounding) { TargetVelocity = new Vector2(TargetVelocity.X, GroundPoundVelocity.Value); }
				else if (IsSlidingDownWall) { TargetVelocity = new Vector2(TargetVelocity.X, SlideDownWallVelocity.Value); }
				else { TargetVelocity = new Vector2(TargetVelocity.X, MaximumGravitationalVelocity.Value); }
			}

			ApplyAccelerationToVelocity(delta);
			graphics.Update();
			actionScheduler.Update();
		}

		protected virtual void ApplyAccelerationToVelocity(float delta)
		{
			Vector2 velocityAddend = Acceleration * delta;
			Vector2 resultVelocity = Vector2.Zero;
			bool xVelocityDiminishingTowardZero = Math.Abs(Velocity.X + velocityAddend.X) < Math.Abs(Velocity.X);   // is the sprite's velocity reducing?
			bool yVelocityDiminishingTowardZero = Math.Abs(Velocity.Y + velocityAddend.Y) < Math.Abs(Velocity.Y);

			// X-axis checks
			if (TargetVelocity.X < 0f)
			{
				// Assume that TargetVelocity and velocityDelta have the same sign
				if (Velocity.X + velocityAddend.X >= TargetVelocity.X) { resultVelocity.X = Velocity.X + velocityAddend.X; }
				else if ((Velocity.X > TargetVelocity.X) && (Velocity.X + velocityAddend.X < TargetVelocity.X)) { resultVelocity.X = TargetVelocity.X; }
				else if (Velocity.X <= TargetVelocity.X) { resultVelocity.X = Velocity.X; }
			}
			else if (TargetVelocity.X > 0f)
			{
				if (Velocity.X + velocityAddend.X <= TargetVelocity.X) { resultVelocity.X = Velocity.X + velocityAddend.X; }
				else if ((Velocity.X < TargetVelocity.X) && (Velocity.X + velocityAddend.X > TargetVelocity.X)) { resultVelocity.X = TargetVelocity.X; }
				else if (Velocity.X >= TargetVelocity.X) { resultVelocity.X = Velocity.X; }
			}
			else if (TargetVelocity.X == 0f)
			{
				if (Velocity.X > 0f && (Velocity.X + velocityAddend.X < 0f)) { resultVelocity.X = 0f; }
				else if (Velocity.X < 0f && (Velocity.X - velocityAddend.X > 0f)) { resultVelocity.X = 0f; }
				else { resultVelocity.X = Velocity.X + velocityAddend.X; }
			}

			// Y-axis checks
			if (TargetVelocity.Y < 0f)
			{
				if (Velocity.Y + velocityAddend.Y >= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y + velocityAddend.Y; }
				else if ((Velocity.Y > TargetVelocity.Y) && (Velocity.Y + velocityAddend.Y < TargetVelocity.Y)) { resultVelocity.Y = TargetVelocity.Y; }
				else if (Velocity.Y <= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y; }
			}
			else if (TargetVelocity.Y > 0f)
			{
				if (Velocity.Y + velocityAddend.Y <= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y + velocityAddend.Y; }
				else if ((Velocity.Y < TargetVelocity.Y) && (Velocity.Y + velocityAddend.Y > TargetVelocity.Y)) { resultVelocity.Y = TargetVelocity.Y; }
				else if (Velocity.Y >= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y; }
			}
			else if (TargetVelocity.Y == 0f)
			{
				if (Velocity.Y > 0f && (Velocity.Y + velocityAddend.Y < 0f)) { resultVelocity.Y = 0f; }
				else if (Velocity.Y < 0f && (Velocity.Y - velocityAddend.Y > 0f)) { resultVelocity.Y = 0f; }
				else { resultVelocity.Y = Velocity.Y + velocityAddend.Y; }
			}

			if (!perfomingInAirSpin) { Velocity = resultVelocity; }
			else { Velocity = new Vector2(resultVelocity.X, resultVelocity.Y * InAirSpinGravityMultiplier.Value); }
		}

		protected virtual void CheckForWalkRunInput()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if (isLeftDown || isRightDown) { isSliding = false; }

			if ((isLeftDown && isRightDown) || (!isLeftDown && !isRightDown) && !isSliding)
			{
				// If the user is holding both left and right down, we should cancel the acceleration and do nothing.
				CancelHorizontalAcceleration();
			}
			else if (isLeftDown)
			{
				float targetVelocityX = (sprintChargeTimer == FramesToSprintingAllowed.Value) ? -MaximumSprintingSpeed.Value : (IsRunning) ? -MaximumRunningSpeed.Value : -MaximumWalkingSpeed.Value;
				if (isGroundPounding && groundPoundSpinTimer >= GroundPoundSpinTimer.Value) { targetVelocityX *= GroundPoundHorizontalMovementMultiplier.Value; }
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
				direction = SMLimitless.Direction.Left;
			}
			else if (isRightDown)
			{
				float targetVelocityX = (sprintChargeTimer == FramesToSprintingAllowed.Value) ? MaximumSprintingSpeed.Value : (IsRunning) ? MaximumRunningSpeed.Value : MaximumWalkingSpeed.Value;
				if (isGroundPounding && groundPoundSpinTimer >= GroundPoundSpinTimer.Value) { targetVelocityX *= GroundPoundHorizontalMovementMultiplier.Value; }
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
				direction = SMLimitless.Direction.Right;
			}
		}

		protected virtual void DetermineHorizontalAcceleration()
		{
			if (TargetVelocity.X < 0f)
			{
				if (Velocity.X <= TargetVelocity.X) { Acceleration = new Vector2(0f, Acceleration.Y); }
				else { Acceleration = new Vector2(-MovementAcceleration.Value, Acceleration.Y); }
			}
			else if (TargetVelocity.X >= 0f)
			{
				if (Velocity.X >= TargetVelocity.X) { Acceleration = new Vector2(0f, Acceleration.Y); }
				else { Acceleration = new Vector2(MovementAcceleration.Value, Acceleration.Y); }
			}
		}

		protected virtual void SprintIfAllowed()
		{
			if (Math.Abs(Velocity.X) >= MaximumWalkingSpeed.Value && IsPlayerMoving && IsOnGround)
			{
				if (sprintChargeTimer < FramesToSprintingAllowed.Value)
				{
					sprintChargeTimer++;
				}
				else
				{
					if (Math.Abs(TargetVelocity.X) < MaximumSprintingSpeed.Value)
					{
						TargetVelocity = new Vector2((Velocity.X > 0f) ? MaximumSprintingSpeed.Value : -MaximumSprintingSpeed.Value, TargetVelocity.Y);
					}
				}
			}
			else if (IsOnGround && Math.Abs(Velocity.X) < MaximumWalkingSpeed.Value && sprintChargeTimer != 0)
			{
				sprintChargeTimer--;
			}
		}

		protected virtual void CancelHorizontalAcceleration()
		{
			Acceleration = new Vector2(0f, Acceleration.Y);
			TargetVelocity = new Vector2(0f, TargetVelocity.Y);
		}

		protected virtual void ApplyTileSurfaceFriction()
		{
			if (isSliding) { return; }

			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			Tile tileBeneathPlayer = GetTileBeneathPlayer();
			if (tileBeneathPlayer == null) { return; }

			if (!IsPlayerMoving || (IsPlayerMoving && !IsRunning && Math.Abs(Velocity.X) > MaximumWalkingSpeed.Value))
			{
				float tileSurfaceFrictionDelta = tileBeneathPlayer.SurfaceFriction * GameServices.GameTime.GetElapsedSeconds();

				if (Velocity.X > -0.5f && Velocity.X < 0.5f)
				{
					Velocity = new Vector2(0f, Velocity.Y);
				}
				else if (Velocity.X < -0.01f)
				{
					Acceleration = new Vector2(tileSurfaceFrictionDelta, Velocity.Y);
				}
				else if (Velocity.X > 0.01f)
				{
					Acceleration = new Vector2(-tileSurfaceFrictionDelta, Velocity.Y);
				}
			}
		}

		protected virtual void CheckForJumpInput()
		{
			bool isNewJumpPress = InputManager.IsNewActionPress(InputAction.Jump);

			if (isNewJumpPress && IsOnGround && !IsJumping && !IsSpinJumping)
			{
				Velocity = new Vector2(Velocity.X, -GetJumpImpulse());
				IsJumping = true;
				isSliding = false;
				PlaySound(jumpSound, (sender, e) => { });
			}
			else if (isNewJumpPress && IsSlidingDownWall && !IsSpinJumping)
			{
				Velocity = new Vector2((FacingDirection == SMLimitless.Direction.Right) ? -WallJumpHorizontalImpulse.Value : WallJumpHorizontalImpulse.Value, -WallJumpVerticalImpulse.Value);
				IsJumping = true;
				isSliding = false;
				PlaySound(wallJumpSound, (sender, e) => { });
			}
		}

		protected virtual void CheckForSpinJumpInput()
		{
			bool isSpinJumpDown = InputManager.IsNewActionPress(InputAction.SpinJump);

			if (isSpinJumpDown && !IsJumping && !IsSpinJumping)
			{
				Velocity = new Vector2(Velocity.X, -GetSpinJumpImpulse());
				IsSpinJumping = true;
				isSliding = false;
				PlaySound(spinJumpSound, (sender, e) => { });
			}
		}

		protected virtual float GetJumpImpulse()
		{
			float ratioBetweenAddendAndMaxSprintSpeed = MaximumJumpImpulseAddend.Value / MaximumSprintingSpeed.Value;
			return JumpImpulse.Value + (ratioBetweenAddendAndMaxSprintSpeed * Math.Abs(Velocity.X));
		}

		protected virtual float GetSpinJumpImpulse()
		{
			float ratioBetweenAddendAndMaxSprintSpeed = MaximumJumpImpulseAddend.Value / MaximumSprintingSpeed.Value;
			return SpinJumpImpulse.Value + (ratioBetweenAddendAndMaxSprintSpeed * Math.Abs(Velocity.X));
		}

		protected virtual void CheckForGroundPoundInput()
		{
			if (!IsOnGround && !isGroundPounding && InputManager.IsNewActionPress(InputAction.Down) && !IsSpinJumping && !isSliding && !IsSlidingDownWall)
			{
				// Start a ground pound.
				isGroundPounding = true;
				Velocity = TargetVelocity = Vector2.Zero;
				PlaySound(groundPoundDropSound, (sender, e) => { });
				SetPlayerGraphicsObject("groundPoundSpin");
			}
			else if (isGroundPounding && groundPoundSpinTimer < GroundPoundSpinTimer.Value)
			{
				groundPoundSpinTimer++;
				Velocity = Vector2.Zero;
			}
			else if (isGroundPounding && IsOnGround)
			{
				PlaySound(groundPoundHitSound, (sender, e) => { });
				isGroundPounding = false;
				groundPoundSpinTimer = 0;
				actionScheduler.ScheduleAction(() => wasGroundPounding = false, 20);
			}
			else if (isGroundPounding && groundPoundSpinTimer == GroundPoundSpinTimer.Value)
			{
				Velocity = new Vector2(0, GroundPoundVelocity.Value);
				wasGroundPounding = true;
			}
		}

		protected virtual void CheckForSlideInput()
		{
			// Sliding down slopes:
			// Is the player not sliding down a slope already?
			//	1. Is the user pressing Down?
			//	2. Is the player on the ground?
			//	3. Is the player on a slope?
			//	4. Is the player ground-pounding?
			//		If yes, immediately impart the total sliding velocity on the player in the direction down the slope.
			//		If no, set the target velocity to the sliding velocity.
			// Is the player already sliding down a slope?
			//	1. Is the player still on the ground?
			//		If yes, check to see if the tile beneath them is a slope. Stop sliding if so.
			//		If no, keep the isSliding flag set.
			// Also, cancel all sliding if the user presses Left, Right, Jump, or Spin Jump.

			Vector2 tileBeneathPlayerCheckPoint = Hitbox.BottomCenter;
			tileBeneathPlayerCheckPoint.Y += 1f;
			Tile tileBeneathPlayer = Owner.GetTileAtPosition(tileBeneathPlayerCheckPoint);

			if (!isSliding)
			{
				if ((InputManager.IsNewActionPress(InputAction.Down) && IsOnGround) || wasGroundPounding)
				{
					if (tileBeneathPlayer != null && tileBeneathPlayer.TileShape == CollidableShape.RightTriangle)
					{
						float slideDirection = (float)((RightTriangle)tileBeneathPlayer.Hitbox).HorizontalSlopedSide;
						if (wasGroundPounding)
						{
							Velocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
							TargetVelocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
						}
						else
						{
							TargetVelocity = new Vector2(SlidingVelocity.Value * slideDirection, Velocity.Y);
						}
						isSliding = true;
					}
				}
			}
			else
			{
				if (tileBeneathPlayer != null)
				{
					if (tileBeneathPlayer.TileShape == CollidableShape.Rectangle)
					{
						isSliding = false;
					}
					else
					{
						TargetVelocity = new Vector2((float)((RightTriangle)tileBeneathPlayer.Hitbox).HorizontalSlopedSide * SlidingVelocity.Value, Velocity.Y);
					}
				}
			}
		}

		protected virtual void CheckForInAirSpinInput()
		{
			if (!IsOnGround && Velocity.Y > 0f && !IsSpinJumping && InputManager.IsNewActionPress(InputAction.SpinJump) && !perfomingInAirSpin && inAirSpinTimeout == 0 && !IsSlidingDownWall && !isGroundPounding)
			{
				perfomingInAirSpin = true;
				inAirSpinTimer = InAirSpinDuration.Value;
				PlaySound(inAirSpinSound, (sender, e) => { });
				SetPlayerGraphicsObject("inAirSpin");
			}
			else if (perfomingInAirSpin)
			{
				inAirSpinTimer--;

				if (inAirSpinTimer == 0)
				{
					inAirSpinTimeout = InAirSpinTimeout.Value;
					perfomingInAirSpin = false;
				}
			}
			else if (inAirSpinTimeout > 0)
			{
				if (IsOnGround)
				{
					inAirSpinTimeout = 0;
				}
				else
				{
					inAirSpinTimeout--;
				}
			}
		}

		protected virtual void PlaySound(CachedSound sound, EventHandler additionalOnPlaybackEndedHandler)
		{
			AudioPlaybackEngine.Instance.PlaySound(sound, additionalOnPlaybackEndedHandler);
		}

		protected virtual void DeterminePlayerGraphicsObject()
		{
			if (isDead)
			{
				SetPlayerGraphicsObject("dead");
			}
			else if (isGroundPounding)
			{
				if (groundPoundSpinTimer >= GroundPoundSpinTimer.Value)
				{
					SetPlayerGraphicsObject("groundPoundDrop");
				}
			}
			else if (perfomingInAirSpin) { return; }
			else if (IsSlidingDownWall && !IsOnGround && Velocity.Y >= 0f)
			{
				SetPlayerGraphicsObject("slidingDownWall");
			}
			else if (IsJumping)
			{
				if (Math.Abs(Velocity.X) < MaximumSprintingSpeed.Value) { SetPlayerGraphicsObject("jumping"); }
				else { SetPlayerGraphicsObject("sprintingJump"); }
			}
			else if (IsSpinJumping)
			{
				SetPlayerGraphicsObject("spinJump");
			}
			else if (isSliding)
			{
				SetPlayerGraphicsObject("sliding");
			}
			else
			{
				if (Velocity.X == 0f /* && IsOnGround */) { SetPlayerGraphicsObject("standing"); }
				if (Velocity.X != 0f /* && IsOnGround */)
				{
					float inputDirection = (InputManager.IsCurrentActionPress(InputAction.Left)) ? -1f : (InputManager.IsCurrentActionPress(InputAction.Right)) ? 1f : 0f;
					if (Acceleration.X != 0f && Math.Sign(Velocity.X) != inputDirection && IsPlayerMoving) { SetPlayerGraphicsObject("skidding"); }
					else
					{
						if (Math.Abs(Velocity.X) < MaximumSprintingSpeed.Value) { SetPlayerGraphicsObject("walking"); }
						else { SetPlayerGraphicsObject("sprinting"); }
					}
				}
			}
		}

		protected virtual void SetPlayerGraphicsObject(string objectName)
		{
            graphics.CurrentObjectName = objectName;
		}

		private Tile GetTileBeneathPlayer()
		{
			Vector2 checkPointA = Hitbox.BottomLeft;
			Vector2 checkPointB = Hitbox.BottomCenter;
			Vector2 checkPointC = Hitbox.BottomRight;

			checkPointA.Y += 1f;
			checkPointB.Y += 1f;
			checkPointC.Y += 1f;

			Tile tileAtCheckPoint = Owner.GetTileAtPosition(checkPointA);
			if (tileAtCheckPoint != null) { return tileAtCheckPoint; }

			tileAtCheckPoint = Owner.GetTileAtPosition(checkPointB);
			if (tileAtCheckPoint != null) { return tileAtCheckPoint; }

			tileAtCheckPoint = Owner.GetTileAtPosition(checkPointC);
			if (tileAtCheckPoint != null) { return tileAtCheckPoint; }

			return null;
		}

		#region Internal/Loading Code
		public override void Initialize(Section owner)
		{
			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3PlayerMarioSmall");
			base.Initialize(owner);
		}

		protected virtual void InitializeSounds()
		{
			jumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiJump"));
			spinJumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiSpinJump"));
			groundPoundDropSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiGroundPound"));
			groundPoundHitSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiGroundPoundHit"));
			inAirSpinSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiInAirSpin"));
			wallJumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiWallJump"));
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}

		public override void HandleTileCollision(Tile tile, Vector2 resolutionDistance)
		{
			if (resolutionDistance.Y != 0f)
			{
				IsJumping = false;
				IsSpinJumping = false;
			}

			if (resolutionDistance.X != 0f)
			{
				Velocity = new Vector2(0f, Velocity.Y);
			}
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (Hitbox.Bottom < sprite.Hitbox.Center.Y && Velocity.Y >= 0f)
			{
				var damageComponent = GetComponent<DamageComponent>();
				damageComponent.PerformDamage(sprite, SpriteDamageTypes.PlayerStomp, 1);

				if (InputManager.IsCurrentActionPress(InputAction.Jump) || InputManager.IsCurrentActionPress(InputAction.SpinJump))
				{
					Velocity = new Vector2(Velocity.X, -(JumpImpulse.Value + MaximumJumpImpulseAddend.Value));
				}
				else
				{
					Velocity = new Vector2(Velocity.X, -JumpImpulse.Value);
				}
			}
			base.HandleSpriteCollision(sprite, resolutionDistance);
		}
		#endregion
	}
}
