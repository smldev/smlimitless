//-----------------------------------------------------------------------
// <copyright file="QuadTreeCell.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a single cell of a QuadTree.
    /// </summary>
    internal class QuadTreeCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuadTreeCell"/> class.
        /// </summary>
        internal QuadTreeCell()
        {
            this.Sprites = new List<Sprite>();
            this.Tiles = new List<Tile>();
        }

        /// <summary>
        /// Gets or sets a list of the sprites in this cell.
        /// </summary>
        internal List<Sprite> Sprites { get; set; }

        /// <summary>
        /// Gets or sets a list of the tiles in this cell.
        /// </summary>
        internal List<Tile> Tiles { get; set; }

        /// <summary>
        /// Adds a sprite to this cell.
        /// </summary>
        /// <param name="sprite">The sprite to add.</param>
        internal void Add(Sprite sprite)
        {
            this.Sprites.Add(sprite);
        }

        /// <summary>
        /// Adds a tile to this cell.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        internal void Add(Tile tile)
        {
            this.Tiles.Add(tile);
        }

        /// <summary>
        /// Removes a sprite from this cell.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        internal void Remove(Sprite sprite)
        {
            this.Sprites.Remove(sprite);
        }

        /// <summary>
        /// Removes a tile from this cell.
        /// </summary>
        /// <param name="tile">The tile to remove.</param>
        internal void Remove(Tile tile)
        {
            this.Tiles.Remove(tile);
        }
    }
}