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

		private static PhysicsSetting<float> MaximumWalkingSpeed = new PhysicsSetting<float>("Player: Max Walking Speed", 0f, 1000f, 2f, PhysicsSettingType.FloatingPoint);
		private static PhysicsSetting<int> WalkAccelerationTime = new PhysicsSetting<int>("Player: Frames to Max Walking Speed", 0, 60, 15, PhysicsSettingType.Integer);

		public override string EditorCategory
		{
			get
			{
				return "Players";
			}
		}

		private bool IsMoving
		{
			get
			{
				return InputManager.IsCurrentActionPress(InputAction.Left) ^ InputManager.IsCurrentActionPress(InputAction.Right);
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
			CheckForWalkInput();
			ClampHorizontalSpeed();

			ApplyTileSurfaceFriction();
			base.Update();
		}

		protected void CheckForWalkInput()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if ((isLeftDown && isRightDown) || (!isLeftDown && !isRightDown))
			{
				// If the user is holding both left and right down, we should cancel the acceleration and do nothing.
				Acceleration = new Vector2(0f, Acceleration.Y);
			}
			else if (isLeftDown)
			{
				// Begin acceleration to the left.
				float delta = MaximumWalkingSpeed.Value / WalkAccelerationTime.Value;
				Acceleration = new Vector2(Acceleration.X - delta, Acceleration.Y);
			}
			else if (isRightDown)
			{
				float delta = MaximumWalkingSpeed.Value / WalkAccelerationTime.Value;
				Acceleration = new Vector2(Acceleration.X + delta, Acceleration.Y);
			}
		}

		protected void ClampHorizontalSpeed()
		{
			bool isRunning = InputManager.IsCurrentActionPress(InputAction.Run) || InputManager.IsCurrentActionPress(InputAction.AltRun);

			if (!isRunning)
			{
				// Player is walking; clamp their speed to the maximum walking velocity.
				if (Velocity.X < -MaximumWalkingSpeed.Value || Velocity.X > MaximumWalkingSpeed.Value)
				{
					float clampedVelocity = MathHelper.Clamp(Velocity.X, -MaximumWalkingSpeed.Value, MaximumWalkingSpeed.Value);
					Acceleration = new Vector2(0f, Acceleration.Y);
					Velocity = new Vector2(clampedVelocity, Velocity.Y);
				}
			}
			else
			{
				// Player is running.
			}
		}

		protected void ApplyTileSurfaceFriction()
		{
			bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
			bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

			if (!IsMoving)
			{
				Vector2 checkPosition = Hitbox.BottomCenter;
				checkPosition.Y += GameServices.GameObjectSize.Y / 2f;

				Tile tileBeneathPlayer = Owner.GetTileAtPosition(checkPosition);
				if (tileBeneathPlayer == null) { return; }

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
