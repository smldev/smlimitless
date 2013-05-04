using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    public interface IGraphicsObject
    {
        void Load(string filePath);
        void Load(string filePath, DataReader config);
        void LoadContent();
        void Update();
        void Draw(Vector2 position, Color color);
        void Draw(Vector2 position, Color color, SpriteEffects effects);
        IGraphicsObject Clone();
    }
}
