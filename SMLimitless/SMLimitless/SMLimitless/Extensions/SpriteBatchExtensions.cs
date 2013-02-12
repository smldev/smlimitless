using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless.Extensions
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, SpriteEffects effects)
        {
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            spriteBatch.Draw(texture, destRect, null, color, 0.0f, Vector2.Zero, effects, 0f);
        }
    }
}
