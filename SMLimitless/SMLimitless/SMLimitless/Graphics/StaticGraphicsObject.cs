using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;

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
            this.Metadata = metadata;
            if (!isLoaded)
            {
                var split = metadata.Split('>');
                if (split[0] == "static-single")
                {
                    // Metadata format: static-single>“//filepath/image.png”
                    this.filePath = MetadataHelpers.TrimQuotes(split[1]);
                    isLoaded = true;
                }
                else if (split[0].Contains("static-spritesheet"))
                {
                    SpritesheetManager.LoadFromMetadata(metadata);
                    isLoaded = true;
                }
                else if (split[0].Contains("static-spritesheet_r"))
                {
                    SpritesheetManager.LoadFromMetadata(metadata);
                    isLoaded = true;
                }
                else
                {
                    throw new Exception("StaticGraphicsObject.LoadFromMetadata: Invalid metadata or metadata type.");
                }
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
                if (Metadata.Contains("spritesheet_r"))
                {
                    // Metadata format: static-spritesheet_r(-nosize)>“//filepath/image.png”,width,height,[x:y:width:height]
                    string filePath;
                    Rectangle sourceRect;

                    var data = Metadata.Split('>')[1].Split(',');
                    filePath = MetadataHelpers.TrimQuotes(data[0]);
                    if (Metadata.Contains("nosize")) sourceRect = MetadataHelpers.ExtractSingleRectangle(data[1]);
                    else sourceRect = MetadataHelpers.ExtractSingleRectangle(data[3]);

                    texture = SpritesheetManager.GetTile(filePath, sourceRect);
                }
                else if (Metadata.Contains("spritesheet"))
                {
                    // Metadata format: static-spritesheet(-nosize)>“//filepath/image.png”,width,height,tileNumber
                    string filePath;
                    int tileIndex;

                    var data = Metadata.Split('>')[1].Split(',');
                    filePath = MetadataHelpers.TrimQuotes(data[0]);
                    if (Metadata.Contains("nosize")) tileIndex = Int32.Parse(data[1]);
                    else tileIndex = Int32.Parse(data[3]);

                    texture = SpritesheetManager.GetTile(filePath, tileIndex);
                }
                else
                {
                    texture = GraphicsManager.LoadFromFile(this.filePath);
                }
                isContentLoaded = true;
            }
        }

        public void Draw(Vector2 position, Color color, SpriteEffects effects)
        {
            SpriteBatch spriteBatch = GameServices.SpriteBatch;
            spriteBatch.Draw(this.texture, position, color, effects);
        }

        public void Draw(Vector2 position, Color color)
        {
            this.Draw(position, color, SpriteEffects.None);
        }
    }
}
