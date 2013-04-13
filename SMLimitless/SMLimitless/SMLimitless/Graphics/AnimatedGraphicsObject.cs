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
    public class AnimatedGraphicsObject
    {
        private bool isLoaded;
        private bool isContentLoaded;
        private bool isRunOnce;

        private string filePath;
        private string configFilePath;

        private List<Texture2D> textures;
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
                this.configFilePath = config.FilePath;

                if (config[0] != "[Animated]" || config[0] != "[Animated_RunOnce]")
                {
                    throw new Exception(String.Format("AnimatedGraphicsObject.Load(string, DataReader): Invalid or corrupt configuration data (expected header [Animated] or [Animated_RunOnce], got header {0})", config[0]));
                }

                Dictionary<string, string> data;
                if (config[0] == "[Animated]") data = config.ReadFullSection("[Animated]");
                else data = config.ReadFullSection("[Animated_RunOnce]");

                this.frameWidth = Int32.Parse(data["FrameWidth"]);
                this.AnimationCycleLength = Int32.Parse(data["CycleLength"]);

                this.isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                Texture2D fullTexture = GraphicsManager.LoadTextureFromFile(filePath);
            }
        }
    }
}
