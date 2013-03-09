using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Sprites.Collections
{
    public class Level
    {
        private List<Tile> tiles;
        private List<Sprite> sprites;

        public Level()
        {
            tiles = new List<Tile>();
            sprites = new List<Sprite>();
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            tiles.ForEach(t => t.LoadContent());
            sprites.ForEach(s => s.LoadContent());
        }

        public void Update()
        {
            tiles.ForEach(t => t.Update());
            sprites.ForEach(s => s.Update());
        }

        public void Draw()
        {
            tiles.ForEach(t => t.Draw());
            sprites.ForEach(s => s.Draw());
        }
    }
}
