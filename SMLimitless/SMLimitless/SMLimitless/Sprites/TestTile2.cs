using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Graphics;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites
{
    public class TestTile2 : Tile
    {
        StaticGraphicsObject graphics;

        public override void Initialize(Level owner)
        {
            graphics = new StaticGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\test_sheet.png");
            graphics.LoadFromMetadata(@"static-spritesheet>""" + absolute + @""",16,16,12");
            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            graphics.LoadContent();
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            graphics.Draw(this.Position, Color.White);
        }

        public override void HandleCollision(Sprite sprite)
        {
            
        }
    }
}
