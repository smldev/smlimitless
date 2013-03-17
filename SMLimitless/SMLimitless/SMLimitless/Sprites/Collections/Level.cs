﻿using System;
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

        private MouseFollowSprite mouseSprite;

        private QuadTree quadTree;

        public const float GravityAcceleration = 64f;

        public Level()
        {
            tiles = new List<Tile>();
            sprites = new List<Sprite>();
            mouseSprite = new MouseFollowSprite();

            quadTree = new QuadTree(new Vector2(64, 64));
        }

        public void Initialize()
        {
            TestSprite sprite = new TestSprite();
            sprites.Add(sprite);
            sprites[0].Initialize(this);
            mouseSprite.Initialize(this);
            quadTree.Add(sprite);
            quadTree.Add(mouseSprite);

            int j = 0;
            for (int i = 0; i < 800; i += 16)
            {
                TestTile tile = new TestTile();
                tiles.Add(tile);
                tiles[j].Initialize(this);
                tiles[j].Position = new Vector2(i, 432f);
                quadTree.Add(tile);
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
            mouseSprite.Update();
            quadTree.Update();

            mouseSprite.CheckCollisions(quadTree.GetCollidableTiles(mouseSprite), new List<Sprite>());
        }

        public void Draw()
        {
            tiles.ForEach(t => t.Draw());
            sprites.ForEach(s => s.Draw());
            mouseSprite.Draw();
            quadTree.Draw();
        }
    }
}
