using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    public class TestSprite : Sprite
    {
        AnimatedGraphicsObject graphics;

        public override void Initialize(Level owner)
        {
            graphics = new AnimatedGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphics.LoadFromMetadata(@"anim-spritesheet>""" + absolute + @""",16,16,15,2,3");
            this.Size = new Vector2(16, 16);
            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            graphics.LoadContent();
        }

        public override void Update()
        {
            graphics.Update();
            base.Velocity = new Vector2(30f, 0f);
            base.Update();
        }

        public override void Draw()
        {
            graphics.Draw(base.Position, Color.White, false);
        }

        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
            throw new NotImplementedException();
        }

        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
            this.Velocity = new Vector2(Velocity.X, 0);
        }
    }
}
