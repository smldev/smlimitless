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
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sounds;

namespace SmlSprites.Players
{
	public class PlayerMario : Sprite
	{
		private ComplexGraphicsObject graphics;
		private int sprintChargeTimer = 0;
		private Direction direction = Direction.Left;

		private static PhysicsSetting<float> MaximumWalkingSpeed = new PhysicsSetting<float>("Player: Max Walking Speed", 0f, 100f, 35f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumRunningSpeed = new PhysicsSetting<float>("Player: Max Running Speed", 0f, 150f, 50f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumSprintingSpeed = new PhysicsSetting<float>("Player: Max Sprinting Speed", 0f, 150f, 65f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MovementAcceleration = new PhysicsSetting<float>("Player: Acceleration", 0f, 10000f, 200f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<int> FramesToSprintingAllowed = new PhysicsSetting<int>("Player: Frames to Sprinting Allowed", 1, 120, 60, PhysicsSettingType.Integer);
		private static PhysicsSetting<float> MaximumJumpImpulseAddend = new PhysicsSetting<float>("Player: Maximum Additional Jump Impulse", 0f, 100f, 10f, PhysicsSettingType.FloatingPoint);

		private static PhysicsSetting<float> JumpImpulse = new PhysicsSetting<float>("Player: Jump Impulse", 5f, 500f, 100f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> JumpGravityMultiplier = new PhysicsSetting<float>("Player: Jump Gravity Multiplier", 0.01f, 1f, 0.5f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> SpinJumpImpulse = new PhysicsSetting<float>("Player: Spin Jump Impulse", 5f, 500f, 100f, PhysicsSettingType.FloatingPoint);

		public string DebugGraphicsName { get; protected set; } = "";
		protected virtual Vector2 TargetVelocity { get; set; }
		protected virtual bool IsJumping { get; set; }
		protected virtual bool IsSpinJumping { get; set; }

		private CachedSound jumpSound;
		private CachedSound spinJumpSound;
		private bool jumpSoundPlaying = false;

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

		protected virtual Direction Direction 
		{
			get
			{
				direction = (Velocity.X < 0f) ? Direction.Left : (Velocity.X == 0f) ? direction : Direction.Right;
				return direction;
			}
		}
		public PlayerMario() : base()
		{
			Size = new Vector2(16f);
			IsActive = true;

			jumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiJump"));
			spinJumpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiSpinJump"));
		}

		public override void Draw()
		{
			graphics.Draw(Position.Floor(), Color.White, (Direction == Direction.Right) ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
		}

		public override void Update()
		{
			CheckForWalkRunInput();
			DetermineHorizontalAcceleration();
			SprintIfAllowed();

			CheckForJumpInput();
			CheckForSpinJumpInput();

			ApplyTileSurfaceFriction();
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
				else
				{
					Acceleration = new Vector2(Acceleration.X, Level.GravityAcceleration.Value);
				}
				TargetVelocity = new Vector2(TargetVelocity.X, MaximumGravitationalVelocity.Value);
			}

			ApplyAccelerationToVelocity(delta);
			graphics.Update();
		}

		protected virtual void ApplyAccelerationToVelocity(float delta)
		{
			Vector2 velocityAddend = Acceleration * delta;
			Vector2 resultVelocity = Vector2.Zero;
			bool xVelocityDiminishingTowardZero = Math.Abs(Velocity.X + velocityAddend.X) < Math.Abs(Velocity.X);	// is the sprite's velocity reducing?
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

			Velocity = resultVelocity;
		}

		protected virtual void CheckForWalkRunInput()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if ((isLeftDown && isRightDown) || (!isLeftDown && !isRightDown))
			{
				// If the user is holding both left and right down, we should cancel the acceleration and do nothing.
				CancelHorizontalAcceleration();
			}
			else if (isLeftDown)
			{
				float targetVelocityX = (sprintChargeTimer == FramesToSprintingAllowed.Value) ? -MaximumSprintingSpeed.Value : (IsRunning) ? -MaximumRunningSpeed.Value : -MaximumWalkingSpeed.Value;
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
			}
			else if (isRightDown)
			{
				float targetVelocityX = (sprintChargeTimer == FramesToSprintingAllowed.Value) ? MaximumSprintingSpeed.Value : (IsRunning) ? MaximumRunningSpeed.Value : MaximumWalkingSpeed.Value;
				TargetVelocity = new Vector2(targetVelocityX, TargetVelocity.Y);
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
			if (Math.Abs(Velocity.X) >= MaximumRunningSpeed.Value && IsPlayerMoving)
			{
				if (sprintChargeTimer < FramesToSprintingAllowed.Value)
				{
					sprintChargeTimer++;
				}
				else
				{
					TargetVelocity = new Vector2((Velocity.X > 0f) ? MaximumSprintingSpeed.Value : -MaximumSprintingSpeed.Value, TargetVelocity.Y);
				}
			}
			else if (Math.Abs(Velocity.X) < MaximumRunningSpeed.Value && sprintChargeTimer != 0)
			{
				sprintChargeTimer = 0;
			}
		}

		protected virtual void CancelHorizontalAcceleration()
		{
			Acceleration = new Vector2(0f, Acceleration.Y);
			TargetVelocity = new Vector2(0f, TargetVelocity.Y);
		}

		protected virtual void ApplyTileSurfaceFriction()
		{
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
			bool isJumpDown = InputManager.IsNewActionPress(InputAction.Jump);

			if (isJumpDown && !IsJumping && !IsSpinJumping)
			{
				Velocity = new Vector2(Velocity.X, -GetJumpImpulse());
				IsJumping = true;
				PlayJumpSound();
			}
		}

		protected virtual void CheckForSpinJumpInput()
		{
			bool isSpinJumpDown = InputManager.IsNewActionPress(InputAction.SpinJump);

			if (isSpinJumpDown && !IsJumping && !IsSpinJumping)
			{
				Velocity = new Vector2(Velocity.X, -GetSpinJumpImpulse());
				IsSpinJumping = true;
				PlaySpinJumpSound();
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

		protected virtual void PlayJumpSound()
		{
			AudioPlaybackEngine.Instance.PlaySound(jumpSound, (sender, e) => { jumpSoundPlaying = false; });
			jumpSoundPlaying = true;
		}

		protected virtual void PlaySpinJumpSound()
		{
			AudioPlaybackEngine.Instance.PlaySound(spinJumpSound, (sender, e) => { });
		}

		protected virtual void DeterminePlayerGraphicsObject()
		{
			if (IsJumping)
			{ 
				if (Math.Abs(Velocity.X) < MaximumSprintingSpeed.Value) { SetPlayerGraphicsObject("jumping"); }
				else { SetPlayerGraphicsObject("sprintingJump"); }
			}
			else if (IsSpinJumping)
			{
				SetPlayerGraphicsObject("spinJump");
			}
			else
			{
				if (Velocity.X == 0f /* && IsOnGround */) { SetPlayerGraphicsObject("standing"); }
				if (Velocity.X != 0f /* && IsOnGround */)
				{
					if (Acceleration.X != 0f && Math.Sign(Velocity.X) != Math.Sign(Acceleration.X) && IsPlayerMoving) { SetPlayerGraphicsObject("skidding"); }
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
			// TODO: implement
			DebugGraphicsName = objectName;
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
			if (resolutionDistance.Y < 0f)
			{
				IsJumping = false;
				IsSpinJumping = false;
			}
		}
		#endregion
	}
}
