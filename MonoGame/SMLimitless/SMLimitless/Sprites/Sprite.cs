//-----------------------------------------------------------------------
// <copyright file="Sprite.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type for all sprites.
    /// </summary>
    public abstract class Sprite : IName, IEditorObject, IPositionable
    {
        /// <summary>
        /// A backing field for the Position property.
        /// </summary>
        private Vector2 position;

		/// <summary>
		/// A backing field for the Velocity property.
		/// </summary>
        private Vector2 velocity;

		/// <summary>
		/// Gets or sets an identification number that identifies all sprites of this kind.
		/// </summary>
		public uint ID { get; set; }

		#region State Properties (components, state, owner)
		/// <summary>
        /// Gets a list of all the components used by this sprite instance.
        /// </summary>
        protected List<SpriteComponent> Components { get; private set; }

		/// <summary>
		/// Gets the state of this sprite when it was first loaded into the level.
		/// </summary>
		public SpriteState InitialState { get; internal set; }

		/// <summary>
		/// Gets or sets a string representing the state of this sprite. Please see
		/// http://smlimitless.wikia.com/wiki/Sprite_State for more information.
		/// </summary>
		public SpriteState State { get; protected internal set; }

		/// <summary>
        /// Gets or sets the section that owns this sprite.
        /// </summary>
        public Section Owner { get; set; }
		#endregion

		#region Flags (active, embedded, ground, slope, remove)
        /// <summary>
        /// Gets or sets a value indicating whether this sprite is actively updating or not.
        /// </summary>
        public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this sprite is embedded inside of a tile.
		/// </summary>
		public bool IsEmbedded { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this sprite is on the ground.
		/// </summary>
		public bool IsOnGround
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this sprite is sitting on a slope.
		/// </summary>
		public bool IsOnSlope
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this sprite should be removed from its owner section on the next frame.
		/// </summary>
		public bool RemoveOnNextFrame { get; set; }
		#endregion

		#region Physics Properties (size, position, velocity, acceleration)
		/// <summary>
		/// Gets or sets the size in pixels of this sprite.
		/// </summary>
		public Vector2 Size { get; protected set; }

		/// <summary>
		/// Gets or sets the position of this sprite when it was first loaded into the level.
		/// </summary>
		public Vector2 InitialPosition { get; protected internal set; }

		/// <summary>
		/// Gets or sets the last position of this sprite.
		/// </summary>
		public Vector2 PreviousPosition { get; protected set; }

		/// <summary>
		/// Gets or sets the current position of this sprite.
		/// </summary>
		public Vector2 Position
		{
			get
			{
				return this.position;
			}

			set
			{
				// Round values to nearest integer in case of really small precision errors.
				this.position = new Vector2(value.X.CorrectPrecision(), value.Y.CorrectPrecision());
			}
		}

		/// <summary>
		/// Gets or sets the velocity of this sprite, measured in pixels per second.
		/// </summary>
		public Vector2 Velocity
		{
			get
			{
				return this.velocity;
			}
			set
			{
				this.velocity = value;
			}
		}

		/// <summary>
		/// Gets or sets the acceleration of this sprite, measured in pixels per second per second.
		/// </summary>
		public Vector2 Acceleration { get; set; }
		#endregion

		#region Collision Properties (mode, hitbox, resting tile/slope)
        /// <summary>
        /// Gets or sets the current collision mode of this sprite. Please see the SpriteCollisionMode documentation for
        /// more information.
        /// </summary>
        public SpriteCollisionMode CollisionMode { get; protected internal set; }

        /// <summary>
        /// Gets a rectangle representing this sprite's hitbox.
        /// </summary>
        public BoundingRectangle Hitbox
        {
            get
            {
                return new BoundingRectangle(this.Position, this.Size + this.Position);
            }
        }

        /// <summary>
        /// Gets the tile that this sprite is resting on the top of.
        /// </summary>
        public Tile RestingTile
        {
            get
            {
				throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the sloped tile that this sprite is resting on top of.
        /// </summary>
        public Tile RestingSlope
        {
            get
            {
                throw new NotImplementedException();
            }
        }
		#endregion

		#region Editor Properties (category, label, name, message, hostility, moving, direction)
        /// <summary>
        /// Gets the name of the category that this sprite is categorized within in the level editor.
        /// </summary>
        public abstract string EditorCategory { get; }

        /// <summary>
        /// Gets or sets the name of this sprite used in the level editor.
        /// </summary>
        public string EditorLabel { get; protected set; }

        /// <summary>
        /// Gets or sets an editor property representing an optional name for this sprite, used by event scripting to
        /// reference this object.
        /// </summary>
        [DefaultValue(""), Description("The name of this sprite to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an editor property representing an optional message for this sprite that is displayed if the
        /// player presses Up while near the sprite.
        /// </summary>
        [DefaultValue(""), Description("An optional message that will be displayed if the user presses Up while near the sprite.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite will injure the player if the player hits it.
        /// </summary>
        [DefaultValue(true), Description("Determines if the sprite will injure the player if the player hits it.")]
        public bool IsHostile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite is moving.
        /// </summary>
        [DefaultValue(true), Description("Determines if the sprite is moving.")]
        public bool IsMoving { get; set; }

        /// <summary>
        /// Gets or sets an editor property representing which direction this sprite is facing.
        /// </summary>
        [DefaultValue(SpriteDirection.FacePlayer), Description("The direction that this sprite faces when it loads.")]
        public SpriteDirection Direction { get; set; }
		#endregion

		#region Core Gameobject Methods
        /// <summary>
        /// Initializes this sprite.
        /// </summary>
        /// <param name="owner">The level that owns this sprites.</param>
        public virtual void Initialize(Section owner)
        {
            this.Owner = owner;
            this.Components = new List<SpriteComponent>();
            //// Initialize all the properties
            ////this.IsActive = true;
            ////this.IsHostile = true;
            ////this.IsMoving = true;
            this.Direction = SpriteDirection.FacePlayer;
        }

        /// <summary>
        /// Loads the content for this sprite.
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Updates this sprite.
        /// </summary>
        public virtual void Update()
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public abstract void Draw();
		#endregion

		#region Collision Handlers
        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public virtual void HandleTileCollision(Tile tile, Vector2 intersect)
        {
            this.Components.ForEach(f => f.HandleTileCollision(tile));
        }

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public virtual void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
            this.Components.ForEach(f => f.HandleSpriteCollision(sprite));
        }
		#endregion

        /// <summary>
        /// Performs an action for when this sprite takes damage.
        /// </summary>
        public virtual void Damage()
        {
        }

		#region Serialization Methods
		/// <summary>
        /// Deserializes any objects that custom sprites have written to the level file.
        /// </summary>
        /// <param name="customObjects">An object containing the objects of the custom sprites.</param>
        public abstract void DeserializeCustomObjects(JsonHelper customObjects);
		#endregion
    }
}
