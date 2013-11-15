using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
    }
}
