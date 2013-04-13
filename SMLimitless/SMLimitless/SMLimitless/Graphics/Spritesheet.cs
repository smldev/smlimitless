using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// Defines a texture made up of many different tiles.
    /// </summary>
    internal class Spritesheet
    {
        internal string Metadata { get; private set; }

        private bool isLoaded;
        private bool isContentLoaded;
        internal string filePath;
        private Vector2 tileSize;

        private Texture2D sheetTexture;
        private Dictionary<Rectangle, Texture2D> croppedTiles;

        internal Spritesheet()
        {
            croppedTiles = new Dictionary<Rectangle, Texture2D>();
        }

        internal void LoadFromMetadata(string metadata)
        {
            if (!isLoaded)
            {
                var split1 = metadata.Split('>');
                var split2 = split1[1].Split(',');

                if (split1[0] == "spritesheet_def")
                {
                    // Metadata definition: spritesheet-def:“//filepath/image.png”,width,height
                    filePath = MetadataHelpers.TrimQuotes(split2[0]);
                    tileSize = new Vector2(Single.Parse(split2[1]), Single.Parse(split2[2]));
                    isLoaded = true;
                }
                else if (split1[0] == "spritesheet_nosize")
                {
                    // Metadata defintion: spritesheet-nosize>“//filepath/image.png”
                    filePath = MetadataHelpers.TrimQuotes(split2[0]);
                    tileSize = new Vector2(Single.NaN, Single.NaN);
                    isLoaded = true;
                }
                else
                {
                    throw new Exception("Spritesheet.LoadFromMetadata: Invalid metadata or metadata type.");
                }
            }
        }

        internal void Load(string filePath, int tileWidth, int tileHeight)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                this.tileSize = new Vector2(tileWidth, tileHeight);
                isLoaded = true;
            }
        }

        internal void Load(string filePath)
        {
            if (!isLoaded)
            {
                this.filePath = filePath;
                this.tileSize = new Vector2(Single.NaN, Single.NaN);
                isLoaded = true;
            }
        }

        internal void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                sheetTexture = GraphicsManager.LoadTextureFromFile(this.filePath);

                // Check the dimensions of the image to see if the tile size is divisible.
                if (!tileSize.IsNaN() && (sheetTexture.Width % tileSize.X != 0 || sheetTexture.Height % tileSize.Y != 0))
                {
                    throw new Exception(string.Format("The tile size {0}, {1} is invalid: the spritesheet cannot be evenly divided into tiles this size.", tileSize.X.ToString(), tileSize.Y.ToString()));
                }

                isContentLoaded = true;
            }
        }

        internal Texture2D GetTile(int tileIndex)
        {
            if (tileSize.IsNaN())
            {
                throw new Exception("This spritesheet cannot get tiles by index.  Please use a Rectangle to get the tiles instead.");
            }
            Rectangle sourceRect;
            int tilesPerRow = sheetTexture.Width / (int)tileSize.X;
            int tilesPerColumn = sheetTexture.Height / (int)tileSize.Y;

            int row = tileIndex / tilesPerRow, column = tileIndex % tilesPerRow;
            row *= (int)tileSize.X;
            column *= (int)tileSize.Y;

            sourceRect = new Rectangle(column, row, (int)tileSize.X, (int)tileSize.Y);
            if (!croppedTiles.ContainsKey(sourceRect))
            {
                Texture2D croppedTexture = sheetTexture.Crop(sourceRect);
                croppedTiles.Add(sourceRect, croppedTexture);
            }
            return croppedTiles[sourceRect];
        }

        internal Texture2D GetTile(Rectangle sourceRect)
        {
            if (!croppedTiles.ContainsKey(sourceRect))
            {
                Texture2D croppedTexture = sheetTexture.Crop(sourceRect);
                croppedTiles.Add(sourceRect, croppedTexture);
            }
            return croppedTiles[sourceRect];
        }
    }
}
