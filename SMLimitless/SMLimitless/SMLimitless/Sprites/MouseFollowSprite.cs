using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites
{
    public class MouseFollowSprite : Sprite
    {
        public override void Initialize(Collections.Level owner)
        {
            Size = new Vector2(16f, 16f);
            base.Initialize(owner);
        }
        public override void Update()
        {
            float distance = 2f;
            if (InputManager.IsCurrentKeyPress(Keys.Up))
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y - distance);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Down))
            {
                this.Position = new Vector2(this.Position.X, this.Position.Y + distance);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                this.Position = new Vector2(this.Position.X - 1.0f, this.Position.Y);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                this.Position = new Vector2(this.Position.X + 1.0f, this.Position.Y);
            }
        }

        public override void Draw()
        {
            GameServices.SpriteBatch.DrawRectangle(Hitbox.ToRectangle(), Color.Red);
        }

        public override void HandleSpriteCollision(Sprite sprite, Intersection intersect)
        {
            throw new NotImplementedException();
        }

        public override void HandleTileCollision(Tile tile, Intersection intersect)
        {
            
        }

        public override void LoadContent()
        {
        
        }
    }
}
