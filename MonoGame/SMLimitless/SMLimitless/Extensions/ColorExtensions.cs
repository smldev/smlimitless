//-----------------------------------------------------------------------
// <copyright file="ColorExtensions.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Extensions
{
    /// <summary>
    /// Extends the Color structure.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Creates an RGBA color from a packed, unsigned 32-bit integer.
        /// </summary>
        /// <param name="packed">The packed, unsigned 32-bit integer.</param>
        /// <returns>The color extracted from the packed integer.</returns>
        public static Color FromPackedValue(uint packed)
        {
            // We assume that Color.PackedValue is RGBA.
            int red = (int)(packed >> 24);
            int green = (int)((packed << 8) >> 24);
            int blue = (int)((packed << 16) >> 24);
            int alpha = (int)((packed << 24) >> 24);

            return new Color(red, green, blue, alpha);
        }

        /// <summary>
        /// Converts a JToken into a Color.
        /// </summary>
        /// <param name="jsonEntry">A JSON token containing four comma-delimited integers between 0 and 255.</param>
        /// <returns>A color extracted from the token.</returns>
        public static Color ToColor(this JToken jsonEntry)
        {
            string json = (string)jsonEntry;
            string[] values = json.Split(',');

            if (values.Length != 4)
            {
                throw new ArgumentException(string.Format("ColorExtensions.ToColor(this JToken): Tried to turn a non-color object into a color. Entry: {0}", json));
            }

            for (int i = 1; i < 4; i++) { values[i] = values[i].TrimStart(); } // there are spaces on the front of [1] through [3]

            int red, green, blue, alpha;
            if (!int.TryParse(values[0], out red)) { throw new JsonException(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for red: {0}", values[0])); }
            if (!int.TryParse(values[1], out green)) { throw new JsonException(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for green: {0}", values[1])); }
            if (!int.TryParse(values[2], out blue)) { throw new JsonException(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for blue: {0}", values[2])); }
            if (!int.TryParse(values[3], out alpha)) { throw new JsonException(string.Format("ColorExtensions.ToColor(this JToken): Invalid value for alpha: {0}", values[3])); }

            return new Color(red, green, blue, alpha);
        }

        /// <summary>
        /// Serializes a color, producing a string usable by serializers/deserializers.
        /// </summary>
        /// <param name="value">The color to serialize.</param>
        /// <returns>A string in the format of "R, G, B, A".</returns>
        public static string Serialize(this Color value)
        {
            return string.Format("{0}, {1}, {2}, {3}", value.R, value.G, value.B, value.A);
        }
    }
}
