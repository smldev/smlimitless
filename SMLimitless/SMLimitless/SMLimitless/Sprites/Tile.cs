//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type of all tiles.
    /// </summary>
    public abstract class Tile : IName, IEditorObject, IPositionable
    {
        /// <summary>
        /// Gets or sets an identification number that identifies all tiles of this kind.
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this tile used in the level editor.
        /// </summary>
        public string EditorLabel { get; set; }

        /// <summary>
        /// Gets or sets the Level that owns this tile.
        /// </summary>
        public Level Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tile is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the state of this tile.
        /// Please see http://smlimitless.wikia.com/wiki/Sprite_State for more information.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the position of this tile.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the size of this tile.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Gets a rectangle representing this tile's hitbox.
        /// </summary>
        public BoundingRectangle Hitbox
        {
            get
            {
                return new BoundingRectangle(this.Position, this.Size + this.Position);
            }
        }

        /// <summary>
        /// Gets or sets the name of this tile to be used in event scripting.  This field is optional.
        /// </summary>
        [DefaultValue(""), Description("The name of this tile to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how sprites should collide with this tile.
        /// </summary>
        [DefaultValue(0), Description("Determines how sprites should collide with this tile.")]
        public TileCollisionType Collision { get; set; }

        /// <summary>
        /// Initializes this tile.
        /// </summary>
        /// <param name="owner">The Level that owns this tile.</param>
        public virtual void Initialize(Level owner)
        {
            this.Owner = owner;
            this.IsActive = true;
            this.Name = "";
        }

        /// <summary>
        /// Loads the content for this tile.
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Updates this tile.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Draws this tile.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Handles a collision between this tile and a sprite.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this tile.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleCollision(Sprite sprite, Intersection intersect);
    }
}
