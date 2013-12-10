using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Graphics;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    public sealed class BackgroundLayer : ISerializable
    {
        private string backgroundTextureResourceName;

        private IGraphicsObject backgroundTexture;
        private Camera2D camera;
        private BoundingRectangle sectionBounds;
        private Vector2 oldCameraPosition;

        public BackgroundScrollDirection ScrollDirection { get; private set; }
        public float ScrollRate { get; private set; }
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

        private Vector2 textureSize;
        private Vector2 delta;

        public BackgroundLayer(Camera2D ownerCamera, BoundingRectangle sectionBounds)
        {
            this.camera = ownerCamera;
            this.sectionBounds = sectionBounds;
        }

        public void Initialize(string backgroundTextureResourceName, BackgroundScrollDirection scrollDirection, float scrollRate)
        {
            // TODO: Remove this code (and the arguments)
            this.backgroundTextureResourceName = backgroundTextureResourceName;
            this.backgroundTexture = ContentPackageManager.GetGraphicsResource(backgroundTextureResourceName);
            this.ScrollDirection = scrollDirection;
            this.ScrollRate = scrollRate;
        }

        public void LoadContent()
        {
            if (this.backgroundTexture == null)
            {
                throw new Exception("BackgroundLayer.LoadContent(): Tried to load content before initializing the object. Please call Initialize() first.");
            }

            this.backgroundTexture.LoadContent();
            this.textureSize = this.backgroundTexture.GetSize();
        }

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
                    this.Delta = new Vector2(this.Delta.X + cameraDisplacement.X * this.ScrollRate, 0f);
                }
                else if (this.ScrollDirection == BackgroundScrollDirection.Vertical)
                {
                    this.Delta = new Vector2(0f, this.Delta.Y + cameraDisplacement.Y * this.ScrollRate);
                }
                else if (this.ScrollDirection == BackgroundScrollDirection.Both)
                {
                    this.Delta = new Vector2(this.Delta.X + cameraDisplacement.X * this.ScrollRate, this.Delta.Y + cameraDisplacement.Y * this.ScrollRate);
                }
            }

            this.oldCameraPosition = this.camera.Position;
            this.backgroundTexture.Update();
        }

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

        private List<Vector2> GetTextureTileDrawPoints(Vector2 baseTextureDrawPosition)
        {
            Vector2 topLeftCorner = baseTextureDrawPosition;
            while (topLeftCorner.X > this.camera.Viewport.X) { topLeftCorner.X -= this.textureSize.X; }
            while (topLeftCorner.Y > this.camera.Viewport.Y) { topLeftCorner.Y -= this.textureSize.Y; }

            Vector2 bottomRightCorner = baseTextureDrawPosition;
            while (bottomRightCorner.X < this.camera.Viewport.Right) { bottomRightCorner.X += this.textureSize.X; }
            while (bottomRightCorner.Y < this.camera.Viewport.Bottom) { bottomRightCorner.Y += this.textureSize.Y; }

            List<Vector2> result = new List<Vector2>();
            for (float x = topLeftCorner.X; x < bottomRightCorner.X; x += textureSize.X)
            {
                for (float y = topLeftCorner.Y; y < bottomRightCorner.Y; y += textureSize.Y)
                {
                    result.Add(new Vector2(x, y));
                }
            }

            return result;
        }

        public object GetSerializableObjects()
        {
            return new
            {
                resourceName = this.backgroundTextureResourceName,
                scrollDirection = (int)this.ScrollDirection,
                scrollRate = this.ScrollRate
            };
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            string resourceName = (string)obj["resourceName"];
            BackgroundScrollDirection direction = (BackgroundScrollDirection)(int)obj["scrollDirection"];
            float scrollRate = (float)obj["scrollRate"];

            this.backgroundTextureResourceName = resourceName;
            this.backgroundTexture = ContentPackageManager.GetGraphicsResource(backgroundTextureResourceName);
            this.ScrollDirection = direction;
            this.ScrollRate = scrollRate;
        }
    }
}
