//-----------------------------------------------------------------------
// <copyright file="Section.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites.Collections.Structures
{
    public sealed class BackgroundData
    {
        public Color TopColor { get; private set; }
        public Color BottomColor { get; private set; }
        public ReadOnlyCollection<BackgroundLayerData> Layers { get; private set; }

        public BackgroundData() { }

        public void Deserialize()
        {

        }

        public void SetManually(Color topColor, Color bottomColor, List<BackgroundLayerData> layers)
        {
            this.TopColor = topColor;
            this.BottomColor = bottomColor;
            this.Layers = new ReadOnlyCollection<BackgroundLayerData>(layers);
        }

        public sealed class BackgroundLayerData
        {
            public string GraphicsName { get; private set; }
            public BackgroundScrollDirection ScrollDirection { get; private set; }
            public float ScrollRate { get; private set; }

            public BackgroundLayerData() { }

            public void Deserialize()
            {
            }

            public void SetManually(string graphicsName, BackgroundScrollDirection scrollDirection, float scrollRate)
            {
                this.GraphicsName = graphicsName;
                this.ScrollDirection = scrollDirection;
                this.ScrollRate = scrollRate;
            }
        }
    }
}
