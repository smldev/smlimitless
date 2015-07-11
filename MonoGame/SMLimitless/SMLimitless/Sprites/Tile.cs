//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type of all tiles.
    /// </summary>
    public abstract class Tile : IName, IEditorObject, IPositionable
    {
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
        public Section Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this tile is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the state of the tile when it was first loaded into the level.
        /// </summary>
        public string InitialState { get; internal set; }

        /// <summary>
        /// Gets or sets a string indicating the state of this tile.
        /// Please see http://smlimitless.wikia.com/wiki/Sprite_State for more information.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the position of the tile when it was first loaded into the level.
        /// </summary>
        public Vector2 InitialPosition { get; protected internal set; }

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
        /// Gets or sets a value that determines how sprites should collide with this tile.
        /// </summary>
        [DefaultValue(0), Description("Determines how sprites should collide with this tile.")]
        public TileCollisionType Collision { get; set; }

        /// <summary>
        /// Gets or sets the name of this tile to be used in event scripting.  This field is optional.
        /// </summary>
        [DefaultValue(""), Description("The name of this tile to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the graphics resource used by this tile.
        /// </summary>
        public string GraphicsResourceName { get; protected internal set; }

        /// <summary>
        /// Initializes this tile.
        /// </summary>
        /// <param name="owner">The Section that owns this tile.</param>
        public virtual void Initialize(Section owner)
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// An abstract method which is called when a sprite intersects this tile.
        /// </summary>
        /// <param name="sprite">The sprite that intersected this tile.</param>
        public virtual void HandleCollision(Sprite sprite)
        {
            this.HandleCollision(sprite, this.Hitbox.GetCollisionResolution(sprite.Hitbox));
        }

        /// <summary>
        /// Handles a collision between this tile and a sprite.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this tile.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleCollision(Sprite sprite, Vector2 intersect);

		public Tile Clone()
		{
			Tile clone = AssemblyManager.GetTileByFullName(this.GetType().FullName);

			clone.EditorLabel = this.EditorLabel.SafeCopy();
			clone.Owner = this.Owner;
			clone.IsActive = this.IsActive;
			clone.InitialState = this.InitialState.SafeCopy();
			clone.State = this.State.SafeCopy();
			clone.Position = this.Position;
			clone.Size = this.Size;
			clone.Collision = this.Collision;
			clone.Name = this.Name.SafeCopy();

			return clone;
		}

        /// <summary>
        /// Loads key custom objects from the level file.
        /// </summary>
        /// <param name="customObjects">An object containing key custom objects.</param>
        public abstract void DeserializeCustomObjects(JsonHelper customObjects);
    }
}
