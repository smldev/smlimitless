using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
    public class AnimatedGraphicsObject
    {
        private List<Texture2D> textures;
        private string metadata;

        private string filePath;
        private int frameLength;
        private List<Rectangle> rectangles;

        private int frameTime; // the number of rendered frames to display each texture
        private int currentTextureIndex;
        private int elapsedFrames; // how many rendered frames have been drawn since we've lasted changed the current texture

        private bool isLoaded;
        private bool isContentLoaded;

        public AnimatedGraphicsObject()
        {
            textures = new List<Texture2D>();
            rectangles = new List<Rectangle>();
        }

        public void LoadFromMetadata(string metadata)
        {
            if (!isLoaded)
            {
                this.metadata = metadata;
                var split = metadata.Split('>');
                if (split[0].Contains("anim-single"))
                {
                    // Metadata format: anim-single>“//filepath/image.png”,frameLength,frameTime
                    var data = split[1].Split(',');
                    this.filePath = MetadataHelpers.TrimQuotes(data[0]);
                    this.frameLength = Int32.Parse(data[1]);
                    this.frameTime = Int32.Parse(data[2]);

                    // We can't get the rectangles now because we don't have the texture (or its width).
                    // We'll grab them on content load if frameLength is set (more than zero).
                    this.isLoaded = true;
                }
                else if (split[0].Contains("anim-spritesheet"))
                {
                    SpritesheetManager.LoadFromMetadata(metadata);
                    this.isLoaded = true;
                }
                else if (split[0].Contains("anim-spritesheet_r"))
                {
                    SpritesheetManager.LoadFromMetadata(metadata);
                    this.isLoaded = true;
                }
                else
                {
                    throw new Exception("AnimatedGraphicsObject.LoadFromMetadata: Invalid metadata or metadata type.");
                }
            }
        }

        public void Load(string FilePath, int FrameLength, int FrameTime)
        {
            this.filePath = FilePath;
            this.frameLength = FrameLength;
            this.frameTime = FrameTime;
            this.isLoaded = true;
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                if (!metadata.Contains("spritesheet"))
                {
                    Texture2D wholeTexture = GraphicsManager.LoadFromFile(this.filePath);

                    if (wholeTexture.Width % this.frameLength != 0) throw new Exception("AnimatedGraphicsObject.LoadContent: Mismatched frame width and texture width.");

                    for (int xPos = 0; xPos < wholeTexture.Width; xPos += frameLength)
                    {
                        this.rectangles.Add(new Rectangle(xPos, 0, frameLength, wholeTexture.Height));
                    }

                    foreach (Rectangle rect in rectangles)
                    {
                        textures.Add(GraphicsManager.Crop(wholeTexture, rect));
                    }

                    isLoaded = true;
                }
                else if (metadata.Contains("spritesheet_r"))
                {
                    // Metadata format: anim-spritesheet_r(-nosize)>”//filepath/image.png”,[x:y:width:height],[x:y:width:height],...,frameTime
                    var data = metadata.Split('>')[1].Split(',');
                    string filePath = MetadataHelpers.TrimQuotes(data[0]);
                    var sourceRects = MetadataHelpers.ExtractAllRectangles(data);

                    textures = SpritesheetManager.GetTiles(filePath, sourceRects);
                    frameTime = Int32.Parse(data[data.Length - 1]);
                }
                else if (metadata.Contains("spritesheet"))
                {
                    // Metadata format: anim-spritesheet>“//filepath/image.png”,width,height,frameTime,tileNumber1,tileNumber2,tileNumber3,...
                    var data = metadata.Split('>')[1].Split(',');
                    string filePath = MetadataHelpers.TrimQuotes(data[0]);
                    var tileIndexes = new List<int>();

                    frameTime = Int32.Parse(data[3]);
                    for (int i = 4; i < data.Length - 1; i++)
                    {
                        tileIndexes.Add(Int32.Parse(data[i]));
                    }

                    textures = SpritesheetManager.GetTiles(filePath, tileIndexes);
                }
            }
        }

        private void UpdateFrameCounter()
        {
            this.elapsedFrames++;
            if (this.elapsedFrames >= this.frameTime)
            {
                this.elapsedFrames = 0;

                if (this.currentTextureIndex == textures.Count - 1) currentTextureIndex = 0;
                else currentTextureIndex++;
            }
        }

        public void Draw(Vector2 position, Color color, bool debug, SpriteEffects effects)
        {
            // TODO: Add support for flipping this sprite (it's a SpriteBatch.Draw overload with a SpriteEffects param)
            SpriteBatch spriteBatch = GameServices.SpriteBatch;
            spriteBatch.Draw(textures[currentTextureIndex], position, color, effects);

            if (debug)
            {
                spriteBatch.DrawString(GameServices.DebugFontSmall, currentTextureIndex.ToString(), position, Color.White);
            }

            UpdateFrameCounter();
        }

        public void Draw(Vector2 position, Color color, bool debug)
        {
            this.Draw(position, color, debug, SpriteEffects.None);
        }
    }
}
