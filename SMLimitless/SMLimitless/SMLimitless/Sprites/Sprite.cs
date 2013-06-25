//-----------------------------------------------------------------------
// <copyright file="Sprite.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type for all sprites.
    /// </summary>
    public abstract class Sprite : IName, IEditorObject, IPositionable
    {
        /// <summary>
        /// A backing field for the IsEmbedded property.
        /// </summary>
        private bool isEmbedded;

        /// <summary>
        /// A backing field for the IsOnGround property.
        /// </summary>
        private bool isOnGround;

        /// <summary>
        /// Gets or sets an identification number that identifies all sprites of this kind.
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Gets or sets the name of this sprite used in the level editor.
        /// </summary>
        public string EditorLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the level that owns this sprite.
        /// </summary>
        public Level Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite is actively updating or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a string representing the state of this sprite.
        /// Please see http://smlimitless.wikia.com/wiki/Sprite_State for more information.
        /// </summary>
        public string State { get; protected set; }

        /// <summary>
        /// Gets or sets the current collision mode of this sprite.
        /// Please see the SpriteCollisionMode documentation for more information.
        /// </summary>
        public SpriteCollisionMode CollisionMode { get; protected set; }

        /// <summary>
        /// Gets or sets the last position of this sprite.
        /// </summary>
        public Vector2 PreviousPosition { get; protected set; }

        /// <summary>
        /// Gets or sets the current position of this sprite.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this 
        /// sprite is embedded inside of a tile.
        /// </summary>
        public bool IsEmbedded
        {
            get
            {
                return this.isEmbedded;
            }

            set
            {
                this.isEmbedded = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// sprite is on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get 
            { 
                return this.isOnGround; 
            }

            set
            {
                if (value)
                {
                    this.Velocity = new Vector2(this.Velocity.X, 0f);
                    this.Acceleration = new Vector2(this.Acceleration.X, 0f);
                }

                this.isOnGround = value;
            }
        }

        /// <summary>
        /// Gets or sets the size in pixels of this sprite.
        /// </summary>
        public Vector2 Size { get; protected set; }

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
        /// Gets or sets the acceleration of this sprite,
        /// measured in pixels per second per second.
        /// </summary>
        public Vector2 Acceleration { get; set; }

        /// <summary>
        /// Gets or sets the velocity of this sprite,
        /// measured in pixels per second.
        /// </summary>
        public Vector2 Velocity { get; protected set; }

        /// <summary>
        /// Gets or sets an editor property representing an optional
        /// name for this sprite, used by event scripting to reference this object.
        /// </summary>
        [DefaultValue(""), Description("The name of this sprite to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an editor property representing an optional
        /// message for this sprite that is displayed if the player
        /// presses Up while near the sprite.
        /// </summary>
        [DefaultValue(""), Description("An optional message that will be displayed if the user presses Up while near the sprite.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// sprite will injure the player if the player hits it.
        /// </summary>
        [DefaultValue(true), Description("Determines if the sprite will injure the player if the player hits it.")]
        public bool IsHostile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite is moving.
        /// </summary>
        [DefaultValue(true), Description("Determines if the sprite is moving.")]
        public bool IsMoving { get; set; }

        /// <summary>
        /// Gets or sets an editor property representing which
        /// direction this sprite is facing.
        /// </summary>
        [DefaultValue(SpriteDirection.FacePlayer), Description("The direction that this sprite faces when it loads.")]
        public SpriteDirection Direction { get; set; }

        /// <summary>
        /// Initializes this sprite.
        /// </summary>
        /// <param name="owner">The level that owns this sprites.</param>
        public virtual void Initialize(Level owner)
        {
            this.Owner = owner;

            // Initialize all the properties
            this.IsActive = true;
            this.IsHostile = true;
            this.IsMoving = true;
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
            float delta = GameServices.GameTime.GetElapsedSeconds();

            this.PreviousPosition = this.Position;

            if (this.IsEmbedded)
            {
                // We are embedded if collision checks tell us we need to resolve
                // both left and right or both up and down.  We should move left until
                // we're out of being embedded.
                this.Acceleration = Vector2.Zero;
                this.Velocity = new Vector2(-25f, 0f);
            }
            else
            {
                if (!this.IsOnGround && this.Velocity.Y < 250f)
                {
                    this.Acceleration = new Vector2(this.Acceleration.X, this.Owner.GravityAcceleration);
                }
                else if (this.Velocity.Y > 250f)
                {
                    this.Acceleration = new Vector2(this.Acceleration.X, 0f);
                    this.Velocity = new Vector2(this.Velocity.X, 250f);
                }
            }

            this.Velocity += this.Acceleration * delta;

            this.Position += this.Velocity * delta;
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleTileCollision(Tile tile, Intersection intersect);

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleSpriteCollision(Sprite sprite, Intersection intersect);
    }
}
