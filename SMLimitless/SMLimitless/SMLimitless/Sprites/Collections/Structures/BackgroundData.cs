//-----------------------------------------------------------------------
// <copyright file="Section.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites.Collections.Structures
{
    internal sealed class BackgroundData
    {
        public Color TopColor { get; private set; }
        public Color BottomColor { get; private set; }

        private sealed class BackgroundLayerData
        {
            public string GraphicsName { get; private set; }

        }
    }
}
