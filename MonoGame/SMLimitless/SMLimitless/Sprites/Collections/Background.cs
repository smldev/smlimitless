//-----------------------------------------------------------------------
// <copyright file="Background.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A tileable, moving, optionally parallax background image
    /// used in sections.
    /// </summary>
    public sealed class Background : ISerializable
    {
        /// <summary>
        /// Gets the color at the top of the screen.
        /// </summary>
        public Color TopColor { get; private set; }

        /// <summary>
        /// Gets the color at the bottom of the screen.
        /// </summary>
        public Color BottomColor { get; private set; }

        /// <summary>
        /// A collection of all the background Layers composing this background.
        /// </summary>
        internal List<BackgroundLayer> Layers;

        /// <summary>
        /// A flag indicating if the content has been loaded.
        /// </summary>
        private bool isContentLoaded;

        /// <summary>
        /// The gradient texture, composed from the top and bottom colors.
        /// </summary>
        private Texture2D backgroundGradient;

        /// <summary>
        /// A reference to the section that owns this background.
        /// </summary>
        private Section owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="Background"/> class.
        /// </summary>
        /// <param name="owner">A reference to the section that owns this background.</param>
        public Background(Section owner)
        {
            this.Layers = new List<BackgroundLayer>();
            this.owner = owner;
        }

        /// <summary>
        /// Initializes this background.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Loads the content (images and gradient texture) for this background.
        /// </summary>
        public void LoadContent()
        {
            if (!this.isContentLoaded)
            {
                Color[] color = new Color[] { this.TopColor, this.BottomColor };
                this.backgroundGradient = new Texture2D(GameServices.Graphics, 1, 2);
                this.backgroundGradient.SetData<Color>(color);

                this.Layers.ForEach(l => l.LoadContent());
                this.isContentLoaded = true;
            }
        }

        /// <summary>
        /// Updates this background.
        /// Background Layers will scroll according to the camera's movement.
        /// </summary>
        public void Update()
        {
            this.Layers.ForEach(l => l.Update());
        }

        /// <summary>
        /// Draws the background, including the gradient and all textures.
        /// </summary>
        public void Draw()
        {
            // A gradient can be drawn by stretching a 1-wide by 2-high texture over the screen.
            // The top third is the first color, and the bottom third is the second color. The middle is the actual gradient.
            // Thus, the texture needs to be made taller than the screen - the top needs to be drawn one-third of a screen higher than the screen,
            // and the height needs to be a screen height plus two-thirds of a screen height.
            float screenHeight = GameServices.ScreenSize.Y;
            float gradientTop = this.owner.Camera.Viewport.Y - (screenHeight / 3f);
            float gradientHeight = this.owner.Camera.Viewport.Height + (screenHeight * (2f / 3f));
            GameServices.SpriteBatch.Draw(this.backgroundGradient, new Rectangle((int)this.owner.Camera.Viewport.X, (int)gradientTop, (int)this.owner.Camera.ViewportSize.X, (int)gradientHeight), Color.White);

            this.Layers.ForEach(l => l.Draw());
        }

        /// <summary>
        /// Adds a given background layer to the front of the background.
        /// </summary>
        /// <param name="layer">The background layer to add.</param>
        public void AddLayerToFront(BackgroundLayer layer)
        {
            this.Layers.Add(layer);

            if (this.isContentLoaded)
            {
                layer.LoadContent();
            }
        }

        /// <summary>
        /// Returns an anonymous object containing all the objects
        /// used in serialization of this background.
        /// </summary>
        /// <returns>The objects to serialize.</returns>
        public object GetSerializableObjects()
        {
            List<object> backgroundLayerObjects = new List<object>(this.Layers.Count);
            this.Layers.ForEach(b => backgroundLayerObjects.Add(b.GetSerializableObjects()));

            return new
            {
                topColor = this.TopColor.Serialize(),
                bottomColor = this.BottomColor.Serialize(),
                layers = backgroundLayerObjects
            };
        }

        /// <summary>
        /// Converts this background into a JSON string.
        /// </summary>
        /// <returns>A JSON string containing the objects of this background.</returns>
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a background from a JSON string.
        /// </summary>
        /// <param name="json">A JSON string containing a valid background.</param>
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            // Deserialize the root objects first.
            this.TopColor = obj["topColor"].ToColor();
            this.BottomColor = obj["bottomColor"].ToColor();

            // Now, get the Layers.
            JArray layersData = (JArray)obj["layers"];

            foreach (var layerData in layersData)
            {
                BackgroundLayer layer = new BackgroundLayer(this.owner.Camera, this.owner.Bounds);
                layer.Deserialize(layerData.ToString());
                this.Layers.Add(layer);
            }
        }
    }
}
