using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Microsoft.Xna.Framework;

using SMLimitless.Interfaces;
using SMLimitless.Extensions;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type for all sprites.
    /// </summary>
    public abstract class Sprite : IName, IEditorObject, IPositionable
    {
        public uint ID { get; set; }
        public string EditorLabel { get; protected set; }
        public Level Owner;

        public bool IsActive { get; set; }
        public string State { get; protected set; }

        public Vector2 PreviousPosition { get; protected set; }
        public Vector2 Position { get; set; }
        public Vector2 ProjectedPosition { get; protected set; }

        public Vector2 Size { get; protected set; }
        public BoundingRectangle Hitbox
        {
            get
            {
                return new BoundingRectangle(Position, Size + Position);
            }
        }

        public Vector2 Acceleration { get; set; }
        public Vector2 Velocity { get; protected set; }

        #region Editor Properties
        [DefaultValue(""), Description("The name of this sprite to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        [DefaultValue(""), Description("An optional message that will be displayed if the user presses Up while near the sprite.")]
        public string Message { get; set; }

        [DefaultValue(true), Description("Determines if the sprite will injure the player if the player hits it.")]
        public bool IsHostile { get; set; }

        [DefaultValue(true), Description("Determines if the sprite is moving.")]
        public bool IsMoving { get; set; }

        [DefaultValue(SpriteDirection.FacePlayer), Description("The direction that this sprite faces when it loads.")]
        public SpriteDirection Direction { get; set; }
        #endregion

        public virtual void Initialize(Level owner)
        {
            Owner = owner;
            // Initialize all the properties
            this.IsActive = true;
            this.IsHostile = true;
            this.IsMoving = true;
            this.Direction = SpriteDirection.FacePlayer;
        }

        public abstract void LoadContent();

        public virtual void Update()
        {
            float delta = GameServices.GameTime.GetElapsedSeconds();

            PreviousPosition = Position;
            Position = ProjectedPosition;

            Velocity += Acceleration * delta;

            ProjectedPosition += Velocity * delta;

            if (Velocity.Y >= 500.0f) Velocity = new Vector2(Velocity.X, 500.0f);
        }

        /// <summary>
        /// Checks for collisions with nearby sprites and tiles.
        /// Tiles are first checked, then sprites.
        /// If a collision is detected, the collision handlers will be called.
        /// </summary>
        /// <param name="nearbyTiles">A list of all the tiles in the cell(s) where this sprite is.</param>
        /// <param name="nearbySprites">A list of all the sprites in the cell(s) where this sprite is.</param>
        public void CheckCollisions(List<Tile> nearbyTiles, List<Sprite> nearbySprites)
        {
            CheckTileCollisions(nearbyTiles);
            CheckSpriteCollisions(nearbySprites);
        }

        protected virtual void CheckTileCollisions(List<Tile> nearbyTiles)
        {
            foreach (Tile tile in nearbyTiles)
            {
                Vector2 intersect = Hitbox.GetIntersectionDepth(tile.Hitbox);
                if (intersect != Vector2.Zero)
                {
                    tile.HandleCollision(this, intersect);
                    this.HandleTileCollision(tile, intersect);
                }
            }
        }

        protected virtual void CheckSpriteCollisions(List<Sprite> nearbySprites)
        {
            foreach (Sprite sprite in nearbySprites)
            {
                Vector2 intersect = Hitbox.GetIntersectionDepth(sprite.Hitbox);
                if (intersect != Vector2.Zero)
                {
                    sprite.HandleSpriteCollision(this, intersect);
                    this.HandleSpriteCollision(sprite, intersect);
                }
            }
        }

        public abstract void Draw();
        public abstract void HandleTileCollision(Tile tile, Vector2 intersect);
        public abstract void HandleSpriteCollision(Sprite sprite, Vector2 intersect);
    }
}
