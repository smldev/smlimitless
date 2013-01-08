using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// Defines a texture made up of many different tiles.
    /// </summary>
    public class Spritesheet
    {
        public string Metadata { get; private set; }

        private bool isLoaded;
        private bool isContentLoaded;
        private string filePath;
        private Vector2 tileSize;

        private Texture2D sheetTexture;
        private Dictionary<Rectangle, Texture2D> croppedTiles;

        public Spritesheet()
        {

        }

        public void LoadFromMetadata(string metadata)
        {
            if (!isLoaded)
            {
                // Metadata definition: spritesheet-def:“//filepath/image.png”,width,height
                var split1 = metadata.Split('>');
                var split2 = split1[1].Split(',');

                if (split2.Length != 3) throw new ArgumentException("Spritesheet metadata definition had an invalid number of items.", "metadata");

                // split2[0] is the file path.
                filePath = split2[0].Substring(1, split2[0].Length - 2); // remove the quotes

                // split2[1] and [2] are the width and height
                int width = Int32.Parse(split2[1]);
                int height = Int32.Parse(split2[2]);
                tileSize = new Vector2(width, height);

                isLoaded = true;
            }
        }

        public void Load(string filePath, int tileWidth, int tileHeight)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                this.tileSize = new Vector2(tileWidth, tileHeight);
                isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                sheetTexture = GraphicsManager.LoadFromFile(this.filePath);

                // Check the dimensions of the image to see if the tile size is divisible.
                if (sheetTexture.Width % tileSize.X != 0 || sheetTexture.Height % tileSize.Y != 0)
                {
                    throw new Exception(string.Format("The tile size {0}, {1} is invalid: the spritesheet cannot be evenly divided into tiles this size.", tileSize.X.ToString(), tileSize.Y.ToString()));
                }

                isContentLoaded = true;
            }
        }

        public Texture2D GetTile(int tileIndex)
        {
            if (tileSize.X < 0 || tileSize.Y < 0) // if the tile size is negative, getting tiles by index is not available (i.e. sprites are differently sized)
            {
                throw new Exception("This spritesheet cannot get tiles by index.  Please use a Rectangle to get the tiles instead.");
            }

            Rectangle sourceRect;
            int tilesPerRow = sheetTexture.Width / (int)tileSize.X;
            int tilesPerColumn = sheetTexture.Height / (int)tileSize.Y;

            int row = 0, column = 0;
            for (; tileIndex > 0 && tileIndex < tilesPerRow; tileIndex -= tilesPerRow) { row++; }
            column = tileIndex;

            row *= (int)tileSize.Y;
            column *= (int)tileSize.X;

            sourceRect = new Rectangle(row, column, (int)tileSize.X, (int)tileSize.Y);
            return sheetTexture.Crop(sourceRect);
        }

        public Texture2D GetTile(Rectangle sourceRect)
        {
            return sheetTexture.Crop(sourceRect);
        }
    }
}
