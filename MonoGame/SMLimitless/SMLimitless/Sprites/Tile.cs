using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
	/// <summary>
	///   A unit of terrain in <see cref="Section" />. Sprites can collide with,
	///   slide down, and walk across these tiles.
	/// </summary>
	public abstract class Tile : IName, IEditorObject, IPositionable2
	{
		private Vector2 position;

		/// <summary>
		///   Gets a value indicating if there are slopes on either side of this tile.
		/// </summary>
		public TileAdjacencyFlags AdjacencyFlags { get; internal set; }

		/// <summary>
		///   Gets a rectangle that totally contains this tile.
		/// </summary>
		public BoundingRectangle Bounds
		{
			get
			{
				return new BoundingRectangle(Position, Position + Size);
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether a collision between this
		///   tile and a sprite should break the debugger if the collision
		///   debugger is enabled.
		/// </summary>
		public bool BreakOnCollision { get; set; }

		/// <summary>
		///   Gets the name of the category that this tile is categorized within
		///   in the level editor.
		/// </summary>
		public abstract string EditorCategory { get; }

		/// <summary>
		///   Gets or sets the name of this tile used in the level editor.
		/// </summary>
		public string EditorLabel { get; set; }

		/// <summary>
		///   Gets or sets the name of the graphics resource used by this tile.
		/// </summary>
		public string GraphicsResourceName { get; protected internal set; }

		/// <summary>
		///   Gets or sets a value indicating whether this tile has moved during
		///   this frame.
		/// </summary>
		public bool HasMoved { get; set; }

		/// <summary>
		///   Gets the collidable hitbox for this tile.
		/// </summary>
		public ICollidableShape Hitbox
		{
			get
			{
				if (TileShape == CollidableShape.Rectangle)
				{
					return Bounds;
				}
				else if (TileShape == CollidableShape.RightTriangle)
				{
					return new RightTriangle(Bounds, SlopedSides);
				}
				else
				{
					throw new InvalidOperationException($"The shape of this tile was set to an invalid value. Value: {TileShape}.");
				}
			}
		}

		/// <summary>
		///   Gets the initial position of this tile in the section in pixels.
		/// </summary>
		public Vector2 InitialPosition { get; protected internal set; }

		/// <summary>
		///   Gets or sets a string indicating the initial state of this tile.
		/// </summary>
		public string InitialState { get; set; }

		/// <summary>
		///   Gets or sets a flag indicating whether this tile is active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		///   Gets or sets the name of this tile to be used in event scripting.
		///   This field is optional.
		/// </summary>
		[DefaultValue(""), Description("The name of this tile to be used in event scripting.  This field is optional.")]
		public string Name { get; set; }

		/// <summary>
		///   Gets or sets the <see cref="Section" /> that owns this tile.
		/// </summary>
		public Section Owner { get; set; }

		/// <summary>
		///   Gets or sets the position of this tile in the section in pixels.
		/// </summary>
		/// <remarks>
		///   When this property is set, the HasMoved flag is set and the new
		///   position is corrected if it is very close to a pixel boundary.
		/// </remarks>
		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				HasMoved = true;
				position = new Vector2(value.X.CorrectPrecision(), value.Y.CorrectPrecision());
			}
		}

		/// <summary>
		///   Gets or sets a value indicating which sides of a rectangular tile
		///   are solid.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   Thrown in the getter if the internal SolidSides property has been
		///   set to an invalid value.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///   Thrown in the setter if this tile is not a rectangle.
		/// </exception>
		/// <exception cref="ArgumentException">
		///   Thrown in the setter if the provided value was invalid.
		/// </exception>
		public TileRectSolidSides RectSolidSides
		{
			get
			{
				if (TileShape != CollidableShape.Rectangle) return TileRectSolidSides.NotARectangle;
				if ((SolidSides < 0x00 || SolidSides > 0x0F) && SolidSides != 0xFF) /* too many flags set */ throw new InvalidOperationException($"The solid sides of this tile are not in a valid form. Value: {SolidSides}");

				return (TileRectSolidSides)SolidSides;
			}
			set
			{
				if (TileShape != CollidableShape.Rectangle) throw new InvalidOperationException("Tried to set the rectangular solid sides of a triangle.");
				if (value < 0x00 || (int)value > 0x0F) throw new ArgumentException($"Invalid value to set solid sides. Value: {value}.", nameof(value));

				SolidSides = (int)value;
			}
		}

		/// <summary>
		///   Gets or sets the size of this tile in pixels.
		/// </summary>
		public Vector2 Size { get; set; }

		/// <summary>
		///   Gets or sets a string indicating the current state of this tile.
		/// </summary>
		public string State { get; set; }

		/// <summary>
		///   Gets the surface friction of the tile - how much sprites not moving
		///   on their own decelerate in each frame. Measured in pixels per
		///   second squared.
		/// </summary>
		public abstract float SurfaceFriction { get; }

		/// <summary>
		///   Gets a value indicating the shape of this tile.
		/// </summary>
		public CollidableShape TileShape { get; protected set; }

		// TODO: Add PropertyGrid attributes
		/// <summary>
		///   Gets or sets a value indicating which sides of a triangular tile
		///   are solid.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		///   Thrown in the getter if the value of the internal SolidSides
		///   property is invalid.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		///   Thrown in the setter if this tile is not a triangle.
		/// </exception>
		/// <exception cref="ArgumentException">
		///   Thrown in the setter if the provided value is invalid.
		/// </exception>
		public TileTriSolidSides TriSolidSides
		{
			get
			{
				if (TileShape != CollidableShape.RightTriangle) return TileTriSolidSides.NotATriangle;
				if ((SolidSides < 0x00 || SolidSides > 0x07) && SolidSides != 0xFF) throw new InvalidOperationException($"The solid sides of this tile are not in a valid state. Value: {SolidSides}");

				return (TileTriSolidSides)SolidSides;
			}
			set
			{
				if (TileShape != CollidableShape.RightTriangle) throw new InvalidOperationException("Tried to set the triangular solid sides of a rectangle.");
				if (value < 0x00 || (int)value > 0x07) throw new ArgumentOutOfRangeException(nameof(value), $"Invalid value to set solid sides. Value: {value}.");

				SolidSides = (int)value;
			}
		}

		internal int SolidSides { get; set; }           // Okay, so this is kind of ugly. This can be either a TileRectSolidSides or a TileTriSolidSides. At least we cast it for public access.

														/// <summary>
														///   Gets or sets a
														///   value indicating
														///   which two sides of
														///   this tile have been
														///   replaced by the
														///   sloped side.
														/// </summary>
		protected internal RtSlopedSides SlopedSides { get; set; }

		/// <summary>
		///   Creates a deep copy of this tile.
		/// </summary>
		/// <returns>
		///   A new tile instance with most of the same values as this tile.
		/// </returns>
		public Tile Clone()
		{
			Tile clone = AssemblyManager.GetTileByFullName(GetType().FullName);

			clone.EditorLabel = EditorLabel.SafeCopy();
			clone.Owner = Owner;
			clone.IsActive = IsActive;
			clone.InitialState = InitialState.SafeCopy();
			clone.State = State.SafeCopy();
			clone.Position = Position;
			clone.Size = Size;
			clone.SolidSides = SolidSides;
			clone.SlopedSides = SlopedSides;
			clone.Name = Name.SafeCopy();
			clone.GraphicsResourceName = GraphicsResourceName.SafeCopy();

			return clone;
		}

		/// <summary>
		///   Loads key custom objects from the level file.
		/// </summary>
		/// <param name="customObjects">An object containing key custom objects.</param>
		public abstract void DeserializeCustomObjects(JsonHelper customObjects);

		/// <summary>
		///   Draws this tile.
		/// </summary>
		public abstract void Draw();

		/// <summary>
		///   Gets an anonymous object containing other objects that need to be
		///   saved in the level file.
		/// </summary>
		/// <returns>
		///   An anonymous object containing objects to be saved in the level file.
		/// </returns>
		public abstract object GetCustomSerializableObjects();

		/// <summary>
		///   An abstract method which is called when a sprite intersects this tile.
		/// </summary>
		/// <param name="sprite">The sprite that intersected this tile.</param>
		public virtual void HandleCollision(Sprite sprite)
		{
			HandleCollision(sprite, Hitbox.GetCollisionResolution(sprite.Hitbox));
		}

		/// <summary>
		///   Handles a collision between this tile and a sprite.
		/// </summary>
		/// <param name="sprite">The sprite that has collided with this tile.</param>
		/// <param name="intersect">The depth of the intersection.</param>
		public abstract void HandleCollision(Sprite sprite, Vector2 intersect);

		/// <summary>
		///   Initializes this tile.
		/// </summary>
		/// <param name="owner">The Section that owns this tile.</param>
		public virtual void Initialize(Section owner)
		{
			Owner = owner;
			IsActive = true;
			Name = "";
		}

		/// <summary>
		///   Determines if a given sprite intersects this tile.
		/// </summary>
		/// <param name="sprite">The sprite to check.</param>
		/// <returns>
		///   True if the sprite intersects this tile, false if otherwise.
		/// </returns>
		/// <remarks>
		///   This method accounts for the different tile collision types.
		/// </remarks>
		public virtual bool Intersects(Sprite sprite)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Loads the content for this tile.
		/// </summary>
		public abstract void LoadContent();

		/// <summary>
		///   Called when the user drops a sprite onto this tile using the level editor.
		/// </summary>
		/// <param name="sprite">The sprite dropped on the tile.</param>
		/// <returns>
		///   True if the tile accepted the sprite, false if it did not.
		/// </returns>
		public virtual bool OnEditorDrop(Sprite sprite)
		{
			return false;
		}

		/// <summary>
		///   Updates this tile.
		/// </summary>
		public abstract void Update();

		internal bool CollisionOnSolidSide(Vector2 resolutionDistance)
		{
			if (TileShape == CollidableShape.Rectangle)
			{
				if (resolutionDistance.X != 0f)
				{
					if (resolutionDistance.X < 0f) { return (RectSolidSides & TileRectSolidSides.Left) == TileRectSolidSides.Left; }
					else if (resolutionDistance.X > 0f) { return (RectSolidSides & TileRectSolidSides.Right) == TileRectSolidSides.Right; }
				}
				else if (resolutionDistance.Y != 0f)
				{
					if (resolutionDistance.Y < 0f) { return (RectSolidSides & TileRectSolidSides.Top) == TileRectSolidSides.Top; }
					else if (resolutionDistance.Y > 0f) { return (RectSolidSides & TileRectSolidSides.Bottom) == TileRectSolidSides.Bottom; }
				}
			}
			else if (TileShape == CollidableShape.RightTriangle)
			{
				if (resolutionDistance.X != 0f)
				{
					return (TriSolidSides & TileTriSolidSides.VerticalLeg) == TileTriSolidSides.VerticalLeg;
				}
				else if (resolutionDistance.Y != 0f)
				{
					if (resolutionDistance.Y < 0f)
					{
						if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
						{ return (TriSolidSides & TileTriSolidSides.Slope) == TileTriSolidSides.Slope; }
						else if (SlopedSides == RtSlopedSides.BottomLeft || SlopedSides == RtSlopedSides.BottomRight)
						{ return (TriSolidSides & TileTriSolidSides.HorizontalLeg) == TileTriSolidSides.HorizontalLeg; }
					}
					else if (resolutionDistance.Y > 0f)
					{
						if (SlopedSides == RtSlopedSides.BottomLeft || SlopedSides == RtSlopedSides.BottomRight)
						{ return (TriSolidSides & TileTriSolidSides.HorizontalLeg) == TileTriSolidSides.HorizontalLeg; }
						else if (SlopedSides == RtSlopedSides.TopLeft || SlopedSides == RtSlopedSides.TopRight)
						{ return (TriSolidSides & TileTriSolidSides.Slope) == TileTriSolidSides.Slope; }
					}
				}
			}

			return false;
		}

		/// <summary>
		///   Returns the distance to resolve a given colliding sprite by so that
		///   it will be moved out of this tile.
		/// </summary>
		/// <param name="sprite">The sprite to resolve.</param>
		/// <returns>A resolution containing the distance.</returns>
		/// <remarks>
		///   This method accounts for the different tile collision types.
		/// </remarks>
		internal virtual Vector2 GetCollisionResolution(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The provided sprite was null."); }

			BoundingRectangle spriteHitbox = (TileShape == CollidableShape.Rectangle) ? sprite.Hitbox : sprite.GetSlopeHitbox(SlopedSides);
			return Hitbox.GetCollisionResolution(spriteHitbox);
		}
	}
}
