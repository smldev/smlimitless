using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    public class StaticGraphicsObject
    {
        private string Metadata;

        private bool isLoaded;
        private bool isContentLoaded;
        private string filePath;
        private Texture2D texture;

        public StaticGraphicsObject()
        {
        }

        public void LoadFromMetadata(string metadata)
        {
            // Metadata format: static-single:“//filepath/image.png”
            if (!isLoaded)
            {
                var split = metadata.Split('>');
                this.filePath = split[1].Substring(1, split[1].Length - 2); // remove the quotes.
                isLoaded = true;
            }
        }

        public void Load(string filePath)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                texture = GraphicsManager.LoadFromFile(this.filePath);
                isContentLoaded = true;
            }
        }

        public void Draw(Vector2 position, Color color)
        {
            SpriteBatch spriteBatch = GameServices.SpriteBatch;
            spriteBatch.Draw(this.texture, position, color);
        }
    }
}
