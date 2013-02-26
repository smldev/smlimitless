using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Graphics
{
    public static class SpritesheetManager
    {
        private static Dictionary<string, Spritesheet> sheetCache;

        public static void Initalize()
        {
            sheetCache = new Dictionary<string, Spritesheet>();
        }

        public static void AddSheetFromMetadata(string metadata)
        {
            // TODO: load from metadata, but only if we're not loaded
            // also, the filepath is always first - we can get that easily

            var split = metadata.Split('>');
            Spritesheet sheet = new Spritesheet();

            if (split[0].Contains("spritesheet"))
            {
                var data = split[1].Split(',');
                if (sheetCache.ContainsKey(MetadataHelpers.TrimQuotes(data[0]))) return;

                if (!split[0].Contains("nosize"))
                {
                    sheet.Load(MetadataHelpers.TrimQuotes(data[0]), Int32.Parse(data[1]), Int32.Parse(data[2]));
                }
                else
                {
                    sheet.Load(MetadataHelpers.TrimQuotes(data[0]));
                }
                sheetCache.Add(sheet.filePath, sheet);
            }
            else
            {
                throw new Exception("SpritesheetManager.LoadFromMetadata: Invalid metadata or metadata type.");
            }
        }

        public static void AddSheet(string filePath, int tileWidth, int tileHeight)
        {
            Spritesheet sheet = new Spritesheet();
            sheet.Load(filePath, tileWidth, tileHeight);
            sheetCache.Add(filePath, sheet);
        }

        public static void AddSheet(string filePath)
        {
            Spritesheet sheet = new Spritesheet();
            sheet.Load(filePath);
            sheetCache.Add(filePath, sheet);
        }

        public static void LoadContent()
        {
            foreach (var sheet in sheetCache.Values)
            {
                sheet.LoadContent();
            }
        }

        public static Texture2D GetTile(string filePath, int tileIndex)
        {
            if (sheetCache.ContainsKey(filePath))
            {
                return sheetCache[filePath].GetTile(tileIndex);
            }
            else
            {
                throw new Exception("SpritesheetManager.GetTile: Tried to get a tile from a sheet that didn't exist.");
            }
        }

        public static Texture2D GetTile(string filePath, Rectangle sourceRect)
        {
            if (sheetCache.ContainsKey(filePath))
            {
                return sheetCache[filePath].GetTile(sourceRect);
            }
            else
            {
                throw new Exception("SpritesheetManager.GetTile: Tried to get a tile from a sheet that didn't exist.");
            }
        }

        public static List<Texture2D> GetTiles(string filePath, List<Rectangle> sourceRects)
        {
            if (sheetCache.ContainsKey(filePath))
            {
                var result = new List<Texture2D>();
                foreach (Rectangle r in sourceRects)
                {
                    result.Add(sheetCache[filePath].GetTile(r));
                }
                return result;
            }
            else
            {
                throw new Exception("SpritesheetManager.GetTiles: Tried to get a tile from a sheet that didn't exist.");
            }
        }

        public static List<Texture2D> GetTiles(string filePath, List<int> tileIndexes)
        {
            if (sheetCache.ContainsKey(filePath))
            {
                var result = new List<Texture2D>();
                foreach (int i in tileIndexes)
                {
                    result.Add(sheetCache[filePath].GetTile(i));
                }
                return result;
            }
            else
            {
                throw new Exception("SpritesheetManager.GetTiles: Tried to get a tile from a sheet that didn't exist.");
            }
        }
    }
}
