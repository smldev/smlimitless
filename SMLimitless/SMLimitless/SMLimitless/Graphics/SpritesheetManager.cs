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
        private Dictionary<string, Spritesheet> sheetCache;

        public void LoadFromMetadata(string metadata)
        {
            // TODO: load from metadata, but only if we're not loaded
            // also, the filepath is always first - we can get that easily

            var split = metadata.Split('>');
            Spritesheet sheet = new Spritesheet();

            if (split[0].Contains("spritesheet"))
            {
                var data = split[1].Split(',');
                if (!sheetCache.ContainsKey(MetadataHelpers.TrimQuotes(data[0]))) return;

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

        public void Load(string filePath, int tileWidth, int tileHeight)
        {
            Spritesheet sheet = new Spritesheet();
            sheet.Load(filePath, tileWidth, tileHeight);
            sheetCache.Add(filePath, sheet);
        }

        public void Load(string filePath)
        {
            Spritesheet sheet = new Spritesheet();
            sheet.Load(filePath);
            sheetCache.Add(filePath, sheet);
        }

        public void LoadContent()
        {
            foreach (var sheet in sheetCache.Values)
            {
                sheet.LoadContent();
            }
        }


    }
}
