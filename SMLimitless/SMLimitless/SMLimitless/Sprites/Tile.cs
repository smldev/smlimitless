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
        /// Gets the name of the category that this tile is
        /// categorized within in the level editor.
        /// </summary>
        public abstract string EditorCategory { get; }

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
        public virtual ICollidableShape Hitbox
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
        /// <param name="contentResourceName">The name of the content resource that is used for this tile's graphics.</param>
        public virtual void Initialize(Level owner, string contentResourceName)
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
        /// Determines if a given sprite intersects this tile.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <returns>True if the sprite intersects this tile, false if otherwise.</returns>
        /// <remarks>This method accounts for the different tile collision types.</remarks>
        public virtual bool Intersects(Sprite sprite)
        {
            bool intersects = this.Hitbox.Intersects(sprite.Hitbox);

            switch (this.Collision)
            {
                case TileCollisionType.Solid:
                    return intersects;
                case TileCollisionType.TopSolid:
                    if (sprite.Velocity.Y > 0f && sprite.PreviousPosition.Y + sprite.Size.Y <= this.Hitbox.Bounds.Top)
                    {
                        return intersects;
                    }

                    return false;
                case TileCollisionType.BottomSolid:
                    if (sprite.Velocity.Y < 0f && sprite.PreviousPosition.Y >= this.Hitbox.Bounds.Bottom)
                    {
                        return intersects;
                    }

                    return false;
                case TileCollisionType.LeftSolid:
                    if (sprite.Velocity.X > 0f && sprite.PreviousPosition.X + sprite.Size.X <= this.Hitbox.Bounds.Left)
                    {
                        return intersects;
                    }

                    return false;
                case TileCollisionType.RightSolid:
                    if (sprite.Velocity.X < 0f && sprite.PreviousPosition.X >= this.Hitbox.Bounds.Right)
                    {
                        return intersects;
                    }

                    return false;
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Returns the distance to resolve a given colliding sprite by
        /// so that it will be moved out of this tile.
        /// </summary>
        /// <param name="sprite">The sprite to resolve.</param>
        /// <returns>A resolution containing the distance.</returns>
        /// <remarks>This method accounts for the different tile collision types.</remarks>
        internal virtual Vector2 GetCollisionResolution(Sprite sprite)
        {
            Vector2 resolution = this.Hitbox.GetCollisionResolution(sprite.Hitbox).ResolutionDistance;

            switch (this.Collision)
            {
                case TileCollisionType.Solid:
                    return resolution;
                case TileCollisionType.TopSolid:
                    if (sprite.PreviousPosition.Y + this.Size.Y <= this.Hitbox.Bounds.Top)
                    {
                        return resolution;
                    }

                    return Vector2.Zero;
                case TileCollisionType.BottomSolid:
                    if (sprite.PreviousPosition.Y >= this.Hitbox.Bounds.Bottom)
                    {
                        return resolution;
                    }

                    return Vector2.Zero;
                case TileCollisionType.LeftSolid:
                    if (sprite.PreviousPosition.X + sprite.Size.X <= this.Hitbox.Bounds.Left)
                    {
                        return resolution;
                    }

                    return Vector2.Zero;
                case TileCollisionType.RightSolid:
                    if (sprite.PreviousPosition.X >= this.Hitbox.Bounds.Right)
                    {
                        return resolution;
                    }

                    return Vector2.Zero;
                default:
                    return Vector2.Zero;
            }
        }

        /// <summary>
        /// An abstract method which is called when a sprite intersects this tile.
        /// </summary>
        /// <param name="sprite">The sprite that intersected this tile.</param>
        public virtual void HandleCollision(Sprite sprite)
        {
            this.HandleCollision(sprite, this.Hitbox.GetCollisionResolution(sprite.Hitbox).ResolutionDistance);
        }

        /// <summary>
        /// Handles a collision between this tile and a sprite.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this tile.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleCollision(Sprite sprite, Vector2 intersect);
    }
}
