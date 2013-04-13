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
    public abstract class Tile : IName, IEditorObject, IPositionable
    {
        public uint ID { get; set; }
        public string EditorLabel { get; set; }
        public Level Owner { get; set; }

        public bool IsActive { get; set; }
        public string State { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public BoundingRectangle Hitbox
        {
            get
            {
                return new BoundingRectangle(Position, Size + Position);
            }
        }

        #region Editor Properties
        [DefaultValue(""), Description("The name of this tile to be used in event scripting.  This field is optional.")]
        public string Name { get; set; }

        [DefaultValue(0), Description("Determines how sprites should collide with this tile.")]
        public TileCollisionType Collision { get; set; }
        #endregion

        public virtual void Initialize(Level owner)
        {
            Owner = owner;
            IsActive = true;
            Name = "";
        }

        public abstract void LoadContent();
        public abstract void Update();
        public abstract void Draw();
        public abstract void HandleCollision(Sprite sprite, Intersection intersect);
    }
}
