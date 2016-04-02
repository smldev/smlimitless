using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Players
{
	public class PlayerMario : Sprite
	{
		private StaticGraphicsObject placeholderGraphics;

		private static PhysicsSetting<float> MaximumWalkingSpeed = new PhysicsSetting<float>("Player: Max Walking Speed", 0f, 100f, 35f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<float> MaximumRunningSpeed = new PhysicsSetting<float>("Player: Max Running Speed", 0f, 150f, 50f, PhysicsSettingType.FloatingPoint);
		//private static PhysicsSetting<float> MaximumAcceleration = new PhysicsSetting<float>("Player: Max Acceleration (abs)", 0f, 10000f, 200f, PhysicsSettingType.FloatingPoint);
		//private static PhysicsSetting<int> RunAccelerationTime = new PhysicsSetting<int>("Player: Frames to Max Running Speed", 1, 60, 15, PhysicsSettingType.Integer);
		private static PhysicsSetting<float> MovementAcceleration = new PhysicsSetting<float>("Player: Acceleration", 0f, 10000f, 200f, PhysicsSettingType.FloatingPoint);

		public string DebugGraphicsName { get; protected set; } = "";
		protected Vector2 TargetVelocity { get; set; }

		public override string EditorCategory
		{
			get
			{
				return "Players";
			}
		}

		protected bool IsMoving
		{
			get
			{
				return InputManager.IsCurrentActionPress(InputAction.Left) ^ InputManager.IsCurrentActionPress(InputAction.Right);
			}
		}

		protected bool IsRunning
		{
			get
			{
				return InputManager.IsCurrentActionPress(InputAction.Run) || InputManager.IsCurrentActionPress(InputAction.AltRun);
			}
		}

		public PlayerMario() : base()
		{
			Size = new Vector2(16f);
		}

		public override void Draw()
		{
			placeholderGraphics.Draw(Position.Floor(), Color.White);
		}

		public override void Update()
		{
			CheckForWalkRunInput();
			DetermineHorizontalAcceleration();

			ApplyTileSurfaceFriction();
			DeterminePlayerGraphicsObject();
			BaseUpdate();
		}

		protected void BaseUpdate()
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
				Acceleration = new Vector2(Acceleration.X, Level.GravityAcceleration.Value);
				TargetVelocity = new Vector2(TargetVelocity.X, MaximumGravitationalVelocity.Value);
			}

			ApplyAccelerationToVelocity(delta);
		}

		protected void ApplyAccelerationToVelocity(float delta)
		{
			Vector2 velocityAddend = Acceleration * delta;
			Vector2 resultVelocity = Vector2.Zero;
			bool xVelocityDiminishingTowardZero = Math.Abs(Velocity.X + velocityAddend.X) < Math.Abs(Velocity.X);
			bool yVelocityDiminishingTowardZero = Math.Abs(Velocity.Y + velocityAddend.Y) < Math.Abs(Velocity.Y);

			// X-axis checks
			if (TargetVelocity.X <= 0f)
			{
				// Assume that TargetVelocity and velocityDelta have the same sign
				if (Velocity.X + velocityAddend.X >= TargetVelocity.X || xVelocityDiminishingTowardZero) { resultVelocity.X = Velocity.X + velocityAddend.X; }
				else if ((Velocity.X > TargetVelocity.X) && (Velocity.X + velocityAddend.X < TargetVelocity.X)) { resultVelocity.X = TargetVelocity.X; }
				else if (Velocity.X <= TargetVelocity.X) { resultVelocity.X = Velocity.X; }
			}
			else if (TargetVelocity.X >= 0f)
			{
				if (Velocity.X + velocityAddend.X <= TargetVelocity.X || xVelocityDiminishingTowardZero) { resultVelocity.X = Velocity.X + velocityAddend.X; }
				else if ((Velocity.X < TargetVelocity.X) && (Velocity.X + velocityAddend.X > TargetVelocity.X)) { resultVelocity.X = TargetVelocity.X; }
				else if (Velocity.X >= TargetVelocity.X) { resultVelocity.X = Velocity.X; }
			}

			// Y-axis checks
			if (TargetVelocity.Y <= 0f)
			{
				if (Velocity.Y + velocityAddend.Y >= TargetVelocity.Y || yVelocityDiminishingTowardZero) { resultVelocity.Y = Velocity.Y + velocityAddend.Y; }
				else if ((Velocity.Y > TargetVelocity.Y) && (Velocity.Y + velocityAddend.Y < TargetVelocity.Y)) { resultVelocity.Y = TargetVelocity.Y; }
				else if (Velocity.Y <= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y; }
			}
			else if (TargetVelocity.Y >= 0f)
			{
				if (Velocity.Y + velocityAddend.Y <= TargetVelocity.Y || yVelocityDiminishingTowardZero) { resultVelocity.Y = Velocity.Y + velocityAddend.Y; }
				else if ((Velocity.Y < TargetVelocity.Y) && (Velocity.Y + velocityAddend.Y > TargetVelocity.Y)) { resultVelocity.Y = TargetVelocity.Y; }
				else if (Velocity.Y >= TargetVelocity.Y) { resultVelocity.Y = Velocity.Y; }
			}

			Velocity = resultVelocity;
		}

		protected void CheckForWalkRunInput()
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
				TargetVelocity = new Vector2((IsRunning)  ? -MaximumRunningSpeed.Value : -MaximumWalkingSpeed.Value, TargetVelocity.Y);
			}
			else if (isRightDown)
			{
				TargetVelocity = new Vector2((IsRunning) ? MaximumRunningSpeed.Value : MaximumWalkingSpeed.Value, TargetVelocity.Y);
			}
		}

		protected void DetermineHorizontalAcceleration()
		{
			float absTargetVelocityX = Math.Abs(TargetVelocity.X);
			float absCurrentVelocityX = Math.Abs(Velocity.X);
			
			if (absCurrentVelocityX < absTargetVelocityX)
			{
				// The player is moving slower than they need to be. Accelerate them.
				Acceleration = new Vector2((TargetVelocity.X >= 0f) ? MovementAcceleration.Value : -MovementAcceleration.Value, Acceleration.Y);
			}
			else if (absCurrentVelocityX > absTargetVelocityX)
			{
				// The player is moving more quickly than they need to be. Slow them down if they're standing on a tile.
				ApplyTileSurfaceFriction();
			}
			else if (absCurrentVelocityX == absTargetVelocityX)
			{
				// The player is moving exactly as fast as they need to be.
				Acceleration = new Vector2(0f, Acceleration.Y);
			}
		}

		protected void CancelHorizontalAcceleration()
		{
			Acceleration = new Vector2(0f, Acceleration.Y);
			TargetVelocity = new Vector2(0f, TargetVelocity.Y);
		}

		protected void ApplyTileSurfaceFriction()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			Vector2 checkPosition = Hitbox.BottomCenter;
			checkPosition.Y += GameServices.GameObjectSize.Y / 2f;

			Tile tileBeneathPlayer = Owner.GetTileAtPosition(checkPosition);
			if (tileBeneathPlayer == null) { return; }

			if (!IsMoving || (IsMoving && !IsRunning && Math.Abs(Velocity.X) > MaximumWalkingSpeed.Value))
			{
				float tileSurfaceFrictionDelta = tileBeneathPlayer.SurfaceFriction * GameServices.GameTime.GetElapsedSeconds();
				
				if (Velocity.X > -0.1f && Velocity.X < 0.1f)
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

		protected void DeterminePlayerGraphicsObject()
		{
			if (Velocity.X == 0f /* && IsOnGround */) { SetPlayerGraphicsObject("standing"); }
			if (Velocity.X != 0f /* && IsOnGround */)
			{
				if (Math.Sign(Velocity.X) != Math.Sign(Acceleration.X) && IsMoving) { SetPlayerGraphicsObject("skidding"); }
				else { SetPlayerGraphicsObject("walking"); }
			}
		}

		protected void SetPlayerGraphicsObject(string objectName)
		{
			// TODO: implement
			DebugGraphicsName = objectName;
		}

		public override void Initialize(Section owner)
		{
			placeholderGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3PlayerPlaceholder");
			base.Initialize(owner);
		}


		#region Internal/Loading Code
		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
			placeholderGraphics.LoadContent();
		}
		#endregion
	}
}
