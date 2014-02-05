//-----------------------------------------------------------------------
// <copyright file="Camera2D.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a camera that is used to view a portion
    /// of a two-dimensional space.
    /// </summary>
    public sealed class Camera2D
    {
        /// <summary>
        /// A backing field for the Zoom property.
        /// </summary>
        private float zoom;

        private Vector2 position;

        /// <summary>
        /// A field containing the matrix transformation for the camera.
        /// </summary>
        private Matrix transform;

        /// <summary>
        /// Gets or sets the zoom of the camera.
        /// </summary>
        public float Zoom
        {
            get 
            { 
                return this.zoom; 
            }

            set
            {
                if (value > 0.1f)
                {
                    this.zoom = value;
                }
                else
                {
                    this.zoom = 0.1f;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the camera in degrees.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the position of the top-left corner of the camera's view.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets a rectangle corresponding to the viewport of the camera.
        /// </summary>
        public BoundingRectangle Viewport
        {
            get
            {
                return new BoundingRectangle(this.Position.X, this.Position.Y, this.ViewportSize.X, this.ViewportSize.Y);
            }
        }

        /// <summary>
        /// Gets the size of the viewport of the camera, adjusted for zoom.
        /// </summary>
        public Vector2 ViewportSize
        {
            get
            {
                return GameServices.ScreenSize * this.Zoom;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera2D"/> class.
        /// </summary>
        public Camera2D()
        {
            this.Zoom = 1.0f;
            this.Rotation = 0.0f;
            this.Position = Vector2.Zero;
        }

        /// <summary>
        /// Moves the camera by a given distance.
        /// </summary>
        /// <param name="amount">The distance to move the camera by.</param>
        public void Move(Vector2 amount)
        {
            this.Position += amount;
        }

        /// <summary>
        /// Gets a matrix transformation for use with the sprite batch.
        /// </summary>
        /// <returns>The matrix transformation.</returns>
        public Matrix GetTransformation()
        {
            this.transform = Matrix.CreateTranslation(-this.Position.X, -this.Position.Y, 0) *
                        Matrix.CreateScale(this.Zoom) *
                        Matrix.CreateRotationZ(this.Rotation);

            return this.transform;
        }
    }
}