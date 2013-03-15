using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;

namespace SMLimitless.Sprites
{
    public class MouseFollowSprite : Sprite
    {
        public override void Initialize(Collections.Level owner)
        {
            Size = new Vector2(100, 100);
            base.Initialize(owner);
        }
        public override void Update()
        {
            this.Position = InputManager.MousePosition;
        }

        public override void Draw()
        {
            GameServices.SpriteBatch.DrawRectangle(Hitbox.ToRectangle(), Color.Red);
        }

        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
            throw new NotImplementedException();
        }

        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
            throw new NotImplementedException();
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
        }
    }
}
