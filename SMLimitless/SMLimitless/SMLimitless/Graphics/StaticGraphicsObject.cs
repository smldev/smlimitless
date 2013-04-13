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

        public StaticGraphicsObject()
        {
        }

        public void Load(string filePath)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                this.isLoaded = true;
            }
        }

        public void Load(string filePath, DataReader config)
        {
            throw new Exception("StaticGraphicsObject.Load(string, DataReader): Static objects do not accepts configuration files.  Please use Load(string) instead.");
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                this.texture = GraphicsManager.LoadTextureFromFile(this.filePath);
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
    }
}
