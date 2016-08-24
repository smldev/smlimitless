using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	public sealed class EditorCameraTrackingObject : Sprite
	{
		private const float MoveDistance = 16f;
		private const float ShiftMultiplier = 4f;
		private const int DefaultMoveTimeout = 2;

		private int moveTimeout = DefaultMoveTimeout;

		public override string EditorCategory => "Internal Sprites";
		public override bool IsPlayer => false;

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void Draw()
		{
		}

		public override void Draw(Rectangle cropping)
		{
		}

		public override void Update()
		{
			if (moveTimeout > 0)
			{
				moveTimeout--;
			}

			if (moveTimeout == 0)
			{
				bool isShiftDown = InputManager.IsCurrentKeyPress(Keys.LeftShift) || InputManager.IsCurrentKeyPress(Keys.RightShift);

				Vector2 moveDistance = Vector2.Zero;
				if (InputManager.IsCurrentKeyPress(Keys.Left))
				{
					moveDistance.X -= MoveDistance;
					moveTimeout = DefaultMoveTimeout;
				}

				if (InputManager.IsCurrentKeyPress(Keys.Right))
				{
					moveDistance.X += MoveDistance;
					moveTimeout = DefaultMoveTimeout;
				}

				if (InputManager.IsCurrentKeyPress(Keys.Up))
				{
					moveDistance.Y -= MoveDistance;
					moveTimeout = DefaultMoveTimeout;
				}

				if (InputManager.IsCurrentKeyPress(Keys.Down))
				{
					moveDistance.Y += MoveDistance;
					moveTimeout = DefaultMoveTimeout;
				}

				if (isShiftDown) { moveDistance *= ShiftMultiplier; }
				Position += moveDistance;
			}

			base.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
		}
	}
}
