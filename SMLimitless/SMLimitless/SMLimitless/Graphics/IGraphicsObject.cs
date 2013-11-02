//-----------------------------------------------------------------------
// <copyright file="IGraphicsObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// Defines a graphics object that can be drawn to the screen.
    /// </summary>
    public interface IGraphicsObject
    {
        /// <summary>
        /// Loads a graphics object.
        /// </summary>
        /// <param name="filePath">The file path of the image.</param>
        void Load(string filePath);

        /// <summary>
        /// Loads a graphics object.
        /// </summary>
        /// <param name="filePath">The file path of the image.</param>
        /// <param name="config">A DataReader containing the configuration file for this object.</param>
        void Load(string filePath, DataReader config);

        /// <summary>
        /// Loads the content for this object.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Updates this object.
        /// </summary>
        void Update();

        /// <summary>
        /// Draws this object to the screen.
        /// </summary>
        /// <param name="position">The position on the screen to draw this object at.</param>
        /// <param name="color">The color to shade the object. Use Color.White for no shading.</param>
        void Draw(Vector2 position, Color color);

        /// <summary>
        /// Draws this object to the screen.
        /// </summary>
        /// <param name="position">The position on the screen to draw this object at.</param>
        /// <param name="color">The color to shade the object. Use Color.White for no shading.</param>
        /// <param name="effects">Defines sprite mirroring options.</param>
        void Draw(Vector2 position, Color color, SpriteEffects effects);

        /// <summary>
        /// Clones this graphics object.
        /// The texture(s) are not cloned, merely their reference is copied.
        /// </summary>
        /// <returns>A cloned object.</returns>
        IGraphicsObject Clone();

        /// <summary>
        /// Gets the size of the texture within this graphics object.
        /// </summary>
        /// <returns>A Vector2 representing the size in pixels of the texture.</returns>
        Vector2 GetSize();
    }
}
