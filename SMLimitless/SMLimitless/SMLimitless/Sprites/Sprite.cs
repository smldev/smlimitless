using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Microsoft.Xna.Framework;

using SMLimitless.Interfaces;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites
{
    /// <summary>
    /// The base type for all sprites.
    /// </summary>
    public abstract class Sprite : IName, IEditorObject, IPositionable
    {
        public uint ID { get; set; }
        public string EditorLabel { get; protected set; }

        public bool IsActive { get; set; }
        public string State { get; protected set; }

        public Vector2 PreviousPosition { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 ProjectedPosition { get; protected set; }

        public Vector2 Size { get; protected set; }

        public Vector2 Acceleration { get; protected set; }
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

        public virtual void Initialize()
        {
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
        }

        public abstract void Draw();
    }
}
