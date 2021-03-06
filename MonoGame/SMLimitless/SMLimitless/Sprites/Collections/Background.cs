﻿//-----------------------------------------------------------------------
// <copyright file="Background.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   A tileable, moving, optionally parallax background image used in sections.
	/// </summary>
	public sealed class Background : IDisposable
	{
		/// <summary>
		///   The gradient texture, composed from the top and bottom colors.
		/// </summary>
		private Texture2D backgroundGradient;

		/// <summary>
		///   A flag indicating if the content has been loaded.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		///   A reference to the section that owns this background.
		/// </summary>
		private Section owner;

        /// <summary>
        /// Gets a value indicating whether the resources of this object have been released.
        /// </summary>
        public bool IsDisposed { get; private set; }

		/// <summary>
		///   Gets the color at the bottom of the screen.
		/// </summary>
		public Color BottomColor { get; internal set; }

		/// <summary>
		///   Gets the color at the top of the screen.
		/// </summary>
		public Color TopColor { get; internal set; }

		/// <summary>
		///   Gets or sets a collection of all the background Layers composing
		///   this background.
		/// </summary>
		internal List<BackgroundLayer> Layers { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="Background" /> class.
		/// </summary>
		/// <param name="owner">
		///   A reference to the section that owns this background.
		/// </param>
		public Background(Section owner)
		{
			Layers = new List<BackgroundLayer>();
			this.owner = owner;
		}

		/// <summary>
		///   Adds a given background layer to the front of the background.
		/// </summary>
		/// <param name="layer">The background layer to add.</param>
		public void AddLayerToFront(BackgroundLayer layer)
		{
			Layers.Add(layer);

			if (isContentLoaded)
			{
				layer.LoadContent();
			}
		}

		/// <summary>
		///   Loads a background from a JSON string.
		/// </summary>
		/// <param name="json">A JSON string containing a valid background.</param>
		[Obsolete]
		public void Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);

			// Deserialize the root objects first.
			TopColor = obj["topColor"].ToColor();
			BottomColor = obj["bottomColor"].ToColor();

			// Now, get the Layers.
			JArray layersData = (JArray)obj["layers"];

			foreach (var layerData in layersData)
			{
				BackgroundLayer layer = new BackgroundLayer(owner.Camera, owner.Bounds);
				layer.Deserialize(layerData.ToString());
				Layers.Add(layer);
			}
		}

		/// <summary>
		///   Draws the background, including the gradient and all textures.
		/// </summary>
		public void Draw()
		{
			// A gradient can be drawn by stretching a 1-wide by 2-high texture
			// over the screen. The top third is the first color, and the bottom
			// third is the second color. The middle is the actual gradient.
			// Thus, the texture needs to be made taller than the screen - the
			// top needs to be drawn one-third of a screen higher than the
			// screen, and the height needs to be a screen height plus two-thirds
			// of a screen height.
			float screenHeight = GameServices.ScreenSize.Y;
			float gradientTop = owner.Camera.Viewport.Y - (screenHeight / 3f);
			float gradientHeight = owner.Camera.Viewport.Height + (screenHeight * (2f / 3f));
			GameServices.SpriteBatch.Draw(backgroundGradient, new Rectangle((int)owner.Camera.Viewport.X, (int)gradientTop, (int)owner.Camera.ViewportSize.X, (int)gradientHeight), Color.White);

			Layers.ForEach(l => l.Draw());
		}

		/// <summary>
		///   Returns an anonymous object containing all the objects used in
		///   serialization of this background.
		/// </summary>
		/// <returns>The objects to serialize.</returns>
		[Obsolete]
		public object GetSerializableObjects()
		{
			List<object> backgroundLayerObjects = new List<object>(Layers.Count);
			Layers.ForEach(b => backgroundLayerObjects.Add(b.GetSerializableObjects()));

			return new
			{
				topColor = TopColor.Serialize(),
				bottomColor = BottomColor.Serialize(),
				layers = backgroundLayerObjects
			};
		}

		/// <summary>
		///   Initializes this background.
		/// </summary>
		public void Initialize()
		{
			Layers.ForEach(l => l.Initialize());
		}

		/// <summary>
		///   Loads the content (images and gradient texture) for this background.
		/// </summary>
		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Color[] color = new Color[] { TopColor, BottomColor };
				backgroundGradient = new Texture2D(GameServices.Graphics, 1, 2);
				backgroundGradient.SetData<Color>(color);

				Layers.ForEach(l => l.LoadContent());
				isContentLoaded = true;
			}
		}

		/// <summary>
		///   Converts this background into a JSON string.
		/// </summary>
		/// <returns>A JSON string containing the objects of this background.</returns>
		[Obsolete]
		public string Serialize()
		{
			return JObject.FromObject(GetSerializableObjects()).ToString();
		}

		/// <summary>
		///   Updates this background. Background Layers will scroll according to
		///   the camera's movement.
		/// </summary>
		public void Update()
		{
			Layers.ForEach(l => l.Update());
		}

        private void Dispose(bool disposing)
        {
            if (IsDisposed) { return; }

            if (disposing)
            {
                if (backgroundGradient != null && !backgroundGradient.IsDisposed)
                {
                    backgroundGradient.Dispose();
                }
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Releases all resources used by this <see cref="Background"/> instance. 
        /// </summary>
        public void Dispose() => Dispose(true);
	}
}
