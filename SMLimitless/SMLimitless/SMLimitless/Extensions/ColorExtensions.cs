using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Extensions
{
    public static class ColorExtensions
    {
        public static Color FromPackedValue(uint packed)
        {
            // We assume that Color.PackedValue is RGBA.
            int red = (int)(packed >> 24);
            int green = (int)((packed << 8) >> 24);
            int blue = (int)((packed << 16) >> 24);
            int alpha = (int)((packed << 24) >> 24);

            return new Color(red, green, blue, alpha);
        }

        public static Color ToColor(this JToken jsonEntry)
        {
            string json = (string)jsonEntry;
            string[] values = json.Split(',');

            if (values.Length != 4)
            {
                throw new ArgumentException("ColorExtensions.ToColor(this JToken): Tried to turn a non-color object into a color.");
            }

            for (int i = 1; i < 4; i++) { values[i] = values[i].TrimStart(); } // there are spaces on the front of [1] through [3]

            int red, green, blue, alpha;
            if (!int.TryParse(values[0], out red)) { throw new Exception(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for red: {0}", values[0])); }
            if (!int.TryParse(values[1], out green)) { throw new Exception(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for green: {0}", values[1])); }
            if (!int.TryParse(values[2], out blue)) { throw new Exception(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for blue: {0}", values[2])); }
            if (!int.TryParse(values[3], out alpha)) { throw new Exception(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for alpha: {0}", values[3])); }

            return new Color(red, green, blue, alpha);
        }
    }
}
