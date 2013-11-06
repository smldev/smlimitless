using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Sprites.Collections
{
    public sealed class Background
    {
        public Color TopColor { get; private set; }
        public Color BottomColor { get; private set; }

        private List<BackgroundLayer> layers;
        private bool isContentLoaded;
        private Texture2D backgroundGradient;
        private Section owner;

        public Background(Section owner)
        {
            this.layers = new List<BackgroundLayer>();
            this.owner = owner;
        }

        public void Initialize(BackgroundData data)
        {
            this.TopColor = data.TopColor;
            this.BottomColor = data.BottomColor;

            foreach (var layerData in data.Layers)
            {
                BackgroundLayer layer = new BackgroundLayer(owner.Camera, owner.Bounds);
                layer.Initialize(layerData.GraphicsName, layerData.ScrollDirection, layerData.ScrollRate);
                this.layers.Add(layer);
            }
        }

        public void LoadContent()
        {
            if (!this.isContentLoaded)
            {
                Color[] color = new Color[] { this.TopColor, this.BottomColor };
                this.backgroundGradient = new Texture2D(GameServices.Graphics, 1, 2);
                this.backgroundGradient.SetData<Color>(color);

                this.layers.ForEach(l => l.LoadContent());
                this.isContentLoaded = true;
            }
        }

        public void Update()
        {
            layers.ForEach(l => l.Update());
        }

        public void Draw()
        {
            // A gradient can be drawn by stretching a 1-wide by 2-high texture over the screen.
            // The top third is the first color, and the bottom third is the second color. The middle is the actual gradient.
            // Thus, the texture needs to be made taller than the screen - the top needs to be drawn one-third of a screen higher than the screen,
            // and the height needs to be a screen height plus two-thirds of a screen height.
            float screenHeight = GameServices.ScreenSize.Y;
            float gradientTop = owner.Camera.Viewport.Y - (screenHeight / 3f);
            float gradientHeight = owner.Camera.Viewport.Height + (screenHeight * (2f / 3f));
            GameServices.SpriteBatch.Draw(this.backgroundGradient, new Rectangle((int)owner.Camera.Viewport.X, (int)gradientTop, (int)owner.Camera.ViewportSize.X, (int)gradientHeight), Color.White);

            layers.ForEach(l => l.Draw());
        }

        public void AddLayerToFront(BackgroundLayer layer)
        {
            this.layers.Add(layer);

            if (this.isContentLoaded)
            {
                layer.LoadContent();
            }
        }
    }
}
