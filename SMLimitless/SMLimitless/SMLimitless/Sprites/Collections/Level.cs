using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    public class Level
    {
        private List<Tile> tiles;
        private List<Sprite> sprites;

        private QuadTree quadTree;

        public const float GravityAcceleration = 300f;

        public Level()
        {
            tiles = new List<Tile>();
            sprites = new List<Sprite>();

            quadTree = new QuadTree(new Vector2(64, 64));
        }

        public void Initialize()
        {
            TestSprite sprite = new TestSprite();
            sprites.Add(sprite);
            quadTree.Add(sprite);
            sprites[0].Initialize(this);

            int j = 0;
            for (int i = 0; i < 800; i += 16)
            {
                TestTile tile = new TestTile();
                tiles.Add(tile);
                quadTree.Add(tile);
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

            quadTree.Update();

            // Temporary collision checks
            
        }

        public void Draw()
        {
            tiles.ForEach(t => t.Draw());
            sprites.ForEach(s => s.Draw());

            quadTree.Draw();
        }
    }
}
