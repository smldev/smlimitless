//-----------------------------------------------------------------------
// <copyright file="Sprite.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
	[DefaultProperty("Name")]
	public abstract class Sprite : IName, IEditorObject, IPositionable, IPositionable2
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
		/// A backing field for the Hitbox property.
		/// </summary>
		private BoundingRectangle hitbox;

		private List<string> attributes = new List<string>();

		/// <summary>
		/// A physics setting representing the maximum downward velocity a sprite can acquire through falling. Measured in pixels per second.
		/// </summary>
		public static PhysicsSetting<float> MaximumGravitationalVelocity = new PhysicsSetting<float>("Max Gravitational Velocity", 0f, 1000f, 350f, PhysicsSettingType.FloatingPoint);

		/// <summary>
		/// A physics setting representing the rate that any upward acceleration is reduced to zero. Measured in pixels per second cubed.
		/// </summary>
		public static PhysicsSetting<float> UpwardAccelerationDecay = new PhysicsSetting<float>("Upward Acceleration Decay", 0.01f, 10f, 0.01f, PhysicsSettingType.FloatingPoint);

		/// <summary>
		/// Gets or sets an identification number that identifies all sprites of this kind.
		/// </summary>
		[Browsable(false)]
		public uint ID { get; set; }

		#region State Properties (components, state, owner)
		/// <summary>
		/// Gets a list of all the components used by this sprite instance.
		/// </summary>
		[Browsable(false)]
		public List<SpriteComponent> Components { get; private set; } = new List<SpriteComponent>();

		/// <summary>
		/// Gets the state of this sprite when it was first loaded into the level.
		/// </summary>
		[Browsable(false)]
		public SpriteState InitialState { get; internal set; }

		/// <summary>
		/// Gets or sets a string representing the state of this sprite. Please see
		/// http://smlimitless.wikia.com/wiki/Sprite_State for more information.
		/// </summary>
		[Browsable(false)]
		public SpriteState State { get; protected internal set; }

		/// <summary>
		/// Gets or sets the section that owns this sprite.
		/// </summary>
		[Browsable(false)]
		public Section Owner { get; set; }

		public IReadOnlyList<string> Attributes => attributes.AsReadOnly();
		#endregion

		#region Flags (active, embedded, ground, slope, remove)
        /// <summary>
        /// Gets or sets a value indicating whether this sprite is actively updating or not.
        /// </summary>
        [Obsolete]
		public bool IsActive { get; set; }

		[Browsable(false)]
		public virtual SpriteActiveState ActiveState { get; set; } = SpriteActiveState.Active;

		/// <summary>
		/// Gets or sets a value indicating whether this sprite is embedded inside of a tile.
		/// </summary>
		[Browsable(false)]
		public bool IsEmbedded { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this sprite is resting on the ground.
		/// </summary>
		[Browsable(false)]
		public bool IsOnGround { get; internal set; }

		[Browsable(false)]
		public Tile TileBeneathSprite
		{
			get
			{
				Vector2 checkPoint = Hitbox.BottomCenter;
				checkPoint.Y += 1f;
				Tile result = Owner.GetTileAtPosition(checkPoint);

				if (result != null) { return result; }

				checkPoint = Hitbox.BottomLeft;
				checkPoint.Y += 1f;
				result = Owner.GetTileAtPosition(checkPoint);

				if (result != null) { return result; }

				checkPoint = Hitbox.BottomRight;
				checkPoint.Y += 1f;
				result = Owner.GetTileAtPosition(checkPoint);

				return result;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this sprite should be removed from its owner section on the next frame.
		/// </summary>
		[Browsable(false)]
		public bool RemoveOnNextFrame { get; set; }
		#endregion

		#region Physics Properties (size, position, velocity, acceleration)
		/// <summary>
		/// Gets or sets the size in pixels of this sprite.
		/// </summary>
		[Browsable(false)]
		public Vector2 Size { get; protected set; }

		/// <summary>
		/// Gets or sets the position of this sprite when it was first loaded into the level, measured in pixels.
		/// </summary>
		[Browsable(false)]
		public Vector2 InitialPosition { get; protected internal set; }

		/// <summary>
		/// Gets or sets the last position of this sprite, measured in pixels.
		/// </summary>
		[Browsable(false)]
		public Vector2 PreviousPosition { get; protected set; }

		[Browsable(false)]
		public Vector2 PreviousVelocity { get; protected set; }

		/// <summary>
		/// Gets or sets the current position of this sprite, measured in pixels.
		/// </summary>
		[Description("The position of this sprite in the level.")]
		[Category("Physics")]
		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				HasMoved = true;

				// Round values to nearest integer in case of really small precision errors.
				position = new Vector2(value.X.CorrectPrecision(), value.Y.CorrectPrecision());

				// Rebuild the hitbox
				hitbox = new BoundingRectangle(position, position + Size);
			}
		}

		/// <summary>
		/// Gets or sets the velocity of this sprite, measured in pixels per second.
		/// </summary>
		[Browsable(false)]
		public Vector2 Velocity 
		{
			get
			{
				return velocity;
			}
			set
			{
				PreviousVelocity = velocity;
				velocity = value;
			}
		}

		/// <summary>
		/// Gets or sets the acceleration of this sprite, measured in pixels per second per second.
		/// </summary>
		[Browsable(false)]
		public Vector2 Acceleration { get; set; }

		/// <summary>
		/// Gets an anonymous object containing objects that need to be saved to the level file.
		/// </summary>
		/// <returns></returns>
		public abstract object GetCustomSerializableObjects();

		/// <summary>
		/// Gets or sets a value indicating whether the sprite has moved during this frame.
		/// </summary>
		[Browsable(false)]
		public bool HasMoved
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether a collision between this sprite and a tile should break the debugger if collision debugging is enabled.
		/// </summary>
		[Browsable(false)]
		public bool BreakOnCollision { get; set; }
		#endregion

		#region Collision Properties (mode, hitbox, resting tile/slope)
		/// <summary>
		/// Gets or sets the current collision mode of this sprite. Please see the SpriteCollisionMode documentation for
		/// more information.
		/// </summary>
		[Browsable(false)]
		public SpriteCollisionMode TileCollisionMode { get; protected internal set; }

		/// <summary>
		/// Gets or sets the current collision mode of this sprite for other sprites.
		/// </summary>
		[Browsable(false)]
		public SpriteCollisionMode SpriteCollisionMode { get; protected internal set; }

		/// <summary>
		/// Gets a rectangle representing this sprite's hitbox.
		/// </summary>
		[Browsable(false)]
		public BoundingRectangle Hitbox
        {
            get
            {
				return hitbox;
            }
        }

		internal HashSet<Sprite> SpritesCollidedWithThisFrame { get; set; } = new HashSet<Sprite>();

		internal SpriteCollisionData CollisionData { get; private set; }
		#endregion

		#region Editor Properties (category, label, name, message, hostility, moving, direction)
		/// <summary>
		/// Gets the name of the category that this sprite is categorized within in the level editor.
		/// </summary>
		[Browsable(false)]
		public abstract string EditorCategory { get; }

		/// <summary>
		/// Gets or sets the name of this sprite used in the level editor.
		/// </summary>
		[Browsable(false)]
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
		/// Gets a value indicating whether this sprite is a player sprite.
		/// </summary>
		[Browsable(false)]
		public virtual bool IsPlayer { get { return false; }}

        /// <summary>
        /// Gets or sets an editor property representing which direction this sprite is facing.
        /// </summary>
        [DefaultValue(SpriteDirection.FacePlayer), Description("The direction that this sprite faces when it loads.")]
        public SpriteDirection Direction { get; set; }
		#endregion

		/// <summary>
		/// Resolves a sprite direction into a horizontal direction.
		/// </summary>
		/// <param name="sprite">The sprite for which the direction needs to be resolved.</param>
		/// <param name="direction">The direction that needs to be resolved.</param>
		/// <param name="section">The section which the <paramref name="sprite"/> is in.</param>
		/// <returns>Left if <paramref name="direction"/> is Left, or Right if <paramref name="direction"/> is Right.
		/// If <paramref name="direction"/> is FacePlayer, then the returned direction will be the one facing the first player
		/// in the section, or Left if there are no players.</returns>
		public static SpriteDirection ResolveDirection(Sprite sprite, SpriteDirection direction, Section section)
		{
			if (section == null) { throw new ArgumentNullException(nameof(section), "Cannot resolve direction; the provided section was null."); }
			if (direction != SpriteDirection.FacePlayer) { return (direction == SpriteDirection.Left) ? SpriteDirection.Left : SpriteDirection.Right; }
			if (!section.Players.Any()) { return SpriteDirection.Left; }

			Sprite firstPlayer = section.Players.First();
			if (firstPlayer.Position.X < sprite.Position.X)
			{
				return SpriteDirection.Left;
			}
			else
			{
				return SpriteDirection.Right;
			}
		}

		/// <summary>
		/// Gets the component of a certain type.
		/// </summary>
		/// <typeparam name="T">The type of component to return.</typeparam>
		/// <param name="componentType">The type of component to return</param>
		/// <returns>The component of the given type, or null if there is no such component.</returns>
		public T GetComponent<T>() where T : SpriteComponent
		{
			SpriteComponent result = Components.FirstOrDefault(c => c.GetType() == typeof(T));
			if (result != null)
			{
				return (T)result;
			}
			return null;
		}

		public virtual void Activate() { }

		public virtual void Deactivate() { }

		public bool HasAttribute(string attribute)
		{
			return attributes.Contains(attribute);
		}

		public void AddAttribute(string attribute)
		{
			if (!attributes.Contains(attribute))
			{
				attributes.Add(attribute);
			}
		}

		public bool RemoveAttribute(string attribute)
		{
			return attributes.Remove(attribute);
		}

		#region Core Gameobject Methods
        /// <summary>
        /// Initializes this sprite.
        /// </summary>
        /// <param name="owner">The level that owns this sprites.</param>
        public virtual void Initialize(Section owner)
        {
			Owner = owner;
			CollisionData = new SpriteCollisionData(this);
			//// Initialize all the properties
			////this.IsActive = true;
			////this.IsHostile = true;
			////this.IsMoving = true;
			Direction = SpriteDirection.FacePlayer;
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

			Components.ForEach(c => c.Update());

			PreviousPosition = Position;

			if (IsEmbedded)
			{
				Acceleration = Vector2.Zero;
				Velocity = new Vector2(-25f, 0f);
			}
			else
			{
				if (Velocity.Y < MaximumGravitationalVelocity.Value)
				{
					Acceleration = new Vector2(Acceleration.X, Level.GravityAcceleration.Value);
				}
				else if (Velocity.Y > MaximumGravitationalVelocity.Value)
				{
					Acceleration = new Vector2(Acceleration.X, 0f);
					Velocity = new Vector2(Velocity.X, MaximumGravitationalVelocity.Value);
				}
			}

			Velocity += Acceleration * delta;
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public abstract void Draw();

		/// <summary>
		/// Draws a part of this sprite specified by a cropping.
		/// </summary>
		/// <param name="cropping">The amount of the sprite to draw.</param>
		public abstract void Draw(Rectangle cropping);
		#endregion

		#region Collision Handlers
		/// <summary>
		/// Gets the hitbox of this sprite for collision with a slope.
		/// </summary>
		/// <param name="slopedSides">The sloped sides of the slope.</param>
		/// <returns>The left half of the sprite's hitbox for TopLeft and BottomLeft slopes or the right half of the hitbox for TopRight and BottomRight slopes.</returns>
		/// <exception cref="ArgumentException">Thrown if <paramref name="slopedSides"/> has a value of Default.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="slopedSides"/> does not have a valid value.</exception>
        public BoundingRectangle GetSlopeHitbox(RtSlopedSides slopedSides)
		{
			Vector2 hitboxSize = new Vector2(Size.X / 2f, Size.Y);

			switch (slopedSides)
			{
				case RtSlopedSides.Default:
					throw new ArgumentException("A sloped sides value of Default is not valid.", nameof(slopedSides));
				case RtSlopedSides.TopLeft:
				case RtSlopedSides.BottomLeft:
					return new BoundingRectangle(Position.X, Position.Y, hitboxSize.X, hitboxSize.Y);
				case RtSlopedSides.TopRight:
				case RtSlopedSides.BottomRight:
					Vector2 hitboxPosition = new Vector2(Position.X + (Size.X / 2f), Position.Y);
					return new BoundingRectangle(hitboxPosition.X, hitboxPosition.Y, hitboxSize.X, hitboxSize.Y);
				default:
					throw new ArgumentOutOfRangeException(nameof(slopedSides), $"Invalid sloped sides value {slopedSides}. The expected range is from 0 to 4.");
			}
		}

		/// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="resolutionDistance">The depth of the intersection.</param>
        public virtual void HandleTileCollision(Tile tile, Vector2 resolutionDistance)
        {
			Components.ForEach(f => f.HandleTileCollision(tile, resolutionDistance));
        }

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="resolutionDistance">The depth of the intersection.</param>
        public virtual void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
        {
			Components.ForEach(f => f.HandleSpriteCollision(sprite, resolutionDistance));
        }

		/// <summary>
		/// Determines if a sprite is stomping another sprite.
		/// </summary>
		/// <param name="a">The sprite that may be stomping.</param>
		/// <param name="b">The sprite that may be being stomped.</param>
		/// <returns>True if <paramref name="a"/> is stomping <paramref name="b"/>.</returns>
		public static bool IsStomping(Sprite a, Sprite b)
		{
			BoundingRectangle aHitbox = a.Hitbox;
			BoundingRectangle bHitbox = b.Hitbox;

			// Condition 1: a's Hitbox must be between b.Left and b.Right
			if (aHitbox.Right < bHitbox.Left || aHitbox.Left > bHitbox.Right)
			{
				return false;
			}

			// Condition 2: a's Bottom must be below or at b's Top
			if (aHitbox.Bottom < bHitbox.Top)
			{
				return false;
			}

			// Condition 3: a's Center.Y must be above b's Center.Y
			if (aHitbox.Center.Y >= bHitbox.Center.Y)
			{
				return false;
			}

			// Condition 4: a's PreviousVelocity must be downward
			if (a.PreviousVelocity.Y <= 0f)
			{
				return false;
			}

			return true;
		}
		#endregion

        /// <summary>
        /// Performs an action for when this sprite takes damage.
        /// </summary>
        public virtual void Damage()
        {
        }

		/// <summary>
		/// Accepts a sprite that was dropped on this sprite by the level editor.
		/// when the user left-clicked on it with a sprite selected.
		/// </summary>
		/// <param name="sprite">The sprite dropped on this sprite.</param>
		/// <returns>True if this sprite supports dropping, false if otherwise.</returns>
		public virtual bool OnEditorDrop(Sprite sprite)
		{
			return false;
		}

		public virtual string GetDebugInfo()
		{
			return "";
		}

		public Sprite Clone()
		{
			Sprite clone = AssemblyManager.GetSpriteByFullName(GetType().FullName);

			clone.EditorLabel = EditorLabel.SafeCopy();
			clone.Owner = Owner;
			clone.InitialState = State;
			clone.State = clone.InitialState;
			// Clones should always start out active. If they're not supposed to be active, the Section will disable them on the next frame.
			clone.ActiveState = (ActiveState == SpriteActiveState.AlwaysActive) ? SpriteActiveState.AlwaysActive : SpriteActiveState.Active;
			clone.Size = Size;
			clone.InitialPosition = clone.Position = Vector2.Zero;
			clone.Velocity = Vector2.Zero;
			clone.Acceleration = Vector2.Zero;
			clone.HasMoved = true;
			clone.TileCollisionMode = TileCollisionMode;
			clone.SpriteCollisionMode = SpriteCollisionMode;
			clone.Name = Name.SafeCopy();
			clone.Message = Message.SafeCopy();
			clone.IsHostile = IsHostile;
			clone.IsMoving = IsMoving;
			clone.Direction = Direction;

			clone.DeserializeCustomObjects(new JsonHelper(JObject.FromObject(GetCustomSerializableObjects())));

			return clone;
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
