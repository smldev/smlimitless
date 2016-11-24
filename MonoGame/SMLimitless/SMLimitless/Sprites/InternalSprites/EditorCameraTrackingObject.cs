using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Input;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.InternalSprites
{
	/// <summary>
	///   A sprite that controls the camera while the level editor is active. The
	///   user can use the keyboard arrow keys to move the camera around. The
	///   user can hold Shift to make the camera move faster.
	/// </summary>
	public sealed class EditorCameraTrackingObject : Sprite
	{
		private const int DefaultMoveTimeout = 2;
		private const float MoveDistance = 16f;
		private const float ShiftMultiplier = 4f;
		private int moveTimeout = DefaultMoveTimeout;

		/// <summary>
		///   Gets the name of the category that this sprite is categorized
		///   within in the level editor.
		/// </summary>
		public override string EditorCategory => "Internal Sprites";

		/// <summary>
		///   Gets a value indicating whether this sprite is a player sprite.
		/// </summary>
		public override bool IsPlayer => false;

		/// <summary>
		///   Deserializes any objects that custom sprites have written to the
		///   level file.
		/// </summary>
		/// <param name="customObjects">
		///   An object containing the objects of the custom sprites.
		/// </param>
		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public EditorCameraTrackingObject()
		{
			// A size of 0.1,0.1 is small enough that the camera won't move to
			// a subpixel boundary
			Size = new Vector2(0.1f);
		}

		/// <summary>
		///   This sprite has no graphics.
		/// </summary>
		public override void Draw()
		{
		}

		/// <summary>
		///   This sprite has no graphics.
		/// </summary>
		public override void Draw(Rectangle cropping)
		{
		}

		/// <summary>
		///   Gets an anonymous object containing objects that need to be saved
		///   to the level file.
		/// </summary>
		/// <returns></returns>
		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		/// <summary>
		///   Loads the content for this sprite.
		/// </summary>
		public override void LoadContent()
		{
		}

		/// <summary>
		///   Updates this object.
		/// </summary>
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
				Owner.LastEditorCameraPosition = Position;
			}

			base.Update();
		}
	}
}
