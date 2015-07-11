//-----------------------------------------------------------------------
// <copyright file="BackgroundLayer.cs" company="The Limitless Development Team">
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
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A single layer of a section background.
    /// Contains one image that scrolls at a given rate.
    /// </summary>
    public sealed class BackgroundLayer
    {
        /// <summary>
        /// The background texture of this layer.
        /// </summary>
        private IGraphicsObject backgroundTexture;

        /// <summary>
        /// A reference to the camera that this background is following.
        /// </summary>
        private Camera2D camera;

		/// <summary>
        /// The overall displacement of this layer.
        /// </summary>
        private Vector2 delta;

		/// <summary>
        /// The camera's position in the last frame.
        /// </summary>
        private Vector2 oldCameraPosition;

        /// <summary>
        /// The bounds of the section that this layer is contained within.
        /// </summary>
        private BoundingRectangle sectionBounds;

		/// <summary>
        /// The size of the texture in pixels.
        /// </summary>
        private Vector2 textureSize;

		/// <summary>
		/// Determines whether the content for this background layer has been loaded.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		/// Gets or sets the resource name of the background texture.
		/// </summary>
		internal string BackgroundTextureResourceName { get; set; }

		/// <summary>
        /// Gets the direction that this background layer scrolls.
        /// </summary>
        public BackgroundScrollDirection ScrollDirection { get; internal set; }

        /// <summary>
        /// Gets the rate that this background scrolls.
        /// A value of 1 will scroll as fast as the camera.
        /// Values greater or lesser than 1 will scroll faster or slower than the camera, respectively.
        /// </summary>
        public float ScrollRate { get; internal set; }

        /// <summary>
        /// Gets the overall displacement of this layer.
        /// </summary>
        public Vector2 Delta
        {
            get
            {
                return this.delta;
            }

            private set
            {
                this.delta = value.Mod(this.textureSize);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundLayer"/> class.
        /// </summary>
        /// <param name="ownerCamera">The camera of the section that contains this layer.</param>
        /// <param name="sectionBounds">The bounds of the section that contains this layer.</param>
        public BackgroundLayer(Camera2D ownerCamera, BoundingRectangle sectionBounds)
        {
            this.camera = ownerCamera;
            this.sectionBounds = sectionBounds;
        }

        /// <summary>
        /// Initializes this layer.
        /// </summary>
        public void Initialize()
        {
			this.backgroundTexture = ContentPackageManager.GetGraphicsResource(this.BackgroundTextureResourceName);
        }

        /// <summary>
        /// Loads the content of this layer.
        /// </summary>
        public void LoadContent()
        {
            if (!this.isContentLoaded)
            {
                if (this.backgroundTexture == null)
                {
                    throw new InvalidOperationException("BackgroundLayer.LoadContent(): Tried to load content before initializing the object. Please call Initialize() first.");
                }

                this.backgroundTexture.LoadContent();
                this.textureSize = this.backgroundTexture.GetSize();

                this.isContentLoaded = true;
            }
        }

        /// <summary>
        /// Updates this layer's position based on the camera's movement.
        /// </summary>
        public void Update()
        {
            if (this.ScrollDirection == BackgroundScrollDirection.Fixed)
            {
                return;
            }
            else
            {
                Vector2 cameraDisplacement = this.camera.Position - this.oldCameraPosition;
                if (this.ScrollDirection == BackgroundScrollDirection.Horizontal)
                {
                    this.Delta = new Vector2(this.Delta.X + (cameraDisplacement.X * this.ScrollRate), 0f);
                }
                else if (this.ScrollDirection == BackgroundScrollDirection.Vertical)
                {
                    this.Delta = new Vector2(0f, this.Delta.Y + (cameraDisplacement.Y * this.ScrollRate));
                }
                else if (this.ScrollDirection == BackgroundScrollDirection.Both)
                {
                    this.Delta = new Vector2(this.Delta.X + (cameraDisplacement.X * this.ScrollRate), this.Delta.Y + (cameraDisplacement.Y * this.ScrollRate));
                }
            }

            this.oldCameraPosition = this.camera.Position;
            this.backgroundTexture.Update();
        }

        /// <summary>
        /// Draws this layer to the screen.
        /// </summary>
        public void Draw()
        {
            if (this.ScrollDirection == BackgroundScrollDirection.Fixed)
            {
                Vector2 textureCenter = this.textureSize / 2;
                Vector2 viewportCenter = this.camera.Viewport.Center;

                this.backgroundTexture.Draw(new Vector2(viewportCenter.X - textureCenter.X, viewportCenter.Y - textureCenter.Y), Color.White);
            }
            else if (this.ScrollDirection == BackgroundScrollDirection.Horizontal)
            {
                float baseTextureDrawPositionX = this.camera.Viewport.X + -this.Delta.X;
                float baseTextureDrawPositionY;

                if (this.textureSize.Y >= this.camera.ViewportSize.Y)
                {
                    float minorAxisScrollableHeight = -this.sectionBounds.Height - this.camera.ViewportSize.Y;
                    float viewportOriginRelatingToScrollableHeight = this.camera.Position.Y / minorAxisScrollableHeight;
                    float viewportMinorAxisLine = this.camera.ViewportSize.Y * viewportOriginRelatingToScrollableHeight;
                    float textureMinorAxisLine = this.textureSize.Y * viewportOriginRelatingToScrollableHeight;
                    baseTextureDrawPositionY = -(viewportMinorAxisLine - textureMinorAxisLine) + this.camera.Viewport.Y;
                }
                else
                {
                    baseTextureDrawPositionY = this.camera.Viewport.Y;
                }

                this.backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);

                foreach (Vector2 textureDrawPosition in this.GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
                {
                    this.backgroundTexture.Draw(textureDrawPosition, Color.White);
                }
            }
            else if (this.ScrollDirection == BackgroundScrollDirection.Vertical)
            {
                float baseTextureDrawPositionX;
                float baseTextureDrawPositionY = this.camera.Viewport.Y + -this.Delta.Y;

                if (this.textureSize.X >= this.camera.ViewportSize.X)
                {
                    float minorAxisScrollableWidth = -this.sectionBounds.Width - this.camera.ViewportSize.X;
                    float viewportOriginRelatingToScrollableWidth = this.camera.Viewport.X / minorAxisScrollableWidth;
                    float viewportMinorAxisLine = this.camera.ViewportSize.X * viewportOriginRelatingToScrollableWidth;
                    float textureMinorAxisLine = this.textureSize.X * viewportOriginRelatingToScrollableWidth;
                    baseTextureDrawPositionX = -(viewportMinorAxisLine - textureMinorAxisLine) + this.camera.Viewport.X;
                }
                else
                {
                    baseTextureDrawPositionX = this.camera.Viewport.X;
                }

                this.backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);

                foreach (Vector2 textureDrawPosition in this.GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
                {
                    this.backgroundTexture.Draw(textureDrawPosition, Color.White);
                }
            }
            else if (this.ScrollDirection == BackgroundScrollDirection.Both)
            {
                float baseTextureDrawPositionX = this.camera.Viewport.X + -this.Delta.X;
                float baseTextureDrawPositionY = this.camera.Viewport.Y + -this.Delta.Y;

                this.backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);
                
                foreach (Vector2 textureDrawPosition in this.GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
                {
                    this.backgroundTexture.Draw(textureDrawPosition, Color.White);
                }
            }
        }

        /// <summary>
        /// Returns a list of all the points to which to draw the tiled background texture.
        /// </summary>
        /// <param name="baseTextureDrawPosition">A single point where the texture will be drawn.</param>
        /// <returns>A list of vectors.</returns>
        private List<Vector2> GetTextureTileDrawPoints(Vector2 baseTextureDrawPosition)
        {
            Vector2 topLeftCorner = baseTextureDrawPosition;
            while (topLeftCorner.X > this.camera.Viewport.X) { topLeftCorner.X -= this.textureSize.X; }
            while (topLeftCorner.Y > this.camera.Viewport.Y) { topLeftCorner.Y -= this.textureSize.Y; }

            Vector2 bottomRightCorner = baseTextureDrawPosition;
            while (bottomRightCorner.X < this.camera.Viewport.Right) { bottomRightCorner.X += this.textureSize.X; }
            while (bottomRightCorner.Y < this.camera.Viewport.Bottom) { bottomRightCorner.Y += this.textureSize.Y; }

            List<Vector2> result = new List<Vector2>();
            for (float x = topLeftCorner.X; x < bottomRightCorner.X; x += this.textureSize.X)
            {
                for (float y = topLeftCorner.Y; y < bottomRightCorner.Y; y += this.textureSize.Y)
                {
                    result.Add(new Vector2(x, y));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the serializable objects for this layer.
        /// </summary>
        /// <returns>An anonymous object containing the background texture's resource name, and the layer's scroll direction and rate.</returns>
		[Obsolete]
        public object GetSerializableObjects()
        {
            return new
            {
                resourceName = this.BackgroundTextureResourceName,
                scrollDirection = (int)this.ScrollDirection,
                scrollRate = this.ScrollRate
            };
        }

        /// <summary>
        /// Returns a JSON string containing this layer.
        /// </summary>
        /// <returns>A JSON string containing this layer.</returns>
		[Obsolete]
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a background layer, given a JSON string containing a background layer.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
		[Obsolete]
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            string resourceName = (string)obj["resourceName"];
            BackgroundScrollDirection direction = (BackgroundScrollDirection)(int)obj["scrollDirection"];
            float scrollRate = (float)obj["scrollRate"];

            this.BackgroundTextureResourceName = resourceName;
            this.backgroundTexture = ContentPackageManager.GetGraphicsResource(this.BackgroundTextureResourceName);
            this.ScrollDirection = direction;
            this.ScrollRate = scrollRate;
        }
    }
}
