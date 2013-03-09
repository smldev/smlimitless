using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites.Collections
{
    public class Level
    {
        private List<Tile> tiles;
        private List<Sprite> sprites;

        public const float GravityAcceleration = 300f;

        public Level()
        {
            tiles = new List<Tile>();
            sprites = new List<Sprite>();
        }

        public void Initialize()
        {
            sprites.Add(new TestSprite());
            sprites[0].Initialize(this);

            int j = 0;
            for (int i = 0; i < 800; i += 16)
            {
                tiles.Add(new TestTile());
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 432f);
                j++;
            }

            for (int i = 0; i < 800; i += 16)
            {
                tiles.Add(new TestTile2());
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 448f);
                j++;
            }

            for (int i = 0; i < 800; i += 16)
            {
                tiles.Add(new TestTile2());
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 464f);
                j++;
            }
        }

        public void LoadContent()
        {
            tiles.ForEach(t => t.LoadContent());
            sprites.ForEach(s => s.LoadContent());
        }

        public void Update()
        {
            tiles.ForEach(t => t.Update());


            foreach (Sprite s in sprites)
            {
                s.Acceleration = new Vector2(s.Acceleration.X, s.Acceleration.Y + GravityAcceleration);
                s.Update();
            }
        }

        public void Draw()
        {
            tiles.ForEach(t => t.Draw());
            sprites.ForEach(s => s.Draw());
        }
    }
}
