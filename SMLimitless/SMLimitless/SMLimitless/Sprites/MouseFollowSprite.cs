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
                Position = new Vector2(Position.X, Position.Y - distance);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Down))
            {
                Position = new Vector2(Position.X, Position.Y + distance);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                Position = new Vector2(Position.X - 1.0f, Position.Y);
            }
            if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                Position = new Vector2(Position.X + 1.0f, Position.Y);
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
