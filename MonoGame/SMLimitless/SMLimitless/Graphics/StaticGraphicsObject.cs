//-----------------------------------------------------------------------
// <copyright file="StaticGraphicsObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
	/// <summary>
	///   A graphics object with one texture.
	/// </summary>
	public class StaticGraphicsObject : IGraphicsObject
	{
		/// <summary>
		///   The file path to the image of the texture.
		/// </summary>
		private string filePath;

		/// <summary>
		///   A value indicating whether this object has had its content loaded.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		///   A value indicating whether this object has been loaded.
		/// </summary>
		private bool isLoaded;

		/// <summary>
		///   The texture of this object.
		/// </summary>
		private Texture2D texture;

		/// <summary>
		///   Gets or sets a value that, if loaded from a ComplexGraphicsObject,
		///   this field represents the owner of this object.
		/// </summary>
		internal ComplexGraphicsObject CgoOwner { get; set; }

		/// <summary>
		///   Gets or sets a value that, if loaded from a ComplexGraphicsObject,
		///   this field represents the source rectangle of the texture on the
		///   CGO's texture.
		/// </summary>
		internal Rectangle CgoSourceRect { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="StaticGraphicsObject"
		///   /> class.
		/// </summary>
		public StaticGraphicsObject()
		{
		}

		/// <summary>
		///   Creates a deep copy of this object. The texture is only shallow
		///   (reference) copied to reduce memory size.
		/// </summary>
		/// <returns>A deep copy of this object.</returns>
		public IGraphicsObject Clone()
		{
			var clone = new StaticGraphicsObject();
			clone.texture = texture;
			clone.CgoSourceRect = CgoSourceRect;
			clone.filePath = filePath;
			clone.isLoaded = isLoaded;
			clone.isContentLoaded = (texture != null) ? isContentLoaded : false;
			return clone;
		}

		/// <summary>
		///   Draws this object to the screen.
		/// </summary>
		/// <param name="position">The position on the screen to draw at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		public void Draw(Vector2 position, Color color)
		{
			GameServices.SpriteBatch.Draw(texture, position, color);
		}

		/// <summary>
		///   Draws this object to the screen.
		/// </summary>
		/// <param name="position">The position on the screen to draw at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="effects">Defines sprite mirroring.</param>
		public void Draw(Vector2 position, Color color, SpriteEffects effects)
		{
			GameServices.SpriteBatch.Draw(texture, position, color, effects);
		}

		/// <summary>
		///   Draws a portion of this object to the screen.
		/// </summary>
		/// <param name="position">
		///   The position to draw this object. Cropping will not affect the position.
		/// </param>
		/// <param name="cropping">The portion of the object to draw.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="effects">Defines sprite mirroring.</param>
		public void Draw(Vector2 position, Rectangle cropping, Color color, SpriteEffects effects)
		{
			if (!texture.ValidateCropping(cropping)) { throw new ArgumentException($"The cropping {cropping} was not valid for this texture. (Width: {texture.Width}, Height: {texture.Height})"); }

			Rectangle destinationRectangle = new Rectangle((int)position.X + cropping.X, (int)position.Y + cropping.Y, cropping.Width, cropping.Height);
			Rectangle sourceRectangle = cropping;

			GameServices.SpriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, effects, 0f);
		}

		/// <summary>
		///   Gets a single texture that can be used on a button in the editor.
		/// </summary>
		/// <returns>The texture of this graphics object.</returns>
		public Texture2D GetEditorGraphics()
		{
			return texture;
		}

		/// <summary>
		///   Returns the size, in pixels, of this object.
		/// </summary>
		/// <returns>The size of this object.</returns>
		public Vector2 GetSize()
		{
			if (isContentLoaded)
			{
				return new Vector2(texture.Width, texture.Height);
			}
			else
			{
				throw new InvalidOperationException("StaticGraphicsObject.GetSize(): This object isn't fully loaded, and thus cannot return its size.");
			}
		}

		/// <summary>
		///   Loads this object.
		/// </summary>
		/// <param name="filePath">The file path to the image to use.</param>
		public void Load(string filePath)
		{
			if (!isLoaded)
			{
				this.filePath = filePath;
				isLoaded = true;
			}
		}

		/// <summary>
		///   Loads this object. Do not use this overload; use Load(string)
		///   instead. This method is only here for the IGraphicsObject contract.
		/// </summary>
		/// <param name="filePath">The file path to the image to use.</param>
		/// <param name="config">
		///   A DataReader containing the configuration file for this object.
		/// </param>
		public void Load(string filePath, DataReader config)
		{
			throw new InvalidOperationException("StaticGraphicsObject.Load(string, DataReader): Static objects do not accepts configuration files.  Please use Load(string) instead.");
		}

		/// <summary>
		///   Loads the content for this object.
		/// </summary>
		public void LoadContent()
		{
			if (isLoaded && !isContentLoaded)
			{
				texture = GraphicsManager.LoadTextureFromFile(filePath);
				isContentLoaded = true;
			}
		}

		/// <summary>
		///   Updates this object.
		/// </summary>
		public void Update()
		{
		}

		/// <summary>
		///   Loads this object from a ComplexGraphicsObject.
		/// </summary>
		/// <param name="section">
		///   The section of the CGO config file that specifies this object.
		/// </param>
		/// <param name="owner">The CGO that owns this object.</param>
		internal void Load(Dictionary<string, string> section, ComplexGraphicsObject owner)
		{
			if (!isLoaded)
			{
				CgoSourceRect = Vector2Extensions.Parse(section["Frame"]).ToRectangle(owner.FrameSize);
				filePath = owner.FilePath;
				CgoOwner = owner;
				isLoaded = true;
			}
		}

		/// <summary>
		///   Loads the content for this object if this object was created by a ComplexGraphicsObject.
		/// </summary>
		/// <param name="fileTexture">The texture of this object.</param>
		internal void LoadContentCGO(Texture2D fileTexture)
		{
			if (isLoaded && !isContentLoaded && CgoSourceRect != Rectangle.Empty)
			{
				texture = fileTexture.Crop(CgoSourceRect);
				isContentLoaded = true;
			}
		}
	}
}
