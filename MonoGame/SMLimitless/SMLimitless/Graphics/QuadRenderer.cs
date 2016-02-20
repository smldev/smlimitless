using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
	internal sealed class QuadRenderer
	{
		// http://gamedev.stackexchange.com/questions/87150/rendering-a-fullscreen-quad-is-leaving-a-one-pixel-line-on-the-left-and-top

		private VertexPositionTexture[] triangles;
		private short[] indexData = new short[] { 0, 1, 2, 2, 3, 0 };
		private GraphicsDevice gfx;

		private Texture2D transparentTexture = new Texture2D(GameServices.Graphics, 1, 1);

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

		public void Render(Effect effect)
		{
			foreach (EffectPass p in effect.CurrentTechnique.Passes)
				p.Apply();

			Render();
		}

		private void Render()
		{
			//gfx.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
			//								   triangles, 0, 4,
			//								   indexData, 0, 2);
			GameServices.SpriteBatch.DrawRectangle(Vector2.Zero.ToRectangle(GameServices.ScreenSize), Color.Transparent);
		}
	}
}