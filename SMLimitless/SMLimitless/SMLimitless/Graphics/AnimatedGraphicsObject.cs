using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    public class AnimatedGraphicsObject : IGraphicsObject
    {
        private const float msFrameLength = 1000f / 60f; // precision is nice

        private bool isLoaded;
        private bool isContentLoaded;
        private bool isRunOnce;
        public bool IsRunning { get; set; }

        private string filePath;
        private string configFilePath;

        private List<Texture2D> textures;
        private int frameCount = -1;
        private int renderedFramesElapsed;
        private int frameIndex;
        private int frameWidth; // measured in pixels

        /// <summary>
        /// The time, measured in seconds, for the animation to play through all the frames.
        /// </summary>
        public decimal AnimationCycleLength { get; set; }
        private int frameTime
        {
            get
            {
                if (textures == null || textures.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return (int)(AnimationCycleLength * 60m) / textures.Count;
                }
            }
        }

        public AnimatedGraphicsObject()
        {
            textures = new List<Texture2D>();
            IsRunning = true;
        }

        public void Load(string filePath)
        {
            throw new Exception("AnimatedGraphicsObject.Load(string): Use the overload Load(string, DataReader) instead.");
        }

        /// <summary>
        /// Loads an instance of an AnimatedGraphicsObject from the given file path and the given configuration.
        /// </summary>
        public void Load(string filePath, DataReader config)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                configFilePath = config.FilePath;

                if (config[0] != "[Animated]" && config[0] != "[Animated_RunOnce]")
                {
                    throw new Exception(String.Format("AnimatedGraphicsObject.Load(string, DataReader): Invalid or corrupt configuration data (expected header [Animated] or [Animated_RunOnce], got header {0})", config[0]));
                }

                Dictionary<string, string> data;
                if (config[0] == "[Animated]") data = config.ReadFullSection("[Animated]");
                else
                {
                    data = config.ReadFullSection("[Animated_RunOnce]");
                    isRunOnce = true;
                }

                frameWidth = Int32.Parse(data["FrameWidth"]);
                AnimationCycleLength = Decimal.Parse(data["CycleLength"]);

                isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                Texture2D fullTexture = GraphicsManager.LoadTextureFromFile(filePath);
                int frameHeight = fullTexture.Height;

                if (fullTexture.Width % frameWidth != 0) throw new Exception("AnimatedGraphicsObject.LoadContent(): The specified frame width for this texture is invalid.");

                for (int x = 0; x < fullTexture.Width; x += frameWidth)
                {
                    textures.Add(GraphicsManager.Crop(fullTexture, new Rectangle(x, 0, frameWidth, frameHeight)));
                    frameCount++;
                }
                isContentLoaded = true;
            }
        }

        public void Update()
        {
            if (IsRunning)
            {
                renderedFramesElapsed++;
                if (renderedFramesElapsed == frameTime)
                {
                    if (frameIndex == frameCount)
                    {
                        if (isRunOnce)
                        {
                            IsRunning = false;
                            return;
                        }
                        else frameIndex = 0;
                    }
                    else frameIndex++;

                    renderedFramesElapsed = 0;
                }
            }
        }

        public void Draw(Vector2 position, Color color)
        {
            GameServices.SpriteBatch.Draw(textures[frameIndex], position, color);
        }

        public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects)
        {
            GameServices.SpriteBatch.Draw(textures[frameIndex], position, color, spriteEffects);
        }

        public void Draw(Vector2 position, Color color, bool debug)
        {
            Draw(position, color);
            if (debug)
            {
                GameServices.SpriteBatch.DrawString(GameServices.DebugFontSmall, frameIndex.ToString(), position, Color.White);
            }
        }

        public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects, bool debug)
        {
            Draw(position, color, spriteEffects);
            if (debug)
            {
                GameServices.SpriteBatch.DrawString(GameServices.DebugFontSmall, frameIndex.ToString(), position, Color.White);
            }
        }

        public IGraphicsObject Clone()
        {
            var clone = new AnimatedGraphicsObject();
            clone.filePath = filePath;
            clone.configFilePath = configFilePath;
            clone.textures = textures;
            clone.frameCount = frameCount;
            clone.frameWidth = frameWidth;
            clone.AnimationCycleLength = AnimationCycleLength;
            clone.isRunOnce = isRunOnce;
            clone.isLoaded = true;
            clone.isContentLoaded = true;
            return clone;
        }
    }
}
