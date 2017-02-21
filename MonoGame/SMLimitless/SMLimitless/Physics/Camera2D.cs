//-----------------------------------------------------------------------
// <copyright file="Camera2D.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using SMLimitless.Editor.Attributes;

namespace SMLimitless.Physics
{
	/// <summary>
	///   Represents a camera that is used to view a portion of a two-dimensional space.
	/// </summary>
	public sealed class Camera2D
	{
		/// <summary>
		///   A backing field for the Position property;
		/// </summary>
		private Vector2 position;

		/// <summary>
		///   A field containing the matrix transformation for the camera.
		/// </summary>
		private Matrix transform;

		/// <summary>
		///   A backing field for the Zoom property.
		/// </summary>
		private float zoom;

		/// <summary>
		///   Gets or sets the position of the top-left corner of the camera's view.
		/// </summary>
		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = new Vector2((int)value.X, (int)value.Y);
			}
		}

		/// <summary>
		///   Gets or sets the rotation of the camera in degrees.
		/// </summary>
		[FloatingPointProperty("Rotation", "rot", 0f, 360f)]
		public float Rotation { get; set; }

		/// <summary>
		///   Gets a rectangle corresponding to the viewport of the camera.
		/// </summary>
		public BoundingRectangle Viewport
		{
			get
			{
				return new BoundingRectangle(Position.X, Position.Y, ViewportSize.X, ViewportSize.Y);
			}
		}

		/// <summary>
		///   Gets the size of the viewport of the camera, adjusted for zoom.
		/// </summary>
		public Vector2 ViewportSize
		{
			get
			{
				return GameServices.ScreenSize / Zoom;
			}
		}

		/// <summary>
		///   Gets or sets the zoom of the camera.
		/// </summary>
		[FloatingPointProperty("Zoom", "zoom", 0f, 100f)]
		public float Zoom
		{
			get
			{
				return zoom;
			}

			set
			{
				if (value > 0.1f)
				{
					zoom = value;
				}
				else
				{
					zoom = 0.1f;
				}
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="Camera2D" /> class.
		/// </summary>
		public Camera2D()
		{
			Zoom = 1.0f;
			Rotation = 0.0f;
			Position = Vector2.Zero;
		}

		/// <summary>
		///   Gets a matrix transformation for use with the sprite batch.
		/// </summary>
		/// <returns>The matrix transformation.</returns>
		public Matrix GetTransformation()
		{
			transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
						Matrix.CreateScale(Zoom) *
						Matrix.CreateRotationZ(Rotation);

			return transform;
		}

		/// <summary>
		///   Moves the camera by a given distance.
		/// </summary>
		/// <param name="amount">The distance to move the camera by.</param>
		public void Move(Vector2 amount)
		{
			Position += amount;
		}
	}
}
