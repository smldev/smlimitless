using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    public static class GraphicsManager
    {
        /// <summary>
        /// Loads a texture from any PNG image.
        /// </summary>
        /// <param name="filePath">The file path to the image.</param>
        public static Texture2D LoadFromFile(string filePath)
        {
            if (!filePath.EndsWith(".png")) { throw new ArgumentException("Tried to load an image that was not a PNG.", "filePath"); }
            if (!File.Exists(filePath)) { throw new FileNotFoundException(string.Format("The file at {0} does not exist.")); }

            using (Stream stream = File.OpenRead(filePath))
            {
                return Texture2D.FromStream(GameServices.Graphics, stream);
            }
        }

        public static Texture2D Crop(this Texture2D texture, Rectangle area)
        {
            if (texture == null)
                return null;

            RenderTarget2D target = new RenderTarget2D(texture.GraphicsDevice, area.Width, area.Height);

            SpriteBatch spriteBatch = new SpriteBatch(texture.GraphicsDevice);

            texture.GraphicsDevice.SetRenderTarget(target);
            texture.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, Vector2.Zero, area, Color.White);
            spriteBatch.End();

            texture.GraphicsDevice.SetRenderTarget(null);

            return target;
        }
    }
}
