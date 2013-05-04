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
    public class StaticGraphicsObject : IGraphicsObject
    {
        private bool isLoaded;
        private bool isContentLoaded;
        private string filePath;
        private Texture2D texture;

        // ComplexGraphicsObjects required fields
        Rectangle cgoSourceRect;
        ComplexGraphicsObject cgoOwner;

        public StaticGraphicsObject()
        {
        }

        public void Load(string filePath)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                isLoaded = true;
            }
        }

        public void Load(string filePath, DataReader config)
        {
            throw new Exception("StaticGraphicsObject.Load(string, DataReader): Static objects do not accepts configuration files.  Please use Load(string) instead.");
        }

        internal void Load(Dictionary<string, string> section, ComplexGraphicsObject owner)
        {
            if (!isLoaded)
            {
                cgoSourceRect = Vector2Extensions.Parse(section["Frame"]).ToRectangle(owner.FrameSize);
                filePath = owner.FilePath;
                cgoOwner = owner;
                isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                texture = GraphicsManager.LoadTextureFromFile(filePath);
                isContentLoaded = true;
            }
        }

        internal void LoadContentCGO(Texture2D fileTexture)
        {
            if (isLoaded && !isContentLoaded && cgoSourceRect != Rectangle.Empty)
            {
                texture = fileTexture.Crop(cgoSourceRect);
                isContentLoaded = true;
            }
        }

        public void Update()
        {
        }

        public void Draw(Vector2 position, Color color)
        {
            GameServices.SpriteBatch.Draw(texture, position, color);
        }

        public void Draw(Vector2 position, Color color, SpriteEffects effects)
        {
            GameServices.SpriteBatch.Draw(texture, position, color, effects);
        }

        public IGraphicsObject Clone()
        {
            var clone = new StaticGraphicsObject();
            clone.texture = this.texture;
            clone.filePath = this.filePath;
            clone.isLoaded = true;
            clone.isContentLoaded = true;
            return clone;
        }
    }
}
