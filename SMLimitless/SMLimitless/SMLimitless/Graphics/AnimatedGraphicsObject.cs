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
        public bool IsRunOnce { get; internal set; }
        public bool IsRunning { get; set; }

        private string filePath;
        private string configFilePath;

        private List<Texture2D> textures;
        private int frameCount = -1;
        private int renderedFramesElapsed;
        private int frameIndex;
        private int frameWidth; // measured in pixels

        // ComplexGraphicsObject required fields
        internal List<Rectangle> cgoSourceRects;
        ComplexGraphicsObject cgoOwner;

        private decimal animationCycleLength;
        /// <summary>
        /// The time, measured in seconds, for the animation to play through all the frames.
        /// </summary>
        public decimal AnimationCycleLength
        {
            get
            {
                return animationCycleLength;
            }
            set
            {
                if (value < ((1m / 60m) * textures.Count))
                {
                    animationCycleLength = ((1m / 60m) * textures.Count);
                }
                else
                {
                    animationCycleLength = value;
                }
            }
        }
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
            cgoSourceRects = new List<Rectangle>();
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
                    IsRunOnce = true;
                }

                frameWidth = Int32.Parse(data["FrameWidth"]);
                AnimationCycleLength = Decimal.Parse(data["CycleLength"]);

                isLoaded = true;
            }
        }

        /// <summary>
        /// Loads an AnimatedGraphicsObjects from a configuration section in a ComplexGraphicsObject.
        /// </summary>
        /// <param name="section">The section from the CGO configuration that specifies this object.</param>
        /// <param name="owner">The CGO that owns this object.</param>
        internal void Load(Dictionary<string, string> section, ComplexGraphicsObject owner)
        {
            if (!isLoaded)
            {
                int frames = Int32.Parse(section["Frames"]);
                Vector2 frameSize = owner.FrameSize;
                filePath = owner.FilePath;
                for (int i = 0; i < frames; i++)
                {
                    cgoSourceRects.Add(Vector2Extensions.Parse(section[String.Concat("Frame", i)]).ToRectangle(frameSize));
                }
                if (section["Type"] == "animated_runonce")
                {
                    IsRunOnce = true;
                }
                AnimationCycleLength = Decimal.Parse(section["CycleLength"]);
                cgoOwner = owner;
                frameCount = frames - 1;
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

        internal void LoadContentCGO(Texture2D fileTexture)
        {
            if (isLoaded && !isContentLoaded && cgoSourceRects.Any())
            {
                foreach (Rectangle sourceRect in cgoSourceRects)
                {
                    textures.Add(fileTexture.Crop(sourceRect));
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
                        if (IsRunOnce)
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
            GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, this.frameTime.ToString(), new Vector2(0, 20), Color.White);
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

        /// <summary>
        /// Adjusts the time it takes for this animated object
        /// to complete one loop through its frames.
        /// </summary>
        /// <param name="newCycleLength">The time, in seconds, each loop takes.</param>
        public void SetSpeed(decimal newCycleLength)
        {
            this.AnimationCycleLength = newCycleLength;
            this.renderedFramesElapsed = 0;
        }

        /// <summary>
        /// Adjusts how many rendered frames each
        /// frame of this object is shown for.
        /// </summary>
        /// <param name="newFrameTime">How many rendered frames each frame is shown for.</param>
        public void SetSpeed(int newFrameTime)
        {
            this.AnimationCycleLength = 60m / textures.Count;
            this.renderedFramesElapsed = 0;
        }

        /// <summary>
        /// Adjusts the speed of the animation of this
        /// object by percentage. Rounded to the closest
        /// frame boundary (usually 1/60th of a second).
        /// </summary>
        /// <param name="percentage">The percentage by which to adjust the animation speed.</param>
        public void AdjustSpeed(float percentage)
        {
            percentage /= 100f;
            Decimal addend = this.AnimationCycleLength * (Decimal)percentage;
            this.AnimationCycleLength += addend;
            this.renderedFramesElapsed = 0;

            // Round it to the nearest frame boundary if necessary.
            if ((this.AnimationCycleLength * 60m) % 1 != 0)
            {
                decimal cycleInFrames = this.AnimationCycleLength * 60m;
                if (percentage > 0f) // If we're slowing down
                {
                    cycleInFrames = NumericExtensions.RoundUp(cycleInFrames);
                }
                else if (percentage < 0f) // If we're speeding up
                {
                    cycleInFrames = NumericExtensions.RoundDown(cycleInFrames);
                }
                else // somehow we're zero
                {
                    return;
                }
                this.AnimationCycleLength = cycleInFrames / 60m;
            }
        }

        public void Reset(bool startRunning)
        {
            renderedFramesElapsed = 0;
            frameIndex = 0;
            if (startRunning) IsRunning = true;
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
            clone.IsRunOnce = IsRunOnce;
            clone.isLoaded = true;
            clone.isContentLoaded = true;
            return clone;
        }
    }
}
