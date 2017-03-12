using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
	internal sealed class QuadRenderer : IDisposable
	{
		// http://gamedev.stackexchange.com/questions/87150/rendering-a-fullscreen-quad-is-leaving-a-one-pixel-line-on-the-left-and-top

		private GraphicsDevice gfx;
		private short[] indexData = new short[] { 0, 1, 2, 2, 3, 0 };
		private Texture2D transparentTexture = new Texture2D(GameServices.Graphics, 1, 1);
		private VertexPositionTexture[] triangles;

        /// <summary>
        /// Gets a value indicating whether the resources for this object have been released.
        /// </summary>
        public bool IsDisposed { get; private set; }

		public QuadRenderer()
		{
			gfx = GameServices.Graphics;
			transparentTexture.SetData(new[] { Color.Transparent });

			// texture coordinates semantic not used or needed
			triangles = new VertexPositionTexture[]
						{
					    new VertexPositionTexture(new Vector3(1, -1, 0),
												 Vector2.Zero),
					    new VertexPositionTexture(new Vector3(-1, -1, 0),
												 Vector2.Zero),
					    new VertexPositionTexture(new Vector3(-1, 1, 0),
												 Vector2.Zero),
					    new VertexPositionTexture(new Vector3(1, 1, 0),
												 Vector2.Zero)
						};
		}

		public void Render(Effect effect, Vector2 position)
		{
			foreach (EffectPass p in effect.CurrentTechnique.Passes)
				p.Apply();

			Render(position);
		}

		private void Render(Vector2 position)
		{
			//gfx.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
			//								   triangles, 0, 4,
			//								   indexData, 0, 2);
			GameServices.SpriteBatch.DrawRectangle(position.ToRectangle(GameServices.ScreenSize), Color.Transparent);
		}


        void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (transparentTexture != null && !transparentTexture.IsDisposed)
                    {
                        transparentTexture.Dispose();
                    }
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
