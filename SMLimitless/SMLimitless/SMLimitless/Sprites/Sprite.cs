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

        private bool isEmbedded;
        public bool IsEmbedded
        {
            get
            {
                return isEmbedded;
            }
            set
            {
                //if (isEmbedded && !value) System.Diagnostics.Debugger.Break();
                isEmbedded = value;
            }
        }

        private bool isOnGround;
        public bool IsOnGround
        {
            get { return this.isOnGround; }
            set
            {
                if (value)
                {
                    Velocity = new Vector2(Velocity.X, 0f);
                    Acceleration = new Vector2(Acceleration.X, 0f);
                }
                isOnGround = value;
            }
        }

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

            if (IsEmbedded)
            {
                // We are embedded if collision checks tell us we need to resolve
                // both left and right or up and down.  We should move left until
                // we're out of being embedded.

                Acceleration = Vector2.Zero;
                Velocity = new Vector2(-25f, 0f);
            }
            else
            {
                if (!IsOnGround && Velocity.Y < 250f)
                {
                    Acceleration = new Vector2(Acceleration.X, Owner.GravityAcceleration);
                }
                else if (Velocity.Y > 250f)
                {
                    Acceleration = new Vector2(Acceleration.X, 0f);
                    Velocity = new Vector2(Velocity.X, 250f);
                }
            }

            Velocity += Acceleration * delta;

            Position += Velocity * delta;
        }

        public abstract void Draw();
        public abstract void HandleTileCollision(Tile tile, Intersection intersect);
        public abstract void HandleSpriteCollision(Sprite sprite, Intersection intersect);
    }
}
