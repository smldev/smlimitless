using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    public static class GraphicsManager
    {
        private static Dictionary<String, Texture2D> loadedTextures;
        private static Dictionary<CroppedTextureMetadata, Texture2D> croppedTextures;

        public GraphicsManager()
        {
            loadedTextures = new Dictionary<string, Texture2D>();
            croppedTextures = new Dictionary<CroppedTextureMetadata, Texture2D>();
        }

        /// <summary>
        /// Loads a texture from any PNG image.
        /// </summary>
        /// <param name="filePath">The file path to the image.</param>
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            if (!filePath.EndsWith(".png")) { throw new ArgumentException("Tried to load an image that was not a PNG.", "filePath"); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(string.Format("The file at {0} does not exist.")); }

            if (!loadedTextures.ContainsKey(filePath))
            {
                using (Stream stream = File.OpenRead(filePath))
                {
                    loadedTextures.Add(filePath, Texture2D.FromStream(GameServices.Graphics, stream));
                }
            }

            return loadedTextures[filePath];
        }

        /// <summary>
        /// Loads an instance of IGraphicsObject from a given filepath.
        /// If a text file with the same name is in the same folder,
        /// that will be used to determine what kind of graphics object
        /// it is. If no text file is present, the object is assumed to
        /// be static. Otherwise, the type (animated, complex) depends
        /// on what the first line of the file is.
        /// </summary>
        public static IGraphicsObject LoadGraphicsObject(string filePath)
        {
            // We'll assume we have the right path (considering graphics overrides).
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string directoryName = new FileInfo(filePath).DirectoryName;
            string configPath = Path.Combine(directoryName, String.Concat(fileNameWithoutExt, ".txt"));

            if (!File.Exists(configPath))
            {
                // No configuration, so the object is probably static
                StaticGraphicsObject result = new StaticGraphicsObject();
                result.Load(filePath);
                return result;
            }
            else
            {
                // A configuration!  Let's read it to find out what it is.
                DataReader config = new DataReader(configPath);
                if (config[0] == "[Animated]")
                {
                    // TODO: create AnimatedGraphicsObject
                }
                else if (config[0] == "[Animated_RunOnce]")
                {
                    // TODO: create AnimatedGraphicsObject
                }
                else if (config[0] == "[Complex]")
                {
                    // TODO: you get the idea
                }
            }
        }

        public static Texture2D Crop(this Texture2D texture, Rectangle area)
        {
            if (texture == null)
                return null;

            CroppedTextureMetadata metadata = new CroppedTextureMetadata(texture, area);

            if (!croppedTextures.ContainsKey(metadata))
            {
                RenderTarget2D target = new RenderTarget2D(texture.GraphicsDevice, area.Width, area.Height);

                SpriteBatch spriteBatch = new SpriteBatch(texture.GraphicsDevice);

                texture.GraphicsDevice.SetRenderTarget(target);
                texture.GraphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin();
                spriteBatch.Draw(texture, Vector2.Zero, area, Color.White);
                spriteBatch.End();

                texture.GraphicsDevice.SetRenderTarget(null);

                croppedTextures.Add(metadata, target);

                return target;
            }

            return croppedTextures[metadata];
        }

        internal struct CroppedTextureMetadata
        {
            internal Texture2D SourceTexture;
            internal Rectangle SourceRectangle;

            internal CroppedTextureMetadata(Texture2D sourceTexture, Rectangle sourceRectangle)
            {
                this.SourceTexture = sourceTexture;
                this.SourceRectangle = sourceRectangle;
            }
        }
    }
}
