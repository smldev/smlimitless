//-----------------------------------------------------------------------
// <copyright file="SlopedTile.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type for all tiles with triangular hitboxes.
    /// </summary>
    public abstract class SlopedTile : Tile
    {
        private Vector2 size;
        private Vector2 position;

        /// <summary>
        /// Gets or sets which sides of this tile are sloped.
        /// </summary>
        public RtSlopedSides SlopedSides { get; protected set; }

        /// <summary>
        /// Gets a triangle representing this tile's hitbox.
        /// </summary>
        //public override ICollidableShape Hitbox { get; private set; }

        public override Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.Hitbox = new RightTriangle(new BoundingRectangle(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y), this.SlopedSides);
            }
        }

        public override Vector2 Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                this.Hitbox = new RightTriangle(new BoundingRectangle(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y), this.SlopedSides);
            }
        }
    }
}
