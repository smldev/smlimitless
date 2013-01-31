using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void LoadFromMetadata(string Metadata)
        {
            if (!isLoaded)
            {
                this.metadata = Metadata;
                var split = metadata.Split('>');
                if (split[0] == "anim-single")
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
                else if (split[0] == "anim-spritesheet")
                {
                    // Metadata format: anim-spritesheet>“//filepath/image.png”,width,height,tileNumber1,tileNumber2,tileNumber3,...
                    throw new NotImplementedException();
                }
                else if (split[0] == "anim-spritesheet_r")
                {
                    // Metadata format: anim-spritesheet_r>”//filepath/image.png”,[x,y,width,height],[x,y,width,height],...,frameTime
                    throw new NotImplementedException();
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
                if (this.frameLength != 0) // if we're loading from a single file
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
                // eventual spritesheet code here
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

        public void Draw(Vector2 position, Color color, bool debug)
        {
            SpriteBatch spriteBatch = GameServices.SpriteBatch;
            spriteBatch.Draw(textures[currentTextureIndex], position, color);

            if (debug)
            {
                spriteBatch.DrawString(GameServices.DebugFontSmall, currentTextureIndex.ToString(), position, Color.White);
            }

            UpdateFrameCounter();
        }
    }
}
