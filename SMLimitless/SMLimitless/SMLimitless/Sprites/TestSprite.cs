using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    public class TestSprite : Sprite
    {
        AnimatedGraphicsObject graphics;

        public override void Initialize(Level owner)
        {
            graphics = new AnimatedGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\gfx\\smb3_goomba.png");
            graphics = (AnimatedGraphicsObject)GraphicsManager.LoadGraphicsObject(absolute);
            Size = new Vector2(16, 16);
            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            graphics.LoadContent();
        }

        public override void Update()
        {
            graphics.Update();
            base.Velocity = new Vector2(30f, Velocity.Y);
            base.Update();
        }

        public override void Draw()
        {
            graphics.Draw(base.Position, Color.White, false);
        }

        public override void HandleSpriteCollision(Sprite sprite, Intersection intersect)
        {
            throw new NotImplementedException();
        }

        public override void HandleTileCollision(Tile tile,Intersection intersect)
        {
            
        }
    }
}
