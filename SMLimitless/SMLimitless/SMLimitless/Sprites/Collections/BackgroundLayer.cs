using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    public sealed class BackgroundLayer
    {
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

                float minorAxisScrollableHeight = -this.sectionBounds.Y - this.camera.ViewportSize.Y;
                float viewportOriginRelatingToScrollableHeight = this.camera.Position.Y / minorAxisScrollableHeight;
                float viewportMinorAxisLine = this.camera.ViewportSize.Y * viewportOriginRelatingToScrollableHeight;
                float textureMinorAxisLine = this.textureSize.Y * viewportOriginRelatingToScrollableHeight;
                float baseTextureDrawPositionY = viewportMinorAxisLine - textureMinorAxisLine;

                this.backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);
            }
        }
    }
}
