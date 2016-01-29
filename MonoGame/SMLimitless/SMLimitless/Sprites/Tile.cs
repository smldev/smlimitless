using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
	public abstract class Tile : IName, IEditorObject, IPositionable2
	{
		private Vector2 position;
		private int solidSides;				// Okay, so this is kind of ugly. This can be either a TileRectSolidSides or a TileTriSolidSides. At least we cast it for public access.

		public CollidableShape TileShape { get; protected set; }

		public Vector2 InitialPosition { get; protected internal set; }

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

		public Vector2 Size { get; set; }

		public bool HasMoved { get; set; }

		public abstract object GetCustomSerializableObjects();

		public BoundingRectangle Bounds
		{
			get
			{
				return new BoundingRectangle(Position, Position + Size);
			}
		}

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
		
		// TODO: Add PropertyGrid attributes
		public TileRectSolidSides RectSolidSides
		{
			get
			{
				if (TileShape != CollidableShape.Rectangle) throw new InvalidOperationException("Tried to get the rectangular solid sides of a triangle.");
				if (solidSides < 0x00 || solidSides > 0x0F) /* too many flags set */ throw new InvalidOperationException($"The solid sides of this tile are not in a valid form. Value: {solidSides}");

				return (TileRectSolidSides)solidSides;
			}
			set
			{
				if (TileShape != CollidableShape.Rectangle) throw new InvalidOperationException("Tried to set the rectangular solid sides of a triangle.");
				if (value < 0x00 || (int)value > 0x0F) throw new ArgumentException($"Invalid value to set solid sides. Value: {value}.", nameof(value));

				solidSides = (int)value;
			}
		}

		public TileTriSolidSides TriSolidSides
		{
			get
			{
				if (TileShape != CollidableShape.RightTriangle) throw new InvalidOperationException("Tried to get the triangular solid sides of a rectangle.");
				if (solidSides < 0x00 || solidSides > 0x07) throw new InvalidOperationException($"The solid sides of this tile are not in a valid state. Value: {solidSides}");

				return (TileTriSolidSides)solidSides;
			}
			set
			{
				if (TileShape != CollidableShape.RightTriangle) throw new InvalidOperationException("Tried to set the triangular solid sides of a rectangle.");
				if (value < 0x00 || (int)value > 0x07) throw new ArgumentOutOfRangeException(nameof(value), $"Invalid value to set solid sides. Value: {value}.");

				solidSides = (int)value;
			}
		}

		public TileAdjacencyFlags AdjacencyFlags { get; internal set; }

		protected internal RtSlopedSides SlopedSides { get; set; }

		public Section Owner { get; set; }

		/// <summary>
		/// Gets the name of the category that this tile is
		/// categorized within in the level editor.
		/// </summary>
		public abstract string EditorCategory { get; }

		/// <summary>
		/// Gets or sets the name of this tile used in the level editor.
		/// </summary>
		public string EditorLabel { get; set; }

		public bool IsActive { get; set; }
		public string InitialState { get; set; }
		public string State { get; set; }

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
			Owner = owner;
			IsActive = true;
			Name = "";
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
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The provided sprite was null."); }

			BoundingRectangle spriteHitbox = (TileShape == CollidableShape.Rectangle) ? sprite.Hitbox : sprite.GetSlopeHitbox(SlopedSides);
			return Hitbox.GetCollisionResolution(spriteHitbox);
		}

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
		/// An abstract method which is called when a sprite intersects this tile.
		/// </summary>
		/// <param name="sprite">The sprite that intersected this tile.</param>
		public virtual void HandleCollision(Sprite sprite)
		{
			HandleCollision(sprite, Hitbox.GetCollisionResolution(sprite.Hitbox));
		}

		/// <summary>
		/// Handles a collision between this tile and a sprite.
		/// </summary>
		/// <param name="sprite">The sprite that has collided with this tile.</param>
		/// <param name="intersect">The depth of the intersection.</param>
		public abstract void HandleCollision(Sprite sprite, Vector2 intersect);

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
			clone.solidSides = solidSides;
			clone.SlopedSides = SlopedSides;
			clone.Name = Name.SafeCopy();

			return clone;
		}

		/// <summary>
		/// Loads key custom objects from the level file.
		/// </summary>
		/// <param name="customObjects">An object containing key custom objects.</param>
		public abstract void DeserializeCustomObjects(JsonHelper customObjects);
	}
}
