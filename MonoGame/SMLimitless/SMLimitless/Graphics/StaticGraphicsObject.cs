//-----------------------------------------------------------------------
// <copyright file="StaticGraphicsObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// A graphics object with one texture.
    /// </summary>
    public class StaticGraphicsObject : IGraphicsObject
    {
        /// <summary>
        /// A value indicating whether this object has been loaded.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// A value indicating whether this object has had its content loaded.
        /// </summary>
        private bool isContentLoaded;

        /// <summary>
        /// The file path to the image of the texture.
        /// </summary>
        private string filePath;

        /// <summary>
        /// The texture of this object.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticGraphicsObject"/> class.
        /// </summary>
        public StaticGraphicsObject()
        {
        }

        /// <summary>
        /// Gets or sets a value that, 
        /// if loaded from a ComplexGraphicsObject,
        /// this field represents the source rectangle
        /// of the texture on the CGO's texture.
        /// </summary>
        internal Rectangle CgoSourceRect { get; set; }

        /// <summary>
        /// Gets or sets a value that,
        /// if loaded from a ComplexGraphicsObject,
        /// this field represents the owner of this object.
        /// </summary>
        internal ComplexGraphicsObject CgoOwner { get; set; }

        /// <summary>
        /// Loads this object.
        /// </summary>
        /// <param name="filePath">The file path to the image to use.</param>
        public void Load(string filePath)
        {
            if (!this.isLoaded)
            {
                this.filePath = filePath;
                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Loads this object.
        /// Do not use this overload; use Load(string) instead. This method is only here for the IGraphicsObject contract.
        /// </summary>
        /// <param name="filePath">The file path to the image to use.</param>
        /// <param name="config">A DataReader containing the configuration file for this object.</param>
        public void Load(string filePath, DataReader config)
        {
            throw new InvalidOperationException("StaticGraphicsObject.Load(string, DataReader): Static objects do not accepts configuration files.  Please use Load(string) instead.");
        }

        /// <summary>
        /// Loads the content for this object.
        /// </summary>
        public void LoadContent()
        {
            if (this.isLoaded && !this.isContentLoaded)
            {
                this.texture = GraphicsManager.LoadTextureFromFile(this.filePath);
                this.isContentLoaded = true;
            }
        }

		public Texture2D GetEditorGraphics()
		{
			return texture;
		}

        /// <summary>
        /// Updates this object.
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// Draws this object to the screen.
        /// </summary>
        /// <param name="position">The position on the screen to draw at.</param>
        /// <param name="color">The color to shade this object. Use Color.White for no shading.</param>
        public void Draw(Vector2 position, Color color)
        {
            GameServices.SpriteBatch.Draw(this.texture, position, color);
        }

        /// <summary>
        /// Draws this object to the screen.
        /// </summary>
        /// <param name="position">The position on the screen to draw at.</param>
        /// <param name="color">The color to shade this object. Use Color.White for no shading.</param>
        /// <param name="effects">Defines sprite mirroring.</param>
        public void Draw(Vector2 position, Color color, SpriteEffects effects)
        {
            GameServices.SpriteBatch.Draw(this.texture, position, color, effects);
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// The texture is only shallow (reference) copied to reduce memory size.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public IGraphicsObject Clone()
        {
            var clone = new StaticGraphicsObject();
            clone.texture = this.texture;
            clone.filePath = this.filePath;
            clone.isLoaded = this.isLoaded;
            clone.isContentLoaded = this.isContentLoaded;
            return clone;
        }

        /// <summary>
        /// Loads this object from a ComplexGraphicsObject.
        /// </summary>
        /// <param name="section">The section of the CGO config file that specifies this object.</param>
        /// <param name="owner">The CGO that owns this object.</param>
        internal void Load(Dictionary<string, string> section, ComplexGraphicsObject owner)
        {
            if (!this.isLoaded)
            {
                this.CgoSourceRect = Vector2Extensions.Parse(section["Frame"]).ToRectangle(owner.FrameSize);
                this.filePath = owner.FilePath;
                this.CgoOwner = owner;
                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Loads the content for this object 
        /// if this object was created by a ComplexGraphicsObject.
        /// </summary>
        /// <param name="fileTexture">The texture of this object.</param>
        internal void LoadContentCGO(Texture2D fileTexture)
        {
            if (this.isLoaded && !this.isContentLoaded && this.CgoSourceRect != Rectangle.Empty)
            {
                this.texture = fileTexture.Crop(this.CgoSourceRect);
                this.isContentLoaded = true;
            }
        }

        /// <summary>
        /// Returns the size, in pixels, of this object.
        /// </summary>
        /// <returns>The size of this object.</returns>
        public Vector2 GetSize()
        {
            if (this.isContentLoaded)
            {
                return new Vector2(this.texture.Width, this.texture.Height);
            }
            else
            {
                throw new InvalidOperationException("StaticGraphicsObject.GetSize(): This object isn't fully loaded, and thus cannot return its size.");
            }
        }
    }
}
