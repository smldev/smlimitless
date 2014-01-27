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
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites.Collections.Structures
{
    public sealed class BackgroundData
    {
        private bool isLoaded;

        public Color TopColor { get; private set; }
        public Color BottomColor { get; private set; }
        public ReadOnlyCollection<BackgroundLayerData> Layers { get; private set; }

        public BackgroundData() { }

        public void Deserialize(JObject data)
        {
            var topColorData = data["background"]["topColor"];
            this.TopColor = new Color((int)topColorData["red"], (int)topColorData["green"], (int)topColorData["blue"], (int)topColorData["alpha"]);
            var bottomColorData = data["background"]["bottomColor"];
            this.BottomColor = new Color((int)bottomColorData["red"], (int)bottomColorData["green"], (int)bottomColorData["blue"], (int)bottomColorData["alpha"]);

            var layers = new List<BackgroundLayerData>();

            foreach (var layerJson in data["background"]["layers"])
            {
                BackgroundLayerData layer = new BackgroundLayerData();
                layer.Deserialize((JObject)layerJson);
            }
        }

        public string Serialize()
        {
            JObject result = JObject.FromObject(new
            {
                background = new
                {
                    topColor = new
                    {
                        red = this.TopColor.R,
                        green = this.TopColor.G,
                        blue = this.TopColor.B,
                        alpha = this.TopColor.A
                    },
                    bottomColor = new
                    {
                        red = this.BottomColor.R,
                        green = this.BottomColor.G,
                        blue = this.BottomColor.B,
                        alpha = this.BottomColor.A
                    },
                    layers =
                        from l in this.Layers
                        select new
                        {
                            graphicsName = l.GraphicsName,
                            scrollDirection = (int)l.ScrollDirection,
                            scrollRate = l.ScrollRate
                        }
                }
            });

            return result.ToString();
        }

        public void SetManually(Color topColor, Color bottomColor, List<BackgroundLayerData> layers)
        {
            if (!this.isLoaded)
            {
                this.TopColor = topColor;
                this.BottomColor = bottomColor;
                this.Layers = new ReadOnlyCollection<BackgroundLayerData>(layers);

                this.isLoaded = true;
            }
        }

        public sealed class BackgroundLayerData
        {
            private bool isLoaded;

            public string GraphicsName { get; private set; }
            public BackgroundScrollDirection ScrollDirection { get; private set; }
            public float ScrollRate { get; private set; }

            public BackgroundLayerData() { }

            public string Serialize()
            {
                JObject result = JObject.FromObject(new
                {
                    graphicsName = this.GraphicsName,
                    scrollDirection = (int)this.ScrollDirection,
                    scrollRate = this.ScrollRate
                });

                return result.ToString();
            }

            public void Deserialize(JObject data)
            {
                this.GraphicsName = (string)data["graphicsName"];
                this.ScrollDirection = (BackgroundScrollDirection)(int)data["scrollDirection"];
                this.ScrollRate = (float)data["scrollRate"];
            }

            public void SetManually(string graphicsName, BackgroundScrollDirection scrollDirection, float scrollRate)
            {
                if (!this.isLoaded)
                {
                    this.GraphicsName = graphicsName;
                    this.ScrollDirection = scrollDirection;
                    this.ScrollRate = scrollRate;

                    this.isLoaded = true;
                }
            }
        }
    }
}
