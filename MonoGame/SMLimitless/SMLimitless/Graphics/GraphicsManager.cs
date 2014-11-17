//-----------------------------------------------------------------------
// <copyright file="GraphicsManager.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
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
    /// <summary>
    /// Handles low-level graphics loading and caching.
    /// </summary>
    public static class GraphicsManager
    {
        /// <summary>
        /// A dictionary containing a list of all loaded textures and their file paths.
        /// </summary>
        private static Dictionary<string, Texture2D> loadedTextures;

        /// <summary>
        /// A dictionary containing all cropped textures and their associated metadata.
        /// </summary>
        private static Dictionary<CroppedTextureMetadata, Texture2D> croppedTextures;

        /// <summary>
        /// A dictionary containing all loaded graphics objects and their image file paths.
        /// </summary>
        private static Dictionary<string, IGraphicsObject> loadedObjects;

        /// <summary>
        /// Initializes static members of the <see cref="GraphicsManager"/> class.
        /// </summary>
        static GraphicsManager()
        {
            loadedTextures = new Dictionary<string, Texture2D>();
            croppedTextures = new Dictionary<CroppedTextureMetadata, Texture2D>();
            loadedObjects = new Dictionary<string, IGraphicsObject>();
        }

        /// <summary>
        /// Loads a texture from any PNG image.
        /// </summary>
        /// <param name="filePath">The file path to the image.</param>
        /// <returns>A texture loaded from the image at the file path.</returns>
        public static Texture2D LoadTextureFromFile(string filePath)
        {
            if (!filePath.EndsWith(".png")) 
            { 
                throw new ArgumentException("Tried to load an image that was not a PNG.", "filePath"); 
            }

            if (!File.Exists(filePath)) 
            { 
                throw new FileNotFoundException(string.Format("The file at {0} does not exist."), filePath); 
            }

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
        /// Loads an instance of IGraphicsObject from a given file path.
        /// If a text file with the same name is in the same folder,
        /// that will be used to determine what kind of graphics object
        /// it is. If no text file is present, the object is assumed to
        /// be static. Otherwise, the type (animated, complex) depends
        /// on what the first line of the file is.
        /// </summary>
        /// <param name="filePath">The path to the image of the graphics object.</param>
        /// <returns>A loaded IGraphicsObject instance.</returns>
        public static IGraphicsObject LoadGraphicsObject(string filePath)
        {
            if (!loadedObjects.ContainsKey(filePath))
            {
                // We'll assume we have the right path (considering graphics overrides).
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                string directoryName = new FileInfo(filePath).DirectoryName;
                string configPath = Path.Combine(directoryName, string.Concat(fileNameWithoutExt, ".txt"));

                if (!File.Exists(configPath))
                {
                    // No configuration, so the object is probably static
                    var result = new StaticGraphicsObject();
                    result.Load(filePath);
                    loadedObjects.Add(filePath, result);
                    return result;
                }
                else
                {
                    // A configuration!  Let's read it to find out what it is.
                    DataReader config = new DataReader(configPath);
                    if (config[0] == "[Animated]" || config[0] == "[Animated_RunOnce]")
                    {
                        var result = new AnimatedGraphicsObject();
                        result.Load(filePath, config);
                        loadedObjects.Add(filePath, result);
                        return result;
                    }
                    else if (config[0] == "[Complex]")
                    {
                        var result = new ComplexGraphicsObject();
                        result.Load(filePath, config);
                        loadedObjects.Add(filePath, result);
                        return result;
                    }
                }
            }
            else
            {
                return loadedObjects[filePath].Clone();
            }

            return null;
        }

        /// <summary>
        /// Crops a texture out of another texture.
        /// </summary>
        /// <param name="texture">The original texture; the one to crop out of.</param>
        /// <param name="area">The area from the original to crop out.</param>
        /// <returns>The cropped texture.</returns>
        public static Texture2D Crop(this Texture2D texture, Rectangle area)
        {
            if (texture == null)
            {
                return null;
            }

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
    }
}
