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
            Size = new Vector2(16f, 16f);
            graphics = new StaticGraphicsObject();
            string absolute = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..\\..\\..\\gfx\\smw_grass_top.png");
            graphics.Load(absolute);
            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            graphics.LoadContent();
        }

        public override void Update()
        {
        }

        public override void HandleCollision(Sprite sprite, Intersection intersect)
        {

        }

        public override void Draw()
        {
            graphics.Draw(Position, Color.White);
        }
    }
}
