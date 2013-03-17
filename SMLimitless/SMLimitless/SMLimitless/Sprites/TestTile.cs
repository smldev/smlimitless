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
    public class TestTile : Tile
    {
        StaticGraphicsObject graphics;

        public override void Initialize(Level owner)
        {
            graphics = new StaticGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphics.LoadFromMetadata(@"static-spritesheet>""" + absolute + @""",16,16,8");
            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            graphics.LoadContent();
        }

        public override void Update()
        {
        }

        public override void HandleCollision(Sprite sprite, Vector2 intersection)
        {
            // Offset the sprite's position by our intersection.
            if (Math.Abs(intersection.X) > Math.Abs(intersection.Y))
            {
                intersection.X = 0;
            }
            else if (Math.Abs(intersection.Y) > Math.Abs(intersection.X))
            {
                intersection.Y = 0;
            }
            else
            {
                intersection.X = 0;
            }
            sprite.Position += intersection;
        }

        public override void Draw()
        {
            graphics.Draw(this.Position, Color.White);
        }
    }
}
