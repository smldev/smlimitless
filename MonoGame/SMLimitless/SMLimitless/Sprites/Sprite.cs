//-----------------------------------------------------------------------
// <copyright file="Sprite.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
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
    public abstract class Sprite : IName, IEditorObject, IPositionable, ISerializable
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
        /// Gets the name of the category that this sprite is
        /// categorized within in the level editor.
        /// </summary>
        public abstract string EditorCategory { get; }

        /// <summary>
        /// Gets or sets the name of this sprite used in the level editor.
        /// </summary>
        public string EditorLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the section that owns this sprite.
        /// </summary>
        public Section Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this sprite is actively updating or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the state of this sprite when it was first loaded into the level.
        /// </summary>
        public string InitialState { get; private set; }

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
        /// Gets the position of this sprite when it was first loaded into the level.
        /// </summary>
        public Vector2 InitialPosition { get; protected set; }

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
        /// Gets or sets a value indicating whether this
        /// sprite is sitting on a slope.
        /// </summary>
        public bool IsOnSlope { get; set; }

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
        public Vector2 Velocity { get; set; }

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
        public virtual void Initialize(Section owner)
        {
            this.Owner = owner;

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
            const float MaximumGravitationalVelocity = 350f;
            float delta = GameServices.GameTime.GetElapsedSeconds();

            this.PreviousPosition = this.Position;

            if (this.IsEmbedded)
            {
                // We are embedded if collision checks tell us we need to resolve
                // both left and right or both up and down.  We should move left until
                // we're out of being embedded.
                this.Acceleration = Vector2.Zero;
                ////this.Velocity = new Vector2(-25f, 0f);
            }
            else
            {
                if (!this.IsOnGround && this.Velocity.Y < MaximumGravitationalVelocity)
                {
                    this.Acceleration = new Vector2(this.Acceleration.X, Level.GravityAcceleration);
                }
                else if (this.Velocity.Y > MaximumGravitationalVelocity)
                {
                    this.Acceleration = new Vector2(this.Acceleration.X, 0f);
                    this.Velocity = new Vector2(this.Velocity.X, MaximumGravitationalVelocity);
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
        public abstract void HandleTileCollision(Tile tile, Vector2 intersect);

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public abstract void HandleSpriteCollision(Sprite sprite, Vector2 intersect);

        /// <summary>
        /// Gets any objects that custom sprites wish to be saved to the level file.
        /// </summary>
        /// <returns>An anonymous object containing objects to be saved to the level file.</returns>
        public abstract object GetCustomSerializableObjects();

        /// <summary>
        /// Deserializes any objects that custom sprites have written to the level file.
        /// </summary>
        /// <param name="customObjects">An object containing the objects of the custom sprites.</param>
        public abstract void DeserializeCustomObjects(JsonHelper customObjects);

        /// <summary>
        /// Gets an anonymous object containing objects of the sprite to be saved to the level file.
        /// </summary>
        /// <returns>An anonymous object.</returns>
        public object GetSerializableObjects()
        {
            return new
            {
                typeName = this.GetType().FullName,
                position = this.InitialPosition.Serialize(),
                isActive = this.IsActive,
                state = this.InitialState,
                collision = (int)this.CollisionMode,
                name = this.Name,
                message = this.Message,
                isHostile = this.IsHostile,
                isMoving = this.IsMoving,
                direction = (int)this.Direction,
                customObjects = this.GetCustomSerializableObjects()
            };
        }

        /// <summary>
        /// Returns a JSON string containing key objects of this sprite.
        /// </summary>
        /// <returns>A JSON string.</returns>
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads this sprite using a valid JSON string.
        /// </summary>
        /// <param name="json">A JSON string containing key objects of this sprite.</param>
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);
            this.InitialPosition = obj["position"].ToVector2();
            this.Position = this.InitialPosition;
            this.IsActive = (bool)obj["isActive"];
            this.InitialState = (string)obj["state"];
            this.State = this.InitialState;
            this.CollisionMode = (SpriteCollisionMode)(int)obj["collision"];
            this.Name = (string)obj["name"];
            this.Message = (string)obj["message"];
            this.IsHostile = (bool)obj["isHostile"];
            this.IsMoving = (bool)obj["isMoving"];
            this.Direction = (SpriteDirection)(int)obj["direction"];
            this.DeserializeCustomObjects(new JsonHelper(obj["customObject"]));
        }
    }
}
