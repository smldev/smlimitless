//-----------------------------------------------------------------------
// <copyright file="CroppedTextureMetadata.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// Defines metadata information for cropped textures.
    /// </summary>
    internal struct CroppedTextureMetadata
    {
        /// <summary>
        /// A reference to the original texture.
        /// </summary>
        internal Texture2D SourceTexture;

        /// <summary>
        /// The area of the original texture that was cropped.
        /// </summary>
        internal Rectangle SourceRectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="CroppedTextureMetadata"/> struct.
        /// </summary>
        /// <param name="sourceTexture">The original texture.</param>
        /// <param name="sourceRectangle">The area of the original texture that was cropped.</param>
        internal CroppedTextureMetadata(Texture2D sourceTexture, Rectangle sourceRectangle)
        {
            this.SourceTexture = sourceTexture;
            this.SourceRectangle = sourceRectangle;
        }
    }
}