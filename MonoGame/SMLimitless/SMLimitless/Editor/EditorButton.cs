//-----------------------------------------------------------------------
// <copyright file="EditorButton.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Graphics;

using DrawRect = System.Drawing.Rectangle;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace SMLimitless.Editor
{
    /// <summary>
    /// Represents the location and image of
    /// an object's button on the level editor
    /// selection window.
    /// </summary>
    public struct EditorButton
    {
		public Button Button { get; private set; }
		
        /// <summary>
        /// Gets or sets the distance from the left edge of the window in pixels.
        /// </summary>
        public int X
		{
			get
			{
				return Button.Location.X;
			}
			set
			{
				Button.Location = new System.Drawing.Point(value, Button.Location.Y);
			}
		}

        /// <summary>
        /// Gets or sets the distance from the top edge of the window in pixels.
        /// </summary>
        public int Y
		{
			get
			{
				return Button.Location.Y;
			}
			set
			{
				Button.Location = new System.Drawing.Point(Button.Location.X, value);
			}
		}

        /// <summary>
        /// Gets or sets the size of the button in pixels.
        /// </summary>
        public Size Size 
		{
			get
			{
				return Button.Size;
			}
			set
			{
				Button.Size = value;
			}
		}

        /// <summary>
        /// Gets the image displayed on the button.
        /// </summary>
        public Image ButtonImage { get; private set; }

        /// <summary>
        /// The texture on which the button's image is present.
        /// </summary>
        private Texture2D buttonSourceTexture;

        /// <summary>
        /// The part of the texture containing the button's image.
        /// </summary>
        private XnaRect textureSourceRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorButton"/> struct.
        /// </summary>
        /// <param name="x">The distance from the left edge of the window in pixels.</param>
        /// <param name="y">The distance from the top edge of the window in pixels.</param>
        /// <param name="width">The width of the button in pixels.</param>
        /// <param name="height">The height of the button in pixels.</param>
        public EditorButton(int x, int y, int width, int height) : this()
        {
            this.X = x;
            this.Y = y;
			this.Size = new Size(width, height);
        }

        /// <summary>
        /// Sets the image for this button.
        /// </summary>
        /// <param name="source">The texture on which the image is.</param>
        /// <param name="sourceRectangle">The part of the texture where the image is.</param>
        public void SetImage(Texture2D source, XnaRect sourceRectangle)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "EditorButton.SetImage(source, sourceRectangle): The source texture was null. Please provide a non-null source texture.");
            }
            else if (source.Width == 0 || source.Height == 0)
            {
                throw new ArgumentException("EditorButton.SetImage(source, sourceRectangle): The source texture has no width and/or height. Please provide a texture containing an image.", "source");
            }
            else if (sourceRectangle.X < 0 || sourceRectangle.Y < 0 || sourceRectangle.Width <= 0 || sourceRectangle.Height <= 0)
            {
                throw new ArgumentException("EditorButton.SetImage(source, sourceRectangle): The source rectangle either starts outside of the source texture or has a zero/negative width or height. Please provide a proper source rectangle.", "sourceRectangle");
            }
            else if (sourceRectangle.X > source.Width || sourceRectangle.Y > source.Height || sourceRectangle.X + sourceRectangle.Width > source.Width || sourceRectangle.Y + sourceRectangle.Height > source.Height)
            {
                throw new ArgumentException("EditorButton.SetImage(source, sourceRectangle): Part or all of the source rectangle falls outside of the source texture. Please provide a rectangle within the source texture.", "sourceRectangle");
            }

            this.buttonSourceTexture = source;
            this.textureSourceRectangle = sourceRectangle;

            Texture2D croppedTexture = source.Crop(this.textureSourceRectangle);
            this.ButtonImage = croppedTexture.ToImage();
        }
    }
}