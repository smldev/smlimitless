//-----------------------------------------------------------------------
// <copyright file="Section.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Physics;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A part of a level.
    /// </summary>
    public sealed class Section
    {
        private Level owner;

        public int Index { get; set; }
        public string Name { get; set; }
        public BoundingRectangle Bounds { get; private set; }
        public Camera2D Camera { get; private set; }

        public Background Background { get; private set; }

        internal Layer MainLayer { get; private set; }
        private List<Layer> layers;

        public Section(BoundingRectangle bounds)
        {
            // TODO: temporary
            this.Bounds = bounds;
            this.Camera = new Camera2D();
        }

        public void MoveCamera(Vector2 offset)
        {
            this.Camera.Position += offset;
        }

        public void Initialize(BackgroundData data) 
        {
            this.Background = new Background(this);
            this.Background.Initialize(data);
        }

        public void LoadContent() 
        {
            this.Background.LoadContent();
        }

        public void Update()
        {
            this.Background.Update();
        }

        public void Draw()
        {
            this.Background.Draw();
        }
    }
}
