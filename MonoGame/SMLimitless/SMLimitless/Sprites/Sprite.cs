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
    public abstract class Sprite : IName, IEditorObject, IPositionable, ISerializable
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
				Vector2 checkPoint = new Vector2(this.Hitbox.BottomCenter.X, this.Hitbox.BottomCenter.Y + 1f);
				Tile tileBeneath = this.Owner.GetTileAtPositionByBounds(checkPoint, true);
				if (tileBeneath == null)
				{
					return false;
				}
				else if (tileBeneath is SlopedTile)
				{
					// There's probably a better way to do this.
					return this.Owner.GetTileAtPosition(checkPoint, true) != null;
				}
				else
				{
					return tileBeneath.Collision == TileCollisionType.Solid || tileBeneath.Collision == TileCollisionType.TopSolid;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this sprite is sitting on a slope.
		/// </summary>
		public bool IsOnSlope
		{
			get
			{
				return this.RestingSlope != null && this.Velocity.Y >= 0;
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
				/*
				 * Where you left off (oh, god):
				 * Sprites do not follow terrain quite well. When moving downwards from a square tile to a slope,
				 * the sprite does not follow the terrain. The IsOnGround flag returns false, so the normal handler
				 * is running and resolving the sprite normally, preventing it from falling onto the slope properly.
				 * Probably the best course of action is to a) turn IsOnGround into a standard flag, b) disable gravity
				 * on upwards collisions until the sprite jumps or the resting tile is removed, and c) create a flag for
				 * sprites to follow terrain (IsFollowingTerrain) or something, where the sprite's Y position is always
				 * determined by the top point of the tile beneath it, and the vertical collision handler doesn't run.
				 * We might even need raycasting.
				 */
                Vector2 checkPoint = new Vector2((int)this.Hitbox.Center.X, (int)(this.Hitbox.Bottom + 1f));
				return this.Owner.GetTileAtPositionByBounds(checkPoint, adjacentPointsAreWithin: true);
            }
        }

        /// <summary>
        /// Gets the sloped tile that this sprite is resting on top of.
        /// </summary>
        public SlopedTile RestingSlope
        {
            get
            {
                if (this.RestingTile is SlopedTile)
                {
                    return (SlopedTile)this.RestingTile;
                }
                return null;
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
            const float MaximumGravitationalVelocity = 350f;
            float delta = GameServices.GameTime.GetElapsedSeconds();

            this.Components.ForEach(c => c.Update());

            this.PreviousPosition = this.Position;

            if (this.IsEmbedded)
            {
                // We are embedded if collision checks tell us we need to resolve both left and right or both up and
                // down. We should move left until we're out of being embedded.
                this.Acceleration = Vector2.Zero;
                this.Velocity = new Vector2(-25f, 0f);
            }
			else if (this.IsOnGround)
			{
				if (this.Velocity.Y >= 0f)
				{
					this.Velocity = new Vector2(this.Velocity.X, 0);
					this.Acceleration = new Vector2(this.Acceleration.X, 0f);
				}
				this.Acceleration = new Vector2(this.Acceleration.X, this.Acceleration.Y - (this.Acceleration.Y * 0.01f));
			}
            else
            {
                if (this.Velocity.Y < MaximumGravitationalVelocity)
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
				state = (int)this.InitialState,
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
        /// Gets any objects that custom sprites wish to be saved to the level file.
        /// </summary>
        /// <returns>An anonymous object containing objects to be saved to the level file.</returns>
        public abstract object GetCustomSerializableObjects();

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
            this.InitialState = (SpriteState)(int)obj["state"];
            this.State = this.InitialState;
            this.CollisionMode = (SpriteCollisionMode)(int)obj["collision"];
            this.Name = (string)obj["name"];
            this.Message = (string)obj["message"];
            this.IsHostile = (bool)obj["isHostile"];
            this.IsMoving = (bool)obj["isMoving"];
            this.Direction = (SpriteDirection)(int)obj["direction"];
            this.DeserializeCustomObjects(new JsonHelper(obj["customObject"]));
        }

		/// <summary>
        /// Deserializes any objects that custom sprites have written to the level file.
        /// </summary>
        /// <param name="customObjects">An object containing the objects of the custom sprites.</param>
        public abstract void DeserializeCustomObjects(JsonHelper customObjects);
		#endregion
    }
}
